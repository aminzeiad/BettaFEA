using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BettaLib.Geometry
{
    public struct Vector3 : IEquatable<Vector3>
    {
        // Fields
        public double X;
        public double Y;
        public double Z;

        // Constructors
        public Vector3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3(Vector3 v)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
        }

        public Vector3(Point3 p)
        {
            X = p.X;
            Y = p.Y;
            Z = p.Z;
        }

        //Methods
        public double Length => Math.Sqrt(X * X + Y * Y + Z * Z);
        
        public double LengthSquared => X * X + Y * Y + Z * Z;
        
        public void Reverse() { X = -X; Y = -Y; Z = -Z; }

        public override int GetHashCode() { return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode(); }
        
        public bool IsZero(double tollerance) { return Math.Abs(X) < tollerance && Math.Abs(Y) < tollerance && Math.Abs(Z) < tollerance; }
        
        public bool IsEqualTo(Vector3 v, double tollerance) { return Math.Abs(X - v.X) < tollerance && Math.Abs(Y - v.Y) < tollerance && Math.Abs(Z - v.Z) < tollerance; }
        
        public bool Equals(Vector3 other) { return X == other.X && Y == other.Y && Z == other.Z; }

        public override bool Equals(object obj)
        {
            if (obj is Vector3)
            {
                Vector3 v = (Vector3)obj;
                return X == v.X && Y == v.Y && Z == v.Z;
            }
            return false;
        }

        public bool Normalize()
        {
            double length = Length;
            if (length > 0)
            {
                X /= length;
                Y /= length;
                Z /= length;
                return true;
            }
            return false;
        }

        //Static Methods
        public static double Dot(Vector3 v1, Vector3 v2) { return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z; }

        public static Vector3 Cross(Vector3 v1, Vector3 v2) { return new Vector3(v1.Y * v2.Z - v1.Z * v2.Y, v1.Z * v2.X - v1.X * v2.Z, v1.X * v2.Y - v1.Y * v2.X); }
        public static double AngleBetween(Vector3 v1, Vector3 v2)
        {
            double dot = Dot(v1, v2);
            double length = v1.Length * v2.Length;
            if (length == 0) return 0;
            return Math.Acos(Math.Max(-1, Math.Min(1, dot / length)));
        }

        //comparison methods
        public static bool IsParallelTo(Vector3 v1, Vector3 v2, double tollerance)
        {
            double angle = AngleBetween(v1, v2);
            return angle < tollerance || angle > Math.PI - tollerance;
        }

        public static bool IsPerpendicularTo(Vector3 v1, Vector3 v2, double tollerance)
        {
            double angle = AngleBetween(v1, v2);
            return angle < tollerance || angle > Math.PI - tollerance;
        }

        //Vec3 Operators
        public static Vector3 operator +(Vector3 v1, Vector3 v2) { return new Vector3(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z); }
        public static Vector3 operator -(Vector3 v1, Vector3 v2) { return new Vector3(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z); }
        public static Vector3 operator *(Vector3 v, double s) { return new Vector3(v.X * s, v.Y * s, v.Z * s); }
        public static Vector3 operator *(double s, Vector3 v) { return new Vector3(v.X * s, v.Y * s, v.Z * s); }
        public static Vector3 operator *(Vector3 v1, Vector3 v2) { return new Vector3(v1.X * v2.X, v1.Y * v2.Y, v1.Z * v2.Z); }
        public static Vector3 operator /(Vector3 v, double s) { return new Vector3(v.X / s, v.Y / s, v.Z / s); }
        public static Vector3 operator -(Vector3 v) { return new Vector3(-v.X, -v.Y, -v.Z); }

        //Boolean operators
        public static bool operator ==(Vector3 v1, Vector3 v2) { return v1.X == v2.X && v1.Y == v2.Y && v1.Z == v2.Z; }
        public static bool operator !=(Vector3 v1, Vector3 v2) { return v1.X != v2.X || v1.Y != v2.Y || v1.Z != v2.Z; }

        //Constants
        public static Vector3 Zero => new(0, 0, 0);
        public static Vector3 UnitX => new(1, 0, 0);
        public static Vector3 UnitY => new(0, 1, 0);
        public static Vector3 UnitZ => new(0, 0, 1);

        //toString
        public override string ToString()
        {
            return "(" + X + ", " + Y + ", " + Z + ")";
        }

    }
}
