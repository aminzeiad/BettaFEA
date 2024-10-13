using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BettaLib.Geometry;
using BettaLib.FEAStructure;
using BettaLib.Utils;

namespace BettaLib.FEAStructurePix
{
    public class Node2 : INode2
    {
        public Support? Support { get; set; } = new Support(Constraints.None);
        public double X { get; set; }
        public double Y { get; set; }
        public int Id { get; set; }
        public Point2 Position { get => new Point2(X, Y); set { X = value.X; Y = value.Y;} }

        //Constructors
        public Node2(Point3 position) { X = position.X; Y = position.Y; }
        public Node2(double x, double y, double z) { X = x; Y = y; }
        public Node2()
        {
            X = 0;
            Y = 0;
        }

        //Methods
        public static bool Equals(Node2 n1, Node2 n2) => n1.X == n2.X && n1.Y == n2.Y; //Do I need to consider rounding errors?

        //ToString
        public override string ToString()
        {
            return $"Node {Position}" + " " + $"with Boundary Conditions {Support}";
        }
    }
}
