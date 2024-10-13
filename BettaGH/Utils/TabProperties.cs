using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BettaGH.Utils
{
    public class TabProperties : GH_AssemblyPriority
    {
        public override GH_LoadingInstruction PriorityLoad()
        {
            var server = Grasshopper.Instances.ComponentServer;

            server.AddCategoryShortName("Betta", "B");
            server.AddCategorySymbolName("Betta", 'B');

            using (var ms = new MemoryStream(Properties.Resources.icon_betta))
            {
                var icon = new Bitmap(ms);
                server.AddCategoryIcon("Betta", icon);
            }

            return GH_LoadingInstruction.Proceed;
        }
    }
}
