﻿using BettaLib.FEAStructure;
using BettaLib.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BettaLib.Utils
{
    public interface INode
    {

        Point3 Position { get; set; }
        int Id { get; set; }
        Support Support { get; set; }

    }
}
