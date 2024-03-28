using BettaLib.Geometry;
using BettaLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BettaLib.FEAStructure
{
    public class LoadNodal : Load
    {
        public INode NodeAppliedOn { get; set; }

        public LoadNodal(INode position, double fx, double fy, double fz, double mx, double my, double mz, Node node)
            : base(fx, fy, fz, mx, my, mz)
        {
            NodeAppliedOn = position;
        }

        public LoadNodal(INode position, Vector3 force, Vector3 moment)
            : base(force, moment)
        {
            NodeAppliedOn = position;
        }
    }
}
