using BettaLib.Geometry;
using BettaLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BettaLib.FEAStructure
{
    public class Beam : IEdge
    {
        // Fields
        public INode N0 { get; set; }
        public INode N1 { get; set; }
        public CrossSection CrossSection { get; set; }
        public Point3 CG => (N0.Position + N1.Position) / 2.0;

        protected Vector3 _vx;
        public Vector3 vx => _vx;

        protected Vector3 _vy;
        public Vector3 vy => _vy;

        protected Vector3 _vz;
        public Vector3 vz => _vz;

        public double Length => (N1.Position - N0.Position).Length;



        // Constructors
        public Beam(Node n0, Node n1, CrossSection cs)
        {
            N0 = n0;
            N1 = n1;
            CrossSection = cs;
            Refresh();
        }

        public Beam() { }


        //Methods
        private void Refresh()
        {
            _vx = N1.Position - N0.Position;
            _vx.Normalize();

            // if (Math.Abs(_vx.Z) >= 1-Constants.Epsilon) ;

            if (Vector3.IsParallelTo(_vx, Vector3.UnitZ, Constants.Epsilon))
            {
                _vy = Vector3.UnitY;
                _vz = Vector3.Cross(_vx, _vy);
                _vz.Normalize();
            }
            else
            {
                _vy = Vector3.Cross(Vector3.UnitZ, _vx);
                _vy.Normalize();
                _vz = Vector3.Cross(_vx, _vy);
                _vz.Normalize();
            }

        }
        public static bool Equals(Beam b1, Beam b2)
        {
            return b1.N0 == b2.N0 && b1.N1 == b2.N1 || b1.N0 == b2.N1 && b1.N1 == b2.N0;
        }

        //ToString
        public override string ToString()
        {
            return $"Beam from {N0} \nto {N1}" + "\n" +
                $"CrossSection: {CrossSection}" + "\n" +
                $"Length: {Length}" + "\n" +
                $"CG: {CG}" + "\n" +
                $"vx: {vx}" + "\n" +
                $"vy: {vy}" + "\n" +
                $"vz: {vz}";
        }

    }
}
