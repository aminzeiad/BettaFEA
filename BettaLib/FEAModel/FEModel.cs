using BettaLib.FEAStructure;
using BettaLib.Geometry;
using BettaLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

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
        double[,] GlobalStiffnessMatrix;
        double[,] GlobalLoadMatrix;
        double[,] GlobalDisplacementMatrix;
        double[,] EquivalentNodalForces;

        public void PerformAnalysis()
        {
            //Clear the model of any previous results

            //Prepare the finite element model
            PrepareModel();

            //Solve linear static analysis
            SolveLinearStatic();
        }

        private void SolveLinearStatic()
        {
            InitializeSystem();

            CalculateGlobalStiffnessMatrix();
            
            //ApplyLoads();

            //PerformStaticCondensation();

            //SolveHooksLaw(); //F = K * u

            //UndoStaticCondensation();

            //StoreDisplacements();
        }

        private void CalculateGlobalStiffnessMatrix() 
        {
            foreach (FEBeam b in feBeams.Edges)
            {
                //b.CalculateLocalStiffnessMatrix();
                //b.CalculateTransformationMatrix(feBeams.Count); //Depends on the local axes of the beam
                //b.CalculateGlobalStiffnessMatrix(); //T' * K * T
               // b.InitializeLocalEquivalentLoad();

            }
        }

        private void InitializeSystem()
        {
            int DOF = 6;

            // 1 node needs 6 DOF, 10 nodes need 60 DOF ....etc
            int numNodes = feNodes.Nodes.Count; //Is this risky?
            int numLoads = LoadCase.Loads.Count;
            
            GlobalStiffnessMatrix = new double[numNodes * DOF, numNodes * DOF];
            GlobalLoadMatrix = new double[numNodes * DOF, 1];
            GlobalDisplacementMatrix = new double[numNodes * DOF, 1];
            EquivalentNodalForces = new double[numNodes * DOF, 1];

        }

        public void PrepareModel()
        {
            List<BeamSplits> beamSplits = new List<BeamSplits>();

            //Add FENodes at the start and end of each beam and use EnsureNode to avoid duplicates
            foreach (Beam b in Structure.strBeams.Edges)
            {
                FENode n1 = feNodes.EnsureNode(b.N0.Position, Constants.Epsilon);
                FENode n2 = feNodes.EnsureNode(b.N1.Position, Constants.Epsilon);


                beamSplits.Add(new BeamSplits(b, n1, n2));
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
                feBeams.EnsureEdge(b, Constants.Epsilon);
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
    }
}
