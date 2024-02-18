using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace BettaGH
{
    public class BettaGHInfo : GH_AssemblyInfo
    {
        public override string Name => "Betta";

        //Return a 24x24 pixel bitmap to represent this GHA library.
        public override Bitmap Icon => null;

        //Return a short string describing the purpose of this GHA library.
        public override string Description => "";

        public override Guid Id => new Guid("8b7950f3-c4ba-4dd0-b23e-5acfa20dde7e");

        //Return a string identifying you or your company.
        public override string AuthorName => "";

        //Return a string representing your preferred contact details.
        public override string AuthorContact => "";
    }
}