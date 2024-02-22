using BettaLib.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BettaLib.Elements
{
        public class Load
        {
        protected Vector3 _force;
        public Vector3 Force => _force;

        protected Vector3 _moment;
        public Vector3 Moment => _moment;
            public double Fx { get; }
            public double Fy { get; }
            public double Fz { get; }
            public double Mx { get; }
            public double My { get; }
            public double Mz { get; }


            public Load(double fx, double fy, double fz, double mx, double my, double mz)
            {
                Fx = fx;
                Fy = fy;
                Fz = fz;
                Mx = mx;
                My = my;
                Mz = mz;
                _force = new Vector3(fx, fy, fz);
                _moment = new Vector3(mx, my, mz);
            }

            public Load(Vector3 force, Vector3 moment)
            {
                _force = force;
                _moment = moment;
                Fx = force.X;
                Fy = force.Y;
                Fz = force.Z;
                Mx = moment.X;
                My = moment.Y;
                Mz = moment.Z;
            }
        }
}
