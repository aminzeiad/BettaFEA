using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using millipedeLibNET2;

namespace Millitest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var rs  = new StatModel();
            var n0 =  rs.AddNode(0, 0, 0, BOUNDARYCONDITIONS.ALL);
            var n1 = rs.AddNode(5, 0, 0);
            n1.AddLoad(0, 0, -1.0);

            var mat = new StatMaterial("mat")
            {
                Em = 1.0,
                Poisson = 0.2,
                Gm = 1.0
            };

            var cs = new StatCrossSection("cs", mat);
            cs.CircSolid(0.01);

            var beam = rs.AddBeam(n0, n1, cs);

            rs.SolveSystem();

           var K = rs.GetGlobalStiffnessMatrix();

            var k = new double[12 * 12];
            beam.BuildStiffnessMatrix(k);

        }
    }
}
