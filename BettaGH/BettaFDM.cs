using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino.Geometry.Collections;
using Rhino.Render.ChangeQueue;
using Rhino;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Double;

namespace BettaGH
{
    public class BettaFDM : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public BettaFDM()
          : base("Betta FDM", "FDM",
              "Perform a Force Density Method Analysis using BettaLib",
              "Betta", "Solvers")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Input Mesh", "M", "A mesh to perform the Finite Density Method on", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Fixed Points", "FP", "A list of fixed points", GH_ParamAccess.list);
            pManager.AddVectorParameter("Loads", "P", "A load to be applied to all mesh vertices", GH_ParamAccess.list);
            pManager.AddNumberParameter("Force Densities", "Q", "A list of force densities. Equals to the number of edges", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Form-found Mesh", "M", "A form-found mesh as a result of the FDM", GH_ParamAccess.item);

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //Define placeholder variables
            Rhino.Geometry.Mesh mesh = new();
            List<int> fixedNodeIndices = new();
            List<Vector3d> loads = new();
            List<double> forceDensities = new();

            //Place the inputs into these variables
            DA.GetData(0, ref mesh);
            DA.GetDataList(1, fixedNodeIndices);
            DA.GetDataList(2, loads);
            DA.GetDataList(3, forceDensities);

            //---------- CODE STARTS HERE ----------//

            //Extracting the nodes fron the mesh
            Point3d[] nodes = mesh.Vertices.ToPoint3dArray(); // Node positions
            int nodeCount = nodes.Length;

            //Constructing the Branch-Node Matrix C
            int branchCount = mesh.TopologyEdges.Count;
            var C = SparseMatrix.Create(branchCount, nodeCount, 0);

            for (int e = 0; e < branchCount; e++)
            {
                IndexPair edgeVertices = mesh.TopologyEdges.GetTopologyVertices(e);
                int startVertex = mesh.TopologyVertices.MeshVertexIndices(edgeVertices.I)[0];
                int endVertex = mesh.TopologyVertices.MeshVertexIndices(edgeVertices.J)[0];

                C[e, startVertex] = 1;
                C[e, endVertex] = -1;
            }

            //Identify which nodes are fixed and which are free
            List<int> freeNodeIndices = Enumerable.Range(0, nodeCount).Except(fixedNodeIndices).ToList();
            int freeNodeCount = freeNodeIndices.Count;
            int fixedNodeCount = fixedNodeIndices.Count;

            // Create mapping from global node indices to local indices
            Dictionary<int, int> freeNodeIndexMap = new Dictionary<int, int>();
            for (int i = 0; i < freeNodeIndices.Count; i++)
            {
                freeNodeIndexMap[freeNodeIndices[i]] = i;
            }

            //Partition the matrix C into Cf and Cx
            // Create empty matrices for C_f and C_s
            var C_f = SparseMatrix.Create(branchCount, freeNodeCount, 0);
            var C_s = SparseMatrix.Create(branchCount, fixedNodeCount, 0);

            // Create mapping from global fixed node indices to local indices
            Dictionary<int, int> fixedNodeIndexMap = new Dictionary<int, int>();
            for (int i = 0; i < fixedNodeIndices.Count; i++)
            {
                fixedNodeIndexMap[fixedNodeIndices[i]] = i;
            }

            // Fill in C_f and C_s
            for (int e = 0; e < branchCount; e++)
            {
                for (int n = 0; n < nodeCount; n++)
                {
                    double value = C[e, n];
                    if (value != 0)
                    {
                        if (freeNodeIndexMap.ContainsKey(n))
                        {
                            int localIndex = freeNodeIndexMap[n];
                            C_f[e, localIndex] = value;
                        }
                        else if (fixedNodeIndexMap.ContainsKey(n))
                        {
                            int localIndex = fixedNodeIndexMap[n];
                            C_s[e, localIndex] = value;
                        }
                    }
                }
            }

            //Assemble the diagonal force density matrix Q
            var Q = SparseMatrix.Create(branchCount, branchCount, 0);

            for (int e = 0; e < branchCount; e++)
            {
                double q = forceDensities.Count == 1 ? forceDensities[0] : forceDensities[e];
                Q[e, e] = q;
            }

            //Assemble the equilibrium equation (Cft * Q * Cf) = p - Cft * Q * Cs
            //compute the left hand side matrix

            var K = C_f.TransposeThisAndMultiply(Q).Multiply(C_f);

            // Initialize force vectors for free nodes
            var Fx = Vector<double>.Build.Dense(freeNodeCount, 0);
            var Fy = Vector<double>.Build.Dense(freeNodeCount, 0);
            var Fz = Vector<double>.Build.Dense(freeNodeCount, 0);

            // Apply external loads to free nodes
            for (int i = 0; i < nodeCount; i++)
            {
                Vector3d force = loads[i];

                if (freeNodeIndexMap.ContainsKey(i))
                {
                    int localIndex = freeNodeIndexMap[i];
                    Fx[localIndex] += force.X;
                    Fy[localIndex] += force.Y;
                    Fz[localIndex] += force.Z;
                }
            }

            var x_s = Vector<double>.Build.Dense(fixedNodeCount);
            var y_s = Vector<double>.Build.Dense(fixedNodeCount);
            var z_s = Vector<double>.Build.Dense(fixedNodeCount);

            for (int i = 0; i < fixedNodeIndices.Count; i++)
            {
                int globalIndex = fixedNodeIndices[i];
                x_s[i] = nodes[globalIndex].X;
                y_s[i] = nodes[globalIndex].Y;
                z_s[i] = nodes[globalIndex].Z;
            }

            var R_x = C_f.TransposeThisAndMultiply(Q).Multiply(C_s).Multiply(x_s);
            var R_y = C_f.TransposeThisAndMultiply(Q).Multiply(C_s).Multiply(y_s);
            var R_z = C_f.TransposeThisAndMultiply(Q).Multiply(C_s).Multiply(z_s);


            var RHS_x = Fx - R_x;
            var RHS_y = Fy - R_y;
            var RHS_z = Fz - R_z;

            var x_f = K.Solve(RHS_x);
            var y_f = K.Solve(RHS_y);
            var z_f = K.Solve(RHS_z);

            for (int i = 0; i < freeNodeIndices.Count; i++)
            {
                int globalIndex = freeNodeIndices[i];
                nodes[globalIndex] = new Point3d(x_f[i], y_f[i], z_f[i]);
            }

            mesh.Vertices.Clear();
            mesh.Vertices.AddVertices(nodes);


            DA.SetData(0, mesh);

        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                var imageBytes = Properties.Resources.icon_FDM_solver;
                using (var ms = new System.IO.MemoryStream(imageBytes))
                {
                    return new System.Drawing.Bitmap(ms);
                }
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("F145DA0D-69EB-41E2-A3E9-FDAC010B9AAF"); }
        }
    }
}