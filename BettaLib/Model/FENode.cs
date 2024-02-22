using BettaLib.Elements;
using BettaLib.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BettaLib.Model
{
    public class FENode
    {
        public Point3 Position;
        public object? Origin;

        public FENode(Point3 position, object? origin = null)
        {
            Position = position;
            Origin = origin;
        }
    }
}
