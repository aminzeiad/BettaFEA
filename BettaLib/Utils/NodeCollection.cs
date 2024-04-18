using BettaLib.Geometry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BettaLib.Utils
{
    // Define NodeCollection<TNode> where TNode is any class that implements INode and has a parameterless constructor
    public class NodeCollection<TNode> : IEnumerable<TNode> where 
        TNode : INode, 
        new()
    {
        private List<TNode> Nodes { get; } = new List<TNode>();

        public int Count => Nodes.Count;

        // Implement the GetEnumerator method to enable foreach iterations over NodeCollection
        public IEnumerator<TNode> GetEnumerator()
        {
            return Nodes.GetEnumerator();
        }

        public void Clear()
        {
            Nodes.Clear();
        }

        // Explicit implementation for IEnumerable.GetEnumerator, required for non-generic interface
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        //allow for indexing
        public TNode this[int index]
        {
            get { return Nodes[index]; }
            set { Nodes[index] = value; }
        }

        public void AddNode(Point3 p)
        {
            TNode n = EnsureNode(p, Constants.Epsilon);
            Nodes.Add(n);
        }

        public TNode AddNode(double x, double y, double z)
        {
            Point3 p = new Point3(x, y, z);
            TNode n = EnsureNode(p, Constants.Epsilon);
            //Nodes.Add(n);
            return n;
        }

        public TNode EnsureNode(Point3 p, double tol)
        {
            var fn = FindNode(p, tol);
            if (fn != null) return fn;
            fn = new TNode { Position = p };
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
