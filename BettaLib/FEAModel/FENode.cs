using BettaLib.FEAStructure;
using BettaLib.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BettaLib.Utils;
using MathNet.Numerics.LinearAlgebra;

namespace BettaLib.FEAModel
{
    public class FENode : INode
    {
        public Point3 Position { get; set; }
        public object? Origin;
        public Vector3 Force { get; set; } = new Vector3();
        public Vector3 Moment { get; set; } = new Vector3();
        public Vector3 Displacement { get; set; } = new Vector3();
        //a boolean that indicates if this a master node or a slave node created beacuase of a support
        //why? Becasue we want to create a spring between the master node and the slave node and we want to know which is which
        public bool IsSupportNode { get; set; } = false; //a node defined because of a support

        public Matrix<double> Deflections; //deflections in x, y, z, rx, ry, rz

        public int Id { get; set; }

        public FENode(Point3 position, object? origin = null)
        {
            Position = position;
            Origin = origin;
        }

        public void ApplyLoad(Vector3 force, Vector3 moment)
        {
            //augment the force and moment in case there is already a load applied
            Force += force;
            Moment += moment;
        }

        internal void FillInLoad(Vector<double> r)
        {
            r[Id * 6] = Force.X;
            r[Id * 6 + 1] = Force.Y;
            r[Id * 6 + 2] = Force.Z;
            r[Id * 6 + 3] = Moment.X;
            r[Id * 6 + 4] = Moment.Y;
            r[Id * 6 + 5] = Moment.Z;
        }

        public FENode() { }

        public String PrintForceVector()
        {
            return $"Node {Id} has a force of {Force}";
        }

        public String PrintMomentVector()
        {
            return $"Node {Id} has a moment of {Moment}";
        }

        public String PrintDisplacementVector()
        {
            return $"Node {Id} has a displacement of {Displacement}";
        }

        public String PrintDeflections()
        {
            return $"Node {Id} has deflections of {Deflections}";
        }

        public override string ToString()
        {
            return $"Node {Id} at {Position}";
        }
    }
}
