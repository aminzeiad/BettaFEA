using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace BettaGH.BettaFDM
{
    public class GH_FDMModelSolver : GH_Component
    {

        FDMModel fdmModel = null;

        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public GH_FDMModelSolver()
          : base("Betta FDM Solver", "FDM Solver",
              "A Solver for the force density method",
              "Betta", "Solvers")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Betta FDM Mesh", "M", "A Betta_FDMModel mesh to perform the Finite Density Method on", GH_ParamAccess.item);
            pManager.AddNumberParameter("Force Densities", "Q", "A list of force densities. Equals to the number of edges", GH_ParamAccess.list);
            pManager.AddVectorParameter("Loads", "P", "A load to be applied to all mesh vertices", GH_ParamAccess.list);
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
            // Define placeholder variables
            List<double> forceDensities = new List<double>();
            List<Vector3d> loads = new List<Vector3d>();

            // Get the inputs
            DA.GetData(0, ref fdmModel);
            DA.GetDataList(1, forceDensities);
            DA.GetDataList(2, loads);

            // Unwrap the GH_FDMModel to get the FDMModel
            if (fdmModel == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid FDMModel");
                return;
            }

            // Ensure loads and force densities are valid
            if (loads.Count != fdmModel.NodeCount)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The number of loads must match the number of nodes.");
                return;
            }

            // Proceed with the solver using fdmModel data
            // Assemble Q
            int branchCount = fdmModel.BranchCount;
            var Q = SparseMatrix.CreateDiagonal(branchCount, branchCount, 0);

            for (int e = 0; e < branchCount; e++)
            {
                double q = forceDensities.Count == 1 ? forceDensities[0] : forceDensities[e];
                Q[e, e] = q;
            }

            // Assemble Equilibrium Equations
            var K = fdmModel.C_f.TransposeThisAndMultiply(Q).Multiply(fdmModel.C_f);

            // Adjust for External Forces and Fixed Nodes
            int freeNodeCount = fdmModel.FreeNodeIndices.Count;
            var Fx = Vector.Build.Dense(freeNodeCount, 0);
            var Fy = Vector.Build.Dense(freeNodeCount, 0);
            var Fz = Vector.Build.Dense(freeNodeCount, 0);

            for (int i = 0; i < fdmModel.NodeCount; i++)
            {
                Vector3d force = loads[i];

                if (fdmModel.FreeNodeIndexMap.ContainsKey(i))
                {
                    int localIndex = fdmModel.FreeNodeIndexMap[i];
                    Fx[localIndex] += force.X;
                    Fy[localIndex] += force.Y;
                    Fz[localIndex] += force.Z;
                }
            }

            // Contributions from fixed nodes
            var R_x = fdmModel.C_f.TransposeThisAndMultiply(Q).Multiply(fdmModel.C_s).Multiply(fdmModel.x_s);
            var R_y = fdmModel.C_f.TransposeThisAndMultiply(Q).Multiply(fdmModel.C_s).Multiply(fdmModel.y_s);
            var R_z = fdmModel.C_f.TransposeThisAndMultiply(Q).Multiply(fdmModel.C_s).Multiply(fdmModel.z_s);

            // Right-hand side vectors
            var RHS_x = Fx - R_x;
            var RHS_y = Fy - R_y;
            var RHS_z = Fz - R_z;

            // Solve for Unknown Node Positions
            var x_f = K.Solve(RHS_x);
            var y_f = K.Solve(RHS_y);
            var z_f = K.Solve(RHS_z);

            // Update Mesh with New Positions
            Point3d[] nodes = fdmModel.Nodes;

            for (int i = 0; i < freeNodeCount; i++)
            {
                int globalIndex = fdmModel.FreeNodeIndices[i];
                nodes[globalIndex] = new Point3d(x_f[i], y_f[i], z_f[i]);
            }

            //// Create a new mesh to avoid modifying the original
            Mesh updatedMesh = new Mesh();
            //updatedMesh.Vertices.AddVertices(nodes);
            //updatedMesh.Faces.AddFaces(fdmModel.Nodes.Select((_, i) => fdmModel.Nodes[i]).ToArray());
            //updatedMesh.TopologyEdges.AddEdges(fdmModel.C);

            // Alternatively, you can clone and update the original mesh
            updatedMesh = fdmModel.Mesh.DuplicateMesh();
            updatedMesh.Vertices.Clear();
            updatedMesh.Vertices.AddVertices(nodes);

            // Output the updated mesh
            DA.SetData(0, updatedMesh);
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
            get { return new Guid("E5493A6E-EA90-4058-8262-5BFC37C4C625"); }
        }
    }
}