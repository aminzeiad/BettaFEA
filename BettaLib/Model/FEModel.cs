using BettaLib.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BettaLib.Model
{
    public class FEModel
    {
        public FEModel(Structure structure, LoadCase loadcase)
        {
            Structure = structure;
            LoadCase = loadcase;
        }
        
        public Structure Structure { get; set; }//this structure should be immutable
        public LoadCase LoadCase { get; set; } //this loadcase should be immutable
        public List<FEElement> Elements { get; set; } = new List<FEElement>();
        public List<FENode> Nodes { get; set; } = new List<FENode>();

        public void PerformAnalysis()
        {
            //Perform analysis
        }
    }
}
