using BettaLib.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BettaLib.FEAStructure
{
    public class LoadNodal : Load
    {
        public Node Node { get; set; }

        public LoadNodal(double fx, double fy, double fz, double mx, double my, double mz, Node node)
            : base(fx, fy, fz, mx, my, mz)
        {
            Node = node;
        }

        public LoadNodal(Node node, Vector3 force, Vector3 moment)
            : base(force, moment)
        {
            Node = node;
        }
    }
}
