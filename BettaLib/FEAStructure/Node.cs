using BettaLib.Geometry;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BettaLib.FEAStructure
{
    public class Node
    {
        //Fields
        public Support? Support { get; set; } = new Support(Constraints.None);
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public Point3 Position { get => new Point3(X, Y, Z); }

        //Constructors
        public Node(Point3 position) { X = position.X; Y = position.Y; Z = position.Z; }
        public Node(double x, double y, double z) { X = x; Y = y; Z = z; }

        //Methods
        public static bool Equals(Node n1, Node n2) => n1.X == n2.X && n1.Y == n2.Y && n1.Z == n2.Z; //Do I need to consider rounding errors?

        //ToString
        public override string ToString()
        {
            return $"Node {Position}" + " " + $"with Boundary Conditions {Support}";
        }
    }
}
