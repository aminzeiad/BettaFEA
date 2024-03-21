using BettaLib.FEAModel;
using BettaLib.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BettaLib.Utils
{
    public class EdgeCollection<TEdge> where
        TEdge : IEdge,
        new()
    {
        public List<TEdge> Edges { get; set; } = new List<TEdge>();
        public void AddEdge(INode n0, INode n1)
        {

            TEdge e = new TEdge();
            e.N0 = n0;
            e.N1 = n1;
            e = EnsureEdge(e, Constants.Epsilon);
            Edges.Add(e);
        }
        public TEdge EnsureEdge(TEdge e, double tol)
        {
            var fe = FindEdge(e, Constants.Epsilon);
            if (fe != null) return fe;
            fe = new TEdge();
            fe.N0 = e.N0;
            fe.N1 = e.N1;
            Edges.Add(fe);
            return fe;
        }
        public TEdge? FindEdge(TEdge e0, double tol)
        {
            Line3 l = new Line3(e0.N0.Position, e0.N1.Position);
            foreach (TEdge e1 in Edges)
            {
                if (l.IsEqualTo(new Line3(e1.N0.Position, e1.N1.Position), tol)) return e1;
                else if (e0.N0 == e1.N0 && e0.N1 == e1.N1 || e0.N0 == e1.N1 && e0.N1 == e1.N0) return e1;
            }
            return default;
        }

    }
}
