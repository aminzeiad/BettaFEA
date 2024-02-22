using BettaLib.Geometry;
using BettaLib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BettaLib.Elements
{
    public class Structure
    {
        public Structure() { }
        public List <Node> Nodes { get; set; } = new List <Node>(); //Consider using your own NodeList class to add methods for adding and removing nodes
        public List <Beam> Beams { get; set; } = new List<Beam>(); //Consider using your own BeamList class to add methods for adding and removing beams
        public LoadCase? LoadCase { get; set; }

        //Methods
        public Node AddNode(Point3 node)
        {
            //Need to add a check for duplicate nodes
            Node n = new Node(node);
            Nodes.Add(n);
            return n;
        }
        public Node AddNode(double x, double y, double z)
        {
            //Need to add a check for duplicate nodes
            Node n = new Node(x, y, z);
            Nodes.Add(n);
            return n;
        }
        public Beam AddBeam(Line3 line, CrossSection cs)
        {
            //Need to add a check for duplicate beams
            //Need to check for intersection with other beams - should this be done now or 
            Node start = AddNode(line.Start);
            Node end = AddNode(line.End);
            Beam b = new Beam(start, end, cs);
            Beams.Add(b);
            return b;
        }
        public Beam AddBeam(Node start, Node end, CrossSection cs)
        {
            //Need to add a check for duplicate beams
            //Need to check for intersection with other beams

            Beam b = new Beam(start, end, cs);
            Beams.Add(b);
            return b;
        }

        public FEModel Solve(LoadCase loadcase)
        {
            FEModel model = new FEModel(this, loadcase);
            model.PerformAnalysis();
            return model;
        }
    }
}
