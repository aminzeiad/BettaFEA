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
        public CrossSection CrossSection { get; set; }
        public Point3 CG => (N0.Position + N1.Position) / 2.0;

        public INode N0 { get; set; }
        public INode N1 { get; set; }

        public Vector3 UpVector { get; set; } = Vector3.UnitZ;

        // Properties defined as part of the IEdge interface


        // Public property to access the length of the beam
        public double Length => N0.Position.DistanceTo(N1.Position);

        // Constructor with optional up vector parameter
        public Beam(INode n0, INode n1, CrossSection cs, Vector3 up )
        {
            N0 = n0;
            N1 = n1;
            CrossSection = cs;
            UpVector = up;
        }

        public Beam(INode n0, INode n1, CrossSection cs)
        {
            N0 = n0;
            N1 = n1;
            CrossSection = cs;
        }

        public Beam()
        {
            N0 = new Node(); // Assuming a default constructor for Node
            N1 = new Node(); // Assuming a default constructor for Node
            CrossSection = new CrossSection(); // Assuming a default constructor for CrossSection
            //Vzz = Vector3.UnitZ; // Default up vector
            //RefreshCoordinates(Vzz);
        }

        public override string ToString()
        {
            return $"Beam from {N0} to {N1}\n" +
                $"CrossSection: {CrossSection}\n" +
                $"Length: {Length}\n" +
                $"CG: {CG}\n" +
                $"Up: {UpVector}\n";
        }
    }
}
