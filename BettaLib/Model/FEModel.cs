﻿using BettaLib.Elements;
using BettaLib.Geometry;
using BettaLib.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BettaLib.Model
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
        public List<FEBeam> feBeams { get; set; } = new();
        public List<FENode> fENodes { get; set; } = new();
        Dictionary<Node, FENode> nodeNodeMap = new();

        public void PerformAnalysis()
        {
            //Perform analysis
            PrepareModel();
        }

        public FENode? FindNode(Node n)
        {
            if (nodeNodeMap.TryGetValue(n, out FENode feNode))
            {
                return feNode;
            }
            return null;
        }

        public FENode EnsureNode(Node n)
        {
            var fn = FindNode(n);
            if (fn != null) return fn;
            fn = new FENode(n.Position, n);
            fENodes.Add(fn);
            nodeNodeMap.Add(n, fn);
            return fn;
        }

        public FENode? FindNode(Point3 p, double tol)
        {
            foreach (FENode n in fENodes)
            {
                if (p.IsEqualTo(n.Position, tol)) return n;
            }
            return null;
        }

        public FENode EnsureNode(Point3 p, double tol)
        {
            var fn = FindNode(p, tol);
            if (fn != null) return fn;
            fn = new FENode(p);
            fENodes.Add(fn);
            return fn;
        }

        public void PrepareModel()
        {
            //List <FENode> nodes = new List<FENode>();
            //List <FEBeam> beams = new List<FEBeam>();
            List<BeamSplits> beamSplits = new List<BeamSplits>();

            foreach (Beam b in Structure.Beams)
            {
                FENode n1 = EnsureNode(b.Node1);
                FENode n2 = EnsureNode(b.Node2);
        
                
                beamSplits.Add(new BeamSplits(b, n1, n2));
            }

            //foreach (Node n in Structure.Nodes)
            //{
            //    FENode feNode = EnsureNode(n);
            //}

            //check for intersections
            foreach (BeamSplits b1 in beamSplits)
            {
                Line3 l1 = new Line3(b1.beam.Node1.Position, b1.beam.Node2.Position);
                foreach (BeamSplits b2 in beamSplits)
                {
                    if (b1 != b2)
                    {                        
                        Line3 l2 = new Line3(b2.beam.Node1.Position, b2.beam.Node2.Position);
                        var (success, t1, t2, p1, p2) = Line3.Intersect(l1, l2, Constants.Epsilon);
                        if (success)
                        {
                            Point3 xp = (p1 + p2) * 0.5;
                            FENode xnode = EnsureNode(xp, Constants.Epsilon);
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
            foreach (BeamSplits bs in beamSplits)
            {
                List<FEBeam> newBeams = bs.SplitBeam();
                feBeams.AddRange(newBeams);
            }

            ////make sure we have unique nodes
            //nodes = nodes.Distinct().ToList();

            ////make sure we have unique beams
            //beams = beams.Distinct().ToList();

            ////add nodes and beams to the model
            //fENodes = nodes;
            //feBeams = beams;


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
                this.beam = b;
                XEvents = new();

                AddEvent(n1, 0.0);  
                AddEvent(n2, 1.0);
            }

            public void AddEvent( FENode n)
            {

                XEvents.Add(new XEvent { t = n.Position.DistanceTo(beam.Node1.Position)/beam.Length, Node = n });
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

                for (int i =0; i<XEvents.Count-1; ++i)
                {
                    newBeams.Add(new FEBeam(XEvents[i].Node, XEvents[i+1].Node, beam));
                }
                return newBeams;
            }
        }
    }
}