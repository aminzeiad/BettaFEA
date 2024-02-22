using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BettaLib.Geometry
{
    public struct Line3 : IEquatable<Line3>
    {
        // Fields
        public Point3 Start;
        public Point3 End;

        // Constructors
        public Line3(Point3 start, Vector3 direction, double length)
        {
            Start = start;
            End = start + direction * length;
        }
        public Line3(double x1, double y1, double z1, double x2, double y2, double z2)
        {
            Start = new Point3(x1, y1, z1);
            End = new Point3(x2, y2, z2);
        }
        public Line3(Point3 start, Point3 end)
        {
            Start = start;
            End = end;
        }

        public Line3(Line3 l)
        {
            Start = l.Start;
            End = l.End;
        }
        //Methods
        public Point3 From => Start;
        public Point3 To => End;
        public Point3 PointAt(double t) => Start + (End - Start) * t;
        public Vector3 Direction => End - Start;
        public double Length => Start.DistanceTo(End);
        public double LengthSquared => Start.DistanceToSquared(End);
        public Point3 MidPoint => (Start + End) / 2;
        public bool IsEqualTo(Line3 l, double tollerance) => Start.IsEqualTo(l.Start, tollerance) && End.IsEqualTo(l.End, tollerance);
        public override bool Equals(object obj) => obj is Line3 l && Equals(l);
        public bool Equals(Line3 l) => Start == l.Start && End == l.End || Start == l.End && End == l.Start;
        public override int GetHashCode() => HashCode.Combine(Start, End);

        public bool IsOnLine(Point3 p, double tollerance)
        {
            double length = Start.DistanceTo(p) + p.DistanceTo(End);
            return length - Length <= tollerance;
        }
        public bool IsVertex(Point3 p, double tollerance)
        {
            return Start.DistanceTo(p) <= tollerance || End.DistanceTo(p) <= tollerance;
        }

        public static (bool success, double t1, double t2, Point3 p1, Point3 p2) Intersect(Line3 l1, Line3 l2, double tollerance)
        {
            //bool isOnLines = false;

            (bool success, double t1, double t2, Point3 p1, Point3 p2) result = (false, 0, 0, new Point3(0, 0, 0), new Point3(0, 0, 0));

           Vector3 a = new Vector3(l1.From);
            Vector3 b = new Vector3(l1.To);
            Vector3 c = new Vector3(l2.From);
            Vector3 d = new Vector3(l2.To);

            Vector3 r = b - a;
            Vector3 s = d - c;
            Vector3 q = a - c;

            double dotqr = Vector3.Dot(q, r);
            double dotqs = Vector3.Dot(q, s);
            double dotrs = Vector3.Dot(r, s);
            double dotrr = Vector3.Dot(r, r);
            double dotss = Vector3.Dot(s, s);

            double denom = dotrr * dotss - dotrs * dotrs;
            double numer = dotqs * dotrs - dotqr * dotss;

            result.t1 = numer / denom;

            if (result.t1 < 0.0 || result.t1 > 1.0) return result;
            result.t2 = (dotqs + result.t1 * dotrs) / dotss;

            if (result.t2 < 0.0 || result.t2 > 1.0) return result;

            result.p1 = l1.PointAt(result.t1);
            result.p2 = l2.PointAt(result.t2);

            result.success = ((result.p1 - result.p2).LengthSquared <= tollerance * tollerance);

            return result;
        }

        public static bool IsParallelTo(Line3 l1, Line3 l2, double tollerance)
        {
            Vector3 v1 = l1.Direction;
            Vector3 v2 = l2.Direction;
            return Vector3.IsParallelTo(v1, v2, tollerance);
        }
        public static bool IsPerpendicularTo(Line3 l1, Line3 l2, double tollerance)
        {
            Vector3 v1 = l1.Direction;
            Vector3 v2 = l2.Direction;
            return Vector3.IsPerpendicularTo(v1, v2, tollerance);
        }

        //boolean operators
        public static bool operator ==(Line3 l1, Line3 l2) => l1.Equals(l2);
        public static bool operator !=(Line3 l1, Line3 l2) => !l1.Equals(l2);
        //toString
        public override string ToString() => $"({Start}, {End})";
    }
}
