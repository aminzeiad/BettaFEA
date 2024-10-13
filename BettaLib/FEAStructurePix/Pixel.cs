using BettaLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BettaLib.FEAStructurePix
{
    public class Pixel : IPixel
    {
        public INode2 N0 { get; set; }
        public INode2 N1 { get; set; }
        public INode2 N2 { get; set; }
        public INode2 N3 { get; set; }

        public Pixel(INode2 n0, INode2 n1, INode2 n2, INode2 n3)
        {
            N0 = n0;
            N1 = n1;
            N2 = n2;
            N3 = n3;

        }
    }
}
