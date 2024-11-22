using Rhino.Geometry;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Rhino;

public class FDMModel
{
    public int NodeCount { get; private set; }
    public int BranchCount { get; private set; }
    public Point3d[] Nodes { get; private set; }
    public List<int> FreeNodeIndices { get; private set; }
    public List<int> FixedNodeIndices { get; private set; }
    public Dictionary<int, int> FreeNodeIndexMap { get; private set; }
    public Dictionary<int, int> FixedNodeIndexMap { get; private set; }
    public SparseMatrix C { get; private set; }
    public SparseMatrix C_f { get; private set; }
    public SparseMatrix C_s { get; private set; }
    public Vector<double> x_s { get; private set; }
    public Vector<double> y_s { get; private set; }
    public Vector<double> z_s { get; private set; }
    public Mesh Mesh { get; private set; }

    public FDMModel(Mesh mesh, List<int> fixedNodeIndices)
    {
        Mesh = mesh;
        BuildModel(mesh, fixedNodeIndices);
    }

    private void BuildModel(Mesh mesh, List<int> fixedNodeIndices)
    {
        // Extract Nodes
        Nodes = mesh.Vertices.ToPoint3dArray();
        NodeCount = Nodes.Length;
        FixedNodeIndices = fixedNodeIndices;

        // Construct Branch-Node Incidence Matrix C
        BranchCount = mesh.TopologyEdges.Count;
        C = SparseMatrix.Create(BranchCount, NodeCount, 0);

        for (int e = 0; e < BranchCount; e++)
        {
            IndexPair edgeVertices = mesh.TopologyEdges.GetTopologyVertices(e);
            int startVertex = mesh.TopologyVertices.MeshVertexIndices(edgeVertices.I)[0];
            int endVertex = mesh.TopologyVertices.MeshVertexIndices(edgeVertices.J)[0];

            C[e, startVertex] = 1;
            C[e, endVertex] = -1;
        }

        // Identify Free Nodes
        FreeNodeIndices = Enumerable.Range(0, NodeCount).Except(FixedNodeIndices).ToList();

        // Create Mappings
        FreeNodeIndexMap = new Dictionary<int, int>();
        for (int i = 0; i < FreeNodeIndices.Count; i++)
        {
            FreeNodeIndexMap[FreeNodeIndices[i]] = i;
        }

        FixedNodeIndexMap = new Dictionary<int, int>();
        for (int i = 0; i < FixedNodeIndices.Count; i++)
        {
            FixedNodeIndexMap[FixedNodeIndices[i]] = i;
        }

        // Partition C into C_f and C_s
        C_f = SparseMatrix.Create(BranchCount, FreeNodeIndices.Count, 0);
        C_s = SparseMatrix.Create(BranchCount, FixedNodeIndices.Count, 0);

        for (int e = 0; e < BranchCount; e++)
        {
            for (int n = 0; n < NodeCount; n++)
            {
                double value = C[e, n];
                if (value != 0)
                {
                    if (FreeNodeIndexMap.ContainsKey(n))
                    {
                        int localIndex = FreeNodeIndexMap[n];
                        C_f[e, localIndex] = value;
                    }
                    else if (FixedNodeIndexMap.ContainsKey(n))
                    {
                        int localIndex = FixedNodeIndexMap[n];
                        C_s[e, localIndex] = value;
                    }
                }
            }
        }

        // Store Fixed Node Positions
        x_s = Vector.Build.Dense(FixedNodeIndices.Count);
        y_s = Vector.Build.Dense(FixedNodeIndices.Count);
        z_s = Vector.Build.Dense(FixedNodeIndices.Count);

        for (int i = 0; i < FixedNodeIndices.Count; i++)
        {
            int globalIndex = FixedNodeIndices[i];
            x_s[i] = Nodes[globalIndex].X;
            y_s[i] = Nodes[globalIndex].Y;
            z_s[i] = Nodes[globalIndex].Z;
        }
    }
}