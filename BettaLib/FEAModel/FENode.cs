using BettaLib.FEAStructure;
using BettaLib.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BettaLib.Utils;

namespace BettaLib.FEAModel
{
    public class FENode : INode
    {
        public Point3 Position { get; set; }
        public object? Origin;
        //a boolean that indicates if this a master node or a slave node created beacuase of a support
        //why? Becasue we want to create a spring between the master node and the slave node and we want to know which is which
        public bool IsSupportNode { get; set; } = false; //a node defined because of a support

        public double[,] Deflections = new double[6, 1]; //deflections in x, y, z, rx, ry, rz

        public FENode(Point3 position, object? origin = null)
        {
            Position = position;
            Origin = origin;
        }

        public FENode() { }
    }
}
