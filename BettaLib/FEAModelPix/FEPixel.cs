using BettaLib.FEAStructure;
using BettaLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BettaLib.FEAModelPix
{
    public struct FEPixel : IPixel
    {
        public INode2 N0 { get; set; }
        public INode2 N1 { get; set; }
        public INode2 N2 { get; set; }
        public INode2 N3 { get; set; }
        public double Density { get; set; }
        public double Sensitivity { get; set; }

        public FEPixel(INode2 n0, INode2 n1, INode2 n2, INode2 n3)
        {
            N0 = n0;
            N1 = n1;
            N2 = n2;
            N3 = n3;

        }

        public static double[,] BuildElementMatrix(Material material)
        {
            double E = material.ElasticModulus;
            double nu = material.PoissonRatio;

            double[] k = {  0.5-nu/6.0, (1.0+nu)/8.0, -0.25-nu/12.0, -(1.0-3.0*nu)/8.0,
                           -0.25+nu/12.0, -(1.0+nu)/8.0, nu/6.0, (1.0-3.0*nu)/8.0};

            double m = E / (1.0 - nu * nu);

            for (int i = 0; i < 8; ++i) k[i] *= m; //premultiply matrix elements with factor m

            double[,] KE = {   {k[0], k[1], k[2], k[3], k[4], k[5], k[6], k[7]},
                               {k[1], k[0], k[7], k[6], k[5], k[4], k[3], k[2]},
                               {k[2], k[7], k[0], k[5], k[6], k[3], k[4], k[1]},
                               {k[3], k[6], k[5], k[0], k[7], k[2], k[1], k[4]},
                               {k[4], k[5], k[6], k[7], k[0], k[1], k[2], k[3]},
                               {k[5], k[4], k[3], k[2], k[1], k[0], k[7], k[6]},
                               {k[6], k[3], k[4], k[1], k[2], k[7], k[0], k[5]},
                               {k[7], k[2], k[1], k[4], k[3], k[6], k[5], k[0]}};


            return KE;
        }
    }
}
