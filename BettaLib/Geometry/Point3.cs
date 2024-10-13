using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BettaLib.Geometry
{
    public struct Point3 : IEquatable<Point3>
    {
        // Fields
        public double X;
        public double Y;
        public double Z;

        // Constructors
        public Point3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Point3(Point3 p)
        {
            X = p.X;
            Y = p.Y;
            Z = p.Z;
        }

        public Point3(Vector3 v)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
        }

        //Methods
        public readonly Vector3 VectorTo(Point3 p) => new(p.X - X, p.Y - Y, p.Z - Z);
       
        public readonly double DistanceTo(Point3 p) => Math.Sqrt((X - p.X) * (X - p.X) + (Y - p.Y) * (Y - p.Y) + (Z - p.Z) * (Z - p.Z));
      
        public readonly double DistanceToSquared(Point3 p) => (X - p.X) * (X - p.X) + (Y - p.Y) * (Y - p.Y) + (Z - p.Z) * (Z - p.Z);
      
        public readonly bool IsEqualTo(Point3 p, double tolerance) => Math.Abs(X - p.X) < tolerance && Math.Abs(Y - p.Y) < tolerance && Math.Abs(Z - p.Z) < tolerance;
      
        public override readonly bool Equals(object? obj) => obj is Point3 p && Equals(p);
      
        public readonly bool Equals(Point3 p) => X == p.X && Y == p.Y && Z == p.Z;
       
        public override readonly int GetHashCode() => HashCode.Combine(X, Y, Z);
      
        //public static Point3 Origin => new(0, 0, 0);

        //boolean operators
        public static bool operator ==(Point3 p1, Point3 p2) => p1.Equals(p2);
        public static bool operator !=(Point3 p1, Point3 p2) => !p1.Equals(p2);

        //Pt3d Operators
        public static Point3 operator +(Point3 p, Vector3 v) { return new Point3(p.X + v.X, p.Y + v.Y, p.Z + v.Z); }
        public static Point3 operator +(Point3 p1, Point3 p2) { return new Point3(p1.X + p2.X, p1.Y + p2.Y, p1.Z + p2.Z); }
        public static Point3 operator -(Point3 p, Vector3 v) { return new Point3(p.X - v.X, p.Y - v.Y, p.Z - v.Z); }
        public static Vector3 operator -(Point3 p1, Point3 p2) { return new Vector3(p1.X - p2.X, p1.Y - p2.Y, p1.Z - p2.Z); }
        public static Point3 operator *(Point3 p, double s) { return new Point3(p.X * s, p.Y * s, p.Z * s); }
        public static Point3 operator *(double s, Point3 p) { return new Point3(p.X * s, p.Y * s, p.Z * s); }
        public static Point3 operator /(Point3 p, double s) { return new Point3(p.X / s, p.Y / s, p.Z / s); }
       
        //toString
        public override readonly string ToString() => $"({X}, {Y}, {Z})";


    }

}
