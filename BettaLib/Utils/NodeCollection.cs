using BettaLib.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BettaLib.Utils
{
    public class NodeCollection<TNode> where
        TNode : INode,
        new()
    {
        public List<TNode> Nodes { get; set; } = new List<TNode>();
        public void AddNode(Point3 p)
        {
            TNode n = EnsureNode(p, Constants.Epsilon);
            Nodes.Add(n);
        }
        public TNode AddNode(double x, double y, double z)
        {
            Point3 p = new Point3(x, y, z);
            TNode n = EnsureNode(p, Constants.Epsilon);
            return n;
        }
        public TNode EnsureNode(Point3 p, double tol)
        {
            var fn = FindNode(p, tol);
            if (fn != null) return fn;
            fn = new TNode();
            fn.Position = p;
            Nodes.Add(fn);
            return fn;
        }
        public TNode? FindNode(Point3 p, double tol)
        {
            foreach (TNode n in Nodes)
            {
                if (p.IsEqualTo(n.Position, tol)) return n;
            }
            return default;
        }
    }
}
