using BettaLib.Geometry;
using BettaLib.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BettaLib.Structure
{
    public class Beam
    {
        // Fields
        public Node Node1 { get; set; }
        public Node Node2 { get; set; }
        public CrossSection CrossSection { get; set; }
        public Point3 CG => (Node1.Position + Node2.Position) / 2.0;

        protected Vector3 _vx;
        public Vector3 vx => _vx;

        protected Vector3 _vy;
        public Vector3 vy => _vy;

        protected Vector3 _vz;
        public Vector3 vz => _vz;

        public double Length => (Node2.Position - Node1.Position).Length;



        // Constructors
        public Beam(Node node1, Node node2, CrossSection cs)
        {
            Node1 = node1;
            Node2 = node2;
            CrossSection = cs;
            Refresh();
        }


        //Methods
        private void Refresh()
        {
            _vx = Node2.Position - Node1.Position;
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
            return b1.Node1 == b2.Node1 && b1.Node2 == b2.Node2 || b1.Node1 == b2.Node2 && b1.Node2 == b2.Node1;
        }

        //ToString
        public override string ToString()
        {
            return $"Beam from {Node1} \nto {Node2}" + "\n" +
                $"CrossSection: {CrossSection}" + "\n" +
                $"Length: {Length}" + "\n" +
                $"CG: {CG}" + "\n" +
                $"vx: {vx}" + "\n" +
                $"vy: {vy}" + "\n" +
                $"vz: {vz}";
        }

    }
}
