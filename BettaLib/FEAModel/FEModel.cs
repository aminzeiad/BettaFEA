using BettaLib.FEAStructure;
using BettaLib.Geometry;
using BettaLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
//using System.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace BettaLib.FEAModel
{
    public class FEModel
    {
        public FEModel(Structure structure, LoadCase loadcase)
        {
            Structure = structure;
            LoadCase = loadcase;
        }

        public Structure Structure { get; set; }//this structure should be immutable
        public LoadCase LoadCase { get; set; } //this loadcase should be immutable

        public NodeCollection<FENode> feNodes = new();
        public EdgeCollection<FEBeam> feBeams = new();


        Dictionary<Node, FENode> nodeNodeMap = new();
        public Matrix<double> GlobalStiffnessMatrix { get; set; }
        public Vector<double> GlobalLoadMatrix { get; set; }
        public Matrix<double> GlobalDisplacementMatrix { get; set; }
        public Matrix<double> EquivalentNodalForces { get; set; }


        public void PerformAnalysis()
        {
            //Clear the model of any previous results
            feNodes.Clear();
            feBeams.Clear();

            //Prepare the finite element model
            PrepareModel(); 

            //Solve linear static analysis
            SolveLinearStatic();
        }

        private void LockMatrixVariable(int id, double value)
        {
           int nx = GlobalStiffnessMatrix.RowCount;
            for (int i=0; i<nx; ++i)
            {
                var entry = GlobalStiffnessMatrix[i, id];
                GlobalStiffnessMatrix[i, id] = 0.0;
                GlobalStiffnessMatrix[id, i] = 0.0;
                GlobalLoadMatrix[i] -= value * entry; //fixing the other side subtract the value that we know
            }

            GlobalStiffnessMatrix[id,id]= 1.0;
            GlobalLoadMatrix[id] = value;
        }

        private void LockMatrixVariableToZero(int id)
        {
            int nx = GlobalStiffnessMatrix.RowCount;
            for (int i = 0; i < nx; ++i)
            {
                GlobalStiffnessMatrix[i, id] = 0.0;
                GlobalStiffnessMatrix[id, i] = 0.0;
            }

            GlobalStiffnessMatrix[id, id] = 1.0;
            GlobalLoadMatrix[id] = 0.0;
        }

        private void LockMatrixVariableFast(int id, double value)
        {
            const double veryLarge = 90000000000000.0;

            GlobalStiffnessMatrix[id, id] = veryLarge;
            GlobalLoadMatrix[id] = value * veryLarge;
        }

        private void SolveLinearStatic()
        {
            InitializeSystem();

            CalculateGlobalStiffnessMatrix();

            foreach(FENode n in feNodes)
            {
                for(int i = 0; i < 6; ++i)
                {
                    if (n.HasSupport((DOFID)i))
                    {
                        LockMatrixVariable(n.GetGlobalDOF((DOFID)i), 0.0);
                    }                   
                }
            }
            

            //SolveHooksLaw(); //F = K * u

            //UndoStaticCondensation();

            //StoreDisplacements();
        }

        private void CalculateGlobalStiffnessMatrix() 
        {
            int size = feNodes.Count * 6;
            var K = Matrix<double>.Build.Sparse(size, size);
            var R = Vector<double>.Build.Dense(size);

            int id = 0;
            foreach(FENode n in feNodes)
            {
                n.Id = id;
                ++id;


                n.FillInLoad(R);
            }

            foreach (FEBeam b in feBeams)
            {
                b.AssembleOnGlobalStiffnessMatrix(K);
                //b.InitializeLocalEquivalentLoad(R); //In case the beam has a linear load on - linear load is not implemented yet
            }

            GlobalStiffnessMatrix = K;
            GlobalLoadMatrix = R;
        }

        private void InitializeSystem()
        {
            int DOF = 6;

            // 1 node needs 6 DOF, 10 nodes need 60 DOF ....etc
            int numNodes = feNodes.Count; 
            int numLoads = LoadCase.Loads.Count;

            GlobalStiffnessMatrix = Matrix<double>.Build.Sparse(numNodes * DOF, numNodes * DOF);
            GlobalLoadMatrix = Vector<double>.Build.Sparse(numNodes * DOF);
            GlobalDisplacementMatrix = Matrix<double>.Build.Sparse(numNodes * DOF, 1);
            EquivalentNodalForces = Matrix<double>.Build.Sparse(numNodes * DOF, 1);

            foreach (FENode n in feNodes)
            {
                n.Deflections = Matrix<double>.Build.Sparse(DOF, 1);
            }
        }

        public void PrepareModel()
        {
            List<BeamSplits> beamSplits = new List<BeamSplits>();

            //Add FENodes at the start and end of each beam and use EnsureNode to avoid duplicates
            foreach (Beam b in Structure.strBeams)
            {
                FENode n1 = feNodes.EnsureNode(b.N0.Position, Constants.Epsilon);
                if (b.N0.Support!= null)
                {
                    n1.Support = b.N0.Support;
                    n1.IsSupportNode = true;
                    //set the support type
                    if (n1.Support.Ux == true) n1.SupportType |= FENodeSupportType.DX;
                    if (n1.Support.Uy == true) n1.SupportType |= FENodeSupportType.DY;
                    if (n1.Support.Uz == true) n1.SupportType |= FENodeSupportType.DZ;
                    if (n1.Support.Rxx == true) n1.SupportType |= FENodeSupportType.RX;
                    if (n1.Support.Ryy == true) n1.SupportType |= FENodeSupportType.RY;
                    if (n1.Support.Rzz == true) n1.SupportType |= FENodeSupportType.RZ;
                    
                }
                //I never set values for isSupportNode and Origin
                n1.Origin = b.N0;
                FENode n2 = feNodes.EnsureNode(b.N1.Position, Constants.Epsilon);
                //I never set values for isSupportNode and Origin
                n2.Origin = b.N1;


                beamSplits.Add(new BeamSplits(b, n1, n2));
            }

            //apply loads
            foreach (Load l in LoadCase.Loads)
            {
                if (l is LoadNodal pl)
                {
                    Point3 p = new Point3(pl.NodeAppliedOn.Position.X, pl.NodeAppliedOn.Position.Y, pl.NodeAppliedOn.Position.Z);
                    FENode n = feNodes.EnsureNode(pl.NodeAppliedOn.Position, Constants.Epsilon);

                    Vector3 f = new Vector3(pl.Fx, pl.Fy, pl.Fz);
                    Vector3 m = new Vector3(pl.Mx, pl.My, pl.Mz);
                    n.ApplyLoad(f, m);
                }
                //else if (l is LinearLoad ll) //Linear load is not implemented yet
            }

            //check for intersections
            foreach (BeamSplits b1 in beamSplits)
            {
                Line3 l1 = new Line3(b1.beam.N0.Position, b1.beam.N1.Position);
                foreach (BeamSplits b2 in beamSplits)
                {
                    if (b1 != b2)
                    {
                        Line3 l2 = new Line3(b2.beam.N0.Position, b2.beam.N1.Position);
                        var (success, t1, t2, p1, p2) = Line3.Intersect(l1, l2, Constants.Epsilon);
                        if (success)
                        {
                            Point3 xp = (p1 + p2) * 0.5;
                            FENode xnode = feNodes.EnsureNode(xp, Constants.Epsilon);
                            XEvent? x = b1.FindEvent(xnode);
                            if (x == null)
                            {
                                b1.AddEvent(xnode, t1);
                            }
                            x = b2.FindEvent(xnode);
                            if (x == null)
                            {
                                b2.AddEvent(xnode, t2);
                            }
                        }
                    }
                }
            }


            //split beams
            List<FEBeam> DupFEBeams = new List<FEBeam>();
            foreach (BeamSplits bs in beamSplits)
            {
                List<FEBeam> newBeams = bs.SplitBeam();
                DupFEBeams.AddRange(newBeams);
            }

            //Check for duplicate beams
            foreach (FEBeam b in DupFEBeams)
            {
                FEBeam fb = feBeams.EnsureEdge(b, Constants.Epsilon);
                //I need to make sure that the local cordinates are set properly
                fb.CrossSection = b.CrossSection;
                fb.Vxx = b.Vxx;
                fb.Vyy = b.Vyy;
                fb.Vzz = b.Vzz;

            }

        }

        struct XEvent
        {
            public double t;
            public FENode Node;
        }

        class BeamSplits
        {
            public Beam beam;
            public List<XEvent> XEvents = new();

            public object GlobalStiffnessMatrix { get; private set; }

            public BeamSplits(Beam b, FENode n1, FENode n2)
            {
                beam = b;
                XEvents = new();

                AddEvent(n1, 0.0);
                AddEvent(n2, 1.0);
            }

            public void AddEvent(FENode n)
            {

                XEvents.Add(new XEvent { t = n.Position.DistanceTo(beam.N0.Position) / beam.Length, Node = n });
            }

            public void AddEvent(FENode n, double t)
            {

                XEvents.Add(new XEvent { t = t, Node = n });
            }

            public XEvent? FindEvent(FENode n)
            {
                //if we detect a new intersectin do we already have an event for it?
                foreach (XEvent x in XEvents)
                {
                    if (x.Node == n) return x;
                }
                return null;

            }


            public List<FEBeam> SplitBeam()
            {
                List<FEBeam> newBeams = new List<FEBeam>();
                XEvents.Sort((x, y) => x.t.CompareTo(y.t));

                for (int i = 0; i < XEvents.Count - 1; ++i)
                {
                    newBeams.Add(new FEBeam(XEvents[i].Node, XEvents[i + 1].Node, beam));
                }
                return newBeams;
            }

        }
        public String PrintGlobalStiffnessMatrix()
        {
            //display the global stiffness matrix in a readable format

            return GlobalStiffnessMatrix.ToString(30, 30);
        }

        public String PrintGlobalLoadMatrix()
        {
            //display the global load matrix in a readable format

            return GlobalLoadMatrix.ToString(30, 30);
        }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("__________________________________________________________________________________");
            sb.AppendLine("__________________________________________________________________________________");
            sb.AppendLine("");

            sb.AppendLine("FE Model: ");
            sb.AppendLine("__________________________________________________________________________________");
            sb.AppendLine("__________________________________________________________________________________");
            sb.AppendLine("");


            sb.AppendLine("Beams: ");
            sb.AppendLine("__________________________________________________________________________________");
            sb.AppendLine("__________________________________________________________________________________");


            foreach (FEBeam b in feBeams)
            {
                sb.AppendLine("A Beam FROM: " + b.N0 + "TO: " + b.N1);
                sb.AppendLine("T Matrix: ");
                sb.AppendLine(b.PrintTransformationMatrix());
                sb.AppendLine("k Matrix: ");
                sb.AppendLine(b.PrintLocalStiffnessMatrix());
                sb.AppendLine("T' k T Matrix ");
                sb.AppendLine(b.PrintGlobalElementalStiffnessMatrix());
                sb.AppendLine("______________________________________________________________________________");
                sb.AppendLine("______________________________________________________________________________");
            }
            sb.AppendLine("");

            sb.AppendLine("Nodes: ");
            sb.AppendLine("______________________________________________________________________________");
            sb.AppendLine("______________________________________________________________________________");
            foreach (FENode n in feNodes)
            {
                sb.AppendLine(n.ToString());
                sb.AppendLine(n.PrintForceVector());
                sb.AppendLine(n.PrintMomentVector());
                sb.AppendLine(n.PrintDisplacementVector());
                sb.AppendLine(n.PrintDeflections());
                sb.AppendLine("______________________________________________________________________________");
                sb.AppendLine("______________________________________________________________________________");


            }
            sb.AppendLine("Global Stiffness Matrix: ");
            sb.AppendLine(PrintGlobalStiffnessMatrix());
            sb.AppendLine("Global Load Matrix: ");
            sb.AppendLine(PrintGlobalLoadMatrix());

            return sb.ToString();
        }

    }
}
