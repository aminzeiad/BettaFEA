using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BettaLib.Geometry
{
    public class Point2
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Point2(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Point2(Point2 other)
        {
            X = other.X;
            Y = other.Y;
        }

        public Point2(Vector2 vector)
        {
            X = vector.X;
            Y = vector.Y;
        }
        //Methods
        public Vector2 VectorTo(Point2 other)
        {
            return new Vector2(other.X - X, other.Y - Y);
        }
        

        public double DistanceTo(Point2 other) => Math.Sqrt(DistanceToSquared(other));

        public double DistanceToSquared(Point2 other) => (X - other.X) * (X - other.X) + (Y - other.Y) * (Y - other.Y);

        public bool Equals(Point2 other) => X == other.X && Y == other.Y;

        public bool Equals(object obj) => obj is Point2 other && Equals(other);

        public bool IsEqualTo(Point2 other, double tolerance) => Math.Abs(X - other.X) < tolerance && Math.Abs(Y - other.Y) < tolerance;

        public override int GetHashCode() => HashCode.Combine(X, Y);

        public static bool operator ==(Point2 a, Point2 b) => a.Equals(b);
        
        public static bool operator !=(Point2 a, Point2 b) => !a.Equals(b);

        public static Point2 operator +(Point2 a, Vector2 b) => new Point2(a.X + b.X, a.Y + b.Y);

        public static Vector2 operator -(Point2 a, Point2 b) => new Vector2(a.X - b.X, a.Y - b.Y);

        public static Point2 operator -(Point2 a, Vector2 b) => new Point2(a.X - b.X, a.Y - b.Y);

        public static Point2 operator *(Point2 a, double b) => new Point2(a.X * b, a.Y * b);

        public static Point2 operator /(Point2 a, double b) => new Point2(a.X / b, a.Y / b);


        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}
