﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BettaLib.Model
{
    public class FEElement
    {
        public FENode[] Nodes { get; set; }
        public FEElement(int numNodes)
        {
            Nodes = new FENode[numNodes];
        }
    }
}
