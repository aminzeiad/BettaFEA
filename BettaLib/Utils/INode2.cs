using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BettaLib.FEAStructurePix;
using BettaLib.Geometry;
using BettaLib.FEAStructure;

namespace BettaLib.Utils
{
    public interface INode2
    {
        Point2 Position { get; set; }
            int Id { get; set; }
    }
}
