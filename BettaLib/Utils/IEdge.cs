using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BettaLib.Geometry;

namespace BettaLib.Utils
{
    public interface IEdge
    {
        INode N0 { get; set; }
        INode N1 { get; set; }

    }

    public abstract class EdgeBase : IEdge
    {
        public INode N0 { get; set; }
        public INode N1 { get; set; }

        public Vector3 Vxx, Vyy, Vzz;

        public void RefreshCoordinates(Vector3 up = default(Vector3))
        {
            Vxx = N1.Position - N0.Position;
            Vxx.Normalize();

            bool notPerpendicular = !Vector3.IsPerpendicularTo(Vxx, up, Constants.Epsilon);
            if (up == default(Vector3) || notPerpendicular)
            {
                if (Vector3.IsParallelTo(Vxx, Vector3.UnitZ, Constants.Epsilon))
                {
                    Vyy = Vector3.UnitY;
                    Vzz = Vector3.Cross(Vxx, Vyy);
                    Vzz.Normalize();
                }
                else
                {
                    Vyy = Vector3.Cross(Vector3.UnitZ, Vxx);
                    Vyy.Normalize();
                    Vzz = Vector3.Cross(Vxx, Vyy);
                    Vzz.Normalize();
                }
            }
            else
            {
                Vzz = up;
                Vzz.Normalize();
                Vyy = Vector3.Cross(Vxx, Vzz);
            }
        }
    }
}
