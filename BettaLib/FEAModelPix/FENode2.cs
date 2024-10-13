using BettaLib.FEAStructure;
using BettaLib.Geometry;
using BettaLib.Utils;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BettaLib.FEAModelPix
{
    //
    public enum DOFID
    {
        X = 0,
        Y = 1
    }

    //a flag enum that represents the support type of a node
    [Flags]
    public enum FENode2SupportType
    {
        None = 0,
        X = 1 << DOFID.X,
        Y = 1 << DOFID.Y,
        All = X | Y
    }

    public class FENode2 : INode2
    {
        public Point2 Position { get; set; }
        public object? Origin;
        public Vector2 F { get; set; } = new Vector2();
        public Vector2 U { get; set; } = new Vector2();
        public bool IsSupportNode { get; set; } = false; //a node defined because of a support

        public FENode2SupportType SupportType { get; set; } = FENode2SupportType.None;

        public bool HasSupport(DOFID dof)
        {
            return SupportType.HasFlag((FENode2SupportType)(1 << (int)dof));
        }

        public int GetGlobalDOF(DOFID dof)
        {
            return Id * 2 + (int)dof;
        }

        public double ForceDOF(DOFID dof)
        {
            if (dof == DOFID.X) return F.X;
            if (dof == DOFID.Y) return F.Y;
            throw new ArgumentException("Invalid DOF");
        }

        public Matrix<double> Deflections; //deflections in x, y, z, rx, ry, rz

        public int Id { get; set; }

        public FENode2(Point2 position, object? origin = null)
        {
            Position = position;
            Origin = origin;
            F = new Vector2();
            U = new Vector2();
        }
        public FENode2(double x, double y, object? origin = null)
        {
            Position = new Point2(x, y);
            Origin = origin;
            F = new Vector2();
            U = new Vector2();
        }
        public FENode2() {
            F = new Vector2();
            U = new Vector2();
        }

        public void ApplyLoad(Vector2 force)
        {
            //augment the force and moment in case there is already a load applied
            F += force;
        }

        internal void FillInLoad(Vector<double> r)
        {
            r[Id * 2] = F.X;
            r[Id * 2 + 1] = F.Y;
        }


        public String PrintForceVector()
        {
            return $"Node2 {Id} has a force of {F}";
        }


        public String PrintDeflections()
        {
            return $"Node2 {Id} has deflections of {Deflections}";
        }

        public override string ToString()
        {
            return $"Node2 {Id} at {Position}";
        }
    }
}
