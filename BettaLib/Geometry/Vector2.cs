using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BettaLib.Geometry
{
    public class Vector2 : IEquatable<Vector2>
    {

        public double X { get; set; }
        public double Y { get; set; }

        public Vector2() { X = 0; Y = 0; }
        public Vector2(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Vector2(Vector2 other)
        {
            X = other.X;
            Y = other.Y;
        }

        public Vector2(Point2 point)
        {
            X = point.X;
            Y = point.Y;
        }

        //zero vector

        public double Length => Math.Sqrt(X * X + Y * Y);

        public double LengthSquared => X * X + Y * Y;

        public void Reverse() { X = -X; Y = -Y; }

        public bool IsZero(double tolerance) => Math.Abs(X) < tolerance && Math.Abs(Y) < tolerance;

        public bool IsEqualTo(Vector2 other, double tolerance) => Math.Abs(X - other.X) < tolerance && Math.Abs(Y - other.Y) < tolerance;

        public bool Equals(Vector2 other) => X == other.X && Y == other.Y;

        public override bool Equals(object? obj)
        {
            if (obj is Vector2 other)
            {
                return X == other.X && Y == other.Y;
            }
            return false;
        }

        public override int GetHashCode() => HashCode.Combine(X, Y);

        public Vector2 Normalize() => this / Length;

        public static Vector2 Zero => new(0, 0);
        public static double Dot(Vector2 a, Vector2 b) => a.X * b.X + a.Y * b.Y;
        public static double Cross(Vector2 a, Vector2 b) => a.X * b.Y - a.Y * b.X;
        public static double AngleBetween(Vector2 a, Vector2 b) => Math.Acos(Dot(a, b) / (a.Length * b.Length));
        public static bool IsParallelTo(Vector2 a, Vector2 b, double tollerance) => Math.Abs(Cross(a, b)) < tollerance;
        public static bool IsPerpendicularTo(Vector2 a, Vector2 b, double tollerance) => Math.Abs(Dot(a, b)) < tollerance;

        //public double Dot(Vector2 other) => X * other.X + Y * other.Y;

        //public double Cross(Vector2 other) => X * other.Y - Y * other.X;

        //public double Angle(Vector2 other) => Math.Acos(Dot(other) / (Length * other.Length));

        public Vector2 Project(Vector2 other) => Vector2.Dot(this, other) / other.LengthSquared * other;

        public static Vector2 operator +(Vector2 a, Vector2 b) => new(a.X + b.X, a.Y + b.Y);

        public static Vector2 operator -(Vector2 a, Vector2 b) => new(a.X - b.X, a.Y - b.Y);

        public static Vector2 operator *(Vector2 a, double b) => new(a.X * b, a.Y * b);

        public static Vector2 operator *(double a, Vector2 b) => new(a * b.X, a * b.Y);

        public static Vector2 operator /(Vector2 a, double b) => new(a.X / b, a.Y / b);

        public static Vector2 operator /(double a, Vector2 b) => new(a / b.X, a / b.Y);

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}
