using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BettaLib.Utils
{
    public interface IPixel
    {
        INode2 N0 { get; set; }
        INode2 N1 { get; set; }
        INode2 N2 { get; set; }
        INode2 N3 { get; set; }
    }
}
