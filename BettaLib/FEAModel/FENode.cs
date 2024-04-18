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
    //
    public enum DOFID
    {
        DX = 0,
        DY = 1,
        DZ = 2,
        RX = 3,
        RY = 4,
        RZ = 5
    }

    //a flag enum that represents the support type of a node
    [Flags]
    public enum FENodeSupportType
    {
        None = 0,
        DX = 1 << DOFID.DX,
        DY = 1 << DOFID.DY,
        DZ = 1 << DOFID.DZ,
        RX = 1 << DOFID.RX,
        RY = 1 << DOFID.RY,
        RZ = 1 << DOFID.RZ
    }

    public class FENode : INode
    {
        public Point3 Position { get; set; }
        public object? Origin;
        public Vector3 Force { get; set; } = new Vector3();
        public Vector3 Moment { get; set; } = new Vector3();
        public Vector3 Displacement { get; set; } = new Vector3();
        public Support Support { get; set; } = new Support(Constraints.None);
        //a boolean that indicates if this a master node or a slave node created beacuase of a support
        //why? Becasue we want to create a spring between the master node and the slave node and we want to know which is which
        public bool IsSupportNode { get; set; } = false; //a node defined because of a support

        public FENodeSupportType SupportType { get; set; } = FENodeSupportType.None;

        public bool HasSupport(DOFID dof)
        {
            return SupportType.HasFlag((FENodeSupportType)(1 << (int)dof));
        }

        public int GetGlobalDOF(DOFID dof)
        {
            return Id * 6 + (int)dof;
        }

        public double ForceDOF(DOFID dof)
        {
            if (dof == DOFID.DX) return Force.X;
            if (dof == DOFID.DY) return Force.Y;
            if (dof == DOFID.DZ) return Force.Z;
            if (dof == DOFID.RX) return Moment.X;
            if (dof == DOFID.RY) return Moment.Y;
            if (dof == DOFID.RZ) return Moment.Z;
            throw new ArgumentException("Invalid DOF");
        }

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
