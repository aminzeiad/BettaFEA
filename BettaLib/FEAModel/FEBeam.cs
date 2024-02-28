using BettaLib.FEAStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace BettaLib.FEAModel
{
    public class FEBeam : FEElement
    {
        public FEBeam(FENode node1, FENode node2, Beam? Origin = null) : base(2)
        {
            Node1 = node1;
            Node2 = node2;
            this.Origin = Origin;
        }

        public FENode Node1
        {
            get { return Nodes[0]; }
            set { Nodes[0] = value; }
        }
        public FENode Node2
        {
            get { return Nodes[1]; }
            set { Nodes[1] = value; }
        }
        public Beam? Origin { get; set; }


    }
}
