using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino.Geometry.Collections;
using Rhino;
using System.Linq;
using TorchSharp;
using static TorchSharp.torch;
using TorchSharp.Modules;

namespace BettaGH.BettaFDM
{
    public class GH_OLDBettaFDM : GH_Component
    {
        // Persistent variables

        /// <summary>
        /// Initializes a new instance of the GH_OLDBettaFDM class.
        /// </summary>
        public GH_OLDBettaFDM()
          : base("Betta FDM Solver - Legacy", "FDM Solver",
              "Perform a Force Density Method Analysis using BettaLib",
              "Betta", "Solvers")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Input Mesh", "M", "A mesh to perform the Force Density Method on", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Fixed Points", "FP", "A list of fixed points", GH_ParamAccess.list);
            pManager.AddVectorParameter("Loads", "P", "A load to be applied to all mesh vertices", GH_ParamAccess.list);
            pManager.AddNumberParameter("Force Densities", "Q", "A list of force densities. Equals to the number of edges", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
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
            Rhino.Geometry.Mesh mesh = new();
            List<int> fixedNodeIndices = new();
            List<Vector3d> loads = new();
            List<double> initialForceDensities = new();

            // Retrieve inputs
            if (!DA.GetData(0, ref mesh)) return;
            if (!DA.GetDataList(1, fixedNodeIndices)) return;
            if (!DA.GetDataList(2, loads)) return;
            if (!DA.GetDataList(3, initialForceDensities)) return;

            // Extract nodes from the mesh
            Point3d[] nodes = mesh.Vertices.ToPoint3dArray(); // Node positions
            int nodeCount = nodes.Length;

            // Ensure loads have the same length as nodes
            if (loads.Count != nodeCount)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Loads list must be the same length as the number of nodes.");
                return;
            }

            // Convert loads to tensors
            Tensor loadsTensor = torch.zeros(new long[] { nodeCount, 3 }, dtype: torch.float32);
            for (int i = 0; i < nodeCount; i++)
            {
                loadsTensor[i, 0] = (float)loads[i].X;
                loadsTensor[i, 1] = (float)loads[i].Y;
                loadsTensor[i, 2] = (float)loads[i].Z;
            }

            // Initialize force densities q as a tensor with requires_grad=true
            int branchCount = mesh.TopologyEdges.Count;

            double[] initialQValues = new double[branchCount];
            if (initialForceDensities.Count == 1)
            {
                for (int i = 0; i < branchCount; i++)
                {
                    initialQValues[i] = initialForceDensities[0];
                }
            }
            else if (initialForceDensities.Count == branchCount)
            {
                initialQValues = initialForceDensities.ToArray();
            }
            else
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Force Densities list must have length 1 or equal to the number of branches.");
                return;
            }

            Tensor q = torch.tensor(initialQValues, dtype: torch.float32, requires_grad: true).reshape(new long[] { branchCount, 1 });
            var q_param = new Parameter(q);

            // Build connectivity matrix C
            Tensor C = torch.zeros(new long[] { branchCount, nodeCount }, dtype: torch.float32);

            for (int e = 0; e < branchCount; e++)
            {
                IndexPair edgeVertices = mesh.TopologyEdges.GetTopologyVertices(e);
                int startVertex = mesh.TopologyVertices.MeshVertexIndices(edgeVertices.I)[0];
                int endVertex = mesh.TopologyVertices.MeshVertexIndices(edgeVertices.J)[0];

                C[e, startVertex] = 1.0f;
                C[e, endVertex] = -1.0f;
            }

            // Identify free and fixed nodes
            List<int> freeNodeIndices = Enumerable.Range(0, nodeCount).Except(fixedNodeIndices).ToList();
            int freeNodeCount = freeNodeIndices.Count;
            int fixedNodeCount = fixedNodeIndices.Count;

            // Create index tensors
            Tensor freeNodeIndicesTensor = torch.tensor(freeNodeIndices.Select(i => (long)i).ToArray());
            Tensor fixedNodeIndicesTensor = torch.tensor(fixedNodeIndices.Select(i => (long)i).ToArray());

            // Partition C into C_f and C_s
            Tensor C_f = C.index_select(1, freeNodeIndicesTensor);
            Tensor C_s = C.index_select(1, fixedNodeIndicesTensor);

            // Get fixed node positions
            Tensor x_s = torch.zeros(new long[] { fixedNodeCount, 1 }, dtype: torch.float32);
            Tensor y_s = torch.zeros(new long[] { fixedNodeCount, 1 }, dtype: torch.float32);
            Tensor z_s = torch.zeros(new long[] { fixedNodeCount, 1 }, dtype: torch.float32);
            for (int i = 0; i < fixedNodeCount; i++)
            {
                int globalIndex = fixedNodeIndices[i];
                x_s[i] = (float)nodes[globalIndex].X;
                y_s[i] = (float)nodes[globalIndex].Y;
                z_s[i] = (float)nodes[globalIndex].Z;
            }

            // Prepare target positions (assuming you want to match the original mesh)
            Tensor targetPositions = torch.zeros(new long[] { nodeCount, 3 }, dtype: torch.float32);
            for (int i = 0; i < nodeCount; i++)
            {
                targetPositions[i, 0] = (float)nodes[i].X;
                targetPositions[i, 1] = (float)nodes[i].Y;
                targetPositions[i, 2] = (float)nodes[i].Z;
            }

            // Define weights for loss terms
            double w_shape = 1.0;       // Weight for shape matching loss
            double w_equilibrium = 0.1; // Weight for equilibrium loss

            // Initialize optimizer
            var optimizer = torch.optim.Adam(new[] { q_param }, lr: 0.01);

            int maxEpochs = 1000;
            double tolerance = 1e-6;

            for (int epoch = 0; epoch < maxEpochs; epoch++)
            {
                optimizer.zero_grad();

                // Compute equilibrium positions
                ComputeEquilibriumPositions(q_param, C, x_s, y_s, z_s, loadsTensor, freeNodeIndices, fixedNodeIndices, out Tensor x, out Tensor y, out Tensor z);

                // Ensure x, y, z are of shape [nodeCount]
                x = x.squeeze();
                y = y.squeeze();
                z = z.squeeze();

                // Stack x, y, z to form positions tensor of shape [nodeCount, 3]
                Tensor positions = torch.stack(new Tensor[] { x, y, z }, dim: 1); // Shape: [nodeCount, 3]

                // Compute the total loss with weights
                Tensor loss = ComputeLoss(positions, targetPositions, q_param, C, loadsTensor, freeNodeIndices, w_shape, w_equilibrium);

                // Backward pass
                loss.backward();

                // Update q
                optimizer.step();

                // Optional: Monitor progress
                double lossValue = loss.item<float>();
                Console.WriteLine($"Epoch {epoch}: Total Loss = {lossValue}");

                // Convergence check
                if (lossValue < tolerance)
                {
                    // Converged
                    Console.WriteLine("Converged!");
                    break;
                }
            }

            // After optimization, compute final positions
            ComputeEquilibriumPositions(q_param, C, x_s, y_s, z_s, loadsTensor, freeNodeIndices, fixedNodeIndices, out Tensor final_x, out Tensor final_y, out Tensor final_z);

            // Convert tensors to arrays and update mesh
            double[] xArray = final_x.detach().cpu().data<float>().Select(f => (double)f).ToArray();
            double[] yArray = final_y.detach().cpu().data<float>().Select(f => (double)f).ToArray();
            double[] zArray = final_z.detach().cpu().data<float>().Select(f => (double)f).ToArray();

            for (int i = 0; i < nodeCount; i++)
            {
                nodes[i] = new Point3d(xArray[i], yArray[i], zArray[i]);
            }

            mesh.Vertices.Clear();
            mesh.Vertices.AddVertices(nodes);

            DA.SetData(0, mesh);
        }

        Tensor ComputeEquilibriumResiduals(
            Tensor q_param,
            Tensor positions,
            Tensor C,
            Tensor loadsTensor,
            List<int> freeNodeIndices)
        {
            // Compute edge vectors
            Tensor edgeVectors = torch.matmul(C, positions); // Shape: [branchCount, 3]

            // Compute forces along edges
            Tensor forces = q_param * edgeVectors; // Element-wise multiplication

            // Compute net forces at nodes
            Tensor nodeForces = torch.matmul(C.transpose(0, 1), forces); // Shape: [nodeCount, 3]

            // Subtract external loads
            nodeForces -= loadsTensor;

            // Extract free node forces
            Tensor freeNodeIndicesTensor = torch.tensor(freeNodeIndices.Select(i => (long)i).ToArray());
            Tensor freeNodeForces = nodeForces.index_select(0, freeNodeIndicesTensor);

            return freeNodeForces; // Shape: [freeNodeCount, 3]
        }

        private Tensor ComputeLoss(
            Tensor positions,
            Tensor targetPositions,
            Tensor q_param,
            Tensor C,
            Tensor loadsTensor,
            List<int> freeNodeIndices,
            double w_shape,
            double w_equilibrium)
        {
            // Compute the shape matching loss
            Tensor diff = positions - targetPositions;
            Tensor L_shape = torch.sum(torch.pow(diff, 2));

            // Compute the equilibrium residuals
            Tensor residuals = ComputeEquilibriumResiduals(q_param, positions, C, loadsTensor, freeNodeIndices);

            // Compute the equilibrium loss
            Tensor L_equilibrium = torch.sum(torch.pow(residuals, 2));

            // Compute the total loss with weights
            Tensor totalLoss = w_shape * L_shape + w_equilibrium * L_equilibrium;

            return totalLoss;
        }

        private void ComputeEquilibriumPositions(
            Tensor q_param,
            Tensor C,
            Tensor x_s,
            Tensor y_s,
            Tensor z_s,
            Tensor loadsTensor,
            List<int> freeNodeIndices,
            List<int> fixedNodeIndices,
            out Tensor x,
            out Tensor y,
            out Tensor z)
        {
            // Convert indices to tensors
            Tensor freeNodeIndicesTensor = torch.tensor(freeNodeIndices.Select(i => (long)i).ToArray());
            Tensor fixedNodeIndicesTensor = torch.tensor(fixedNodeIndices.Select(i => (long)i).ToArray());

            // Compute Q
            Tensor Q = torch.diag(q_param.squeeze());

            // Extract C_f and C_s
            Tensor C_f = C.index_select(1, freeNodeIndicesTensor); // Shape: [branchCount, freeNodeCount]
            Tensor C_s = C.index_select(1, fixedNodeIndicesTensor); // Shape: [branchCount, fixedNodeCount]

            // Transpose C_f and C_s
            Tensor C_f_T = C_f.transpose(0, 1); // Shape: [freeNodeCount, branchCount]
            // Tensor C_s_T = C_s.transpose(0, 1); // Not used further

            // Compute D_N and D_F
            Tensor D_N = torch.matmul(torch.matmul(C_f_T, Q), C_f); // Shape: [freeNodeCount, freeNodeCount]
            Tensor D_F = torch.matmul(torch.matmul(C_f_T, Q), C_s); // Shape: [freeNodeCount, fixedNodeCount]

            // Compute external forces for free nodes
            Tensor loads_f = loadsTensor.index_select(0, freeNodeIndicesTensor); // Shape: [freeNodeCount, 3]

            // Extract Fx, Fy, Fz using .select()
            Tensor Fx = loads_f.select(1, 0).reshape(new long[] { freeNodeIndices.Count, 1 });
            Tensor Fy = loads_f.select(1, 1).reshape(new long[] { freeNodeIndices.Count, 1 });
            Tensor Fz = loads_f.select(1, 2).reshape(new long[] { freeNodeIndices.Count, 1 });

            // Compute RHS
            Tensor R_x = torch.matmul(D_F, x_s);
            Tensor R_y = torch.matmul(D_F, y_s);
            Tensor R_z = torch.matmul(D_F, z_s);

            Tensor RHS_x = Fx - R_x;
            Tensor RHS_y = Fy - R_y;
            Tensor RHS_z = Fz - R_z;

            // Solve for x_f, y_f, z_f
            Tensor x_f = torch.linalg.solve(D_N, RHS_x);
            Tensor y_f = torch.linalg.solve(D_N, RHS_y);
            Tensor z_f = torch.linalg.solve(D_N, RHS_z);

            // Reconstruct full node positions
            int nodeCount = (int)C.shape[1];

            x = torch.zeros(new long[] { nodeCount, 1 }, dtype: torch.float32);
            y = torch.zeros(new long[] { nodeCount, 1 }, dtype: torch.float32);
            z = torch.zeros(new long[] { nodeCount, 1 }, dtype: torch.float32);

            // Set positions for free nodes
            x.scatter_(0, freeNodeIndicesTensor.reshape(new long[] { freeNodeIndicesTensor.shape[0], 1 }), x_f);
            y.scatter_(0, freeNodeIndicesTensor.reshape(new long[] { freeNodeIndicesTensor.shape[0], 1 }), y_f);
            z.scatter_(0, freeNodeIndicesTensor.reshape(new long[] { freeNodeIndicesTensor.shape[0], 1 }), z_f);

            // Set positions for fixed nodes
            x.scatter_(0, fixedNodeIndicesTensor.reshape(new long[] { fixedNodeIndicesTensor.shape[0], 1 }), x_s);
            y.scatter_(0, fixedNodeIndicesTensor.reshape(new long[] { fixedNodeIndicesTensor.shape[0], 1 }), y_s);
            z.scatter_(0, fixedNodeIndicesTensor.reshape(new long[] { fixedNodeIndicesTensor.shape[0], 1 }), z_s);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
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
