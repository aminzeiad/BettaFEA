using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BettaLib.Utils
{
    public interface IEdge
    {
        INode N0 { get; set; }
        INode N1 { get; set; }
    }
}
