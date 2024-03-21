using BettaLib.Geometry;
using BettaLib.FEAModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BettaLib.Utils;
using System.Xml.Linq;

namespace BettaLib.FEAStructure
{

    public class Structure
    {
        public Structure() { }
        public NodeCollection<Node> strNodes = new();
        public EdgeCollection<Beam> strBeams = new();
        public LoadCase? LoadCase { get; set; }

        //Methods
        public Node AddNode(Point3 p)
        {
            Node n = strNodes.EnsureNode(p, Constants.Epsilon);
            return n;
        }
        public Node AddNode(double x, double y, double z)
        {
            Point3 p = new Point3(x, y, z);
            Node n = strNodes.EnsureNode(p, Constants.Epsilon);
            return n;
        }

        public Beam AddBeam(Node start, Node end, CrossSection cs)
        {
            Beam b = strBeams.EnsureEdge(new Beam(start, end, cs), Constants.Epsilon);
            return b;
        }

        public FEModel Solve(LoadCase loadcase)
        {
            FEModel model = new FEModel(this, loadcase);
            model.PerformAnalysis();
            return model;
        }

        public override string ToString()
        {
            return $"Structure with {strNodes.Nodes.Count} nodes and {strBeams.Edges.Count} beams";
        }
    }
}
