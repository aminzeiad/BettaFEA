using BettaLib.FEAModel;
using BettaLib.Geometry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BettaLib.Utils
{
    // Define EdgeCollection<TEdge> where TEdge is any class that implements IEdge and has a parameterless constructor
    public class EdgeCollection<TEdge> : IEnumerable<TEdge> where 
        TEdge : IEdge, 
        new()
    {
        private List<TEdge> Edges { get; } = new List<TEdge>();

        public int Count => Edges.Count;

        // Implement the GetEnumerator method to enable foreach iterations over EdgeCollection
        public IEnumerator<TEdge> GetEnumerator()
        {
            return Edges.GetEnumerator();
        }

        public void Clear()
        {
            Edges.Clear();
        }

        // Explicit implementation for IEnumerable.GetEnumerator, required for non-generic interface
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void AddEdge(INode n0, INode n1)
        {
            TEdge e = new TEdge { N0 = n0, N1 = n1 };
            e = EnsureEdge(e, Constants.Epsilon);
            // No need to add the edge here again since EnsureEdge already adds it
        }

        public TEdge EnsureEdge(TEdge e, double tol)
        {
            var fe = FindEdge(e, tol);
            if (fe != null) return fe;
            fe = new TEdge { N0 = e.N0, N1 = e.N1 };
            Edges.Add(fe);
            return fe;
        }

        public TEdge? FindEdge(TEdge e0, double tol)
        {
            Line3 l = new Line3(e0.N0.Position, e0.N1.Position);
            foreach (TEdge e1 in Edges)
            {
                Line3 l1 = new Line3(e1.N0.Position, e1.N1.Position);
                if (l.IsEqualTo(l1, tol)) return e1;
                // Simplified the condition for clarity
                if ((e0.N0 == e1.N0 && e0.N1 == e1.N1) || (e0.N0 == e1.N1 && e0.N1 == e1.N0)) return e1;
            }
            return default;
        }
    }
}
