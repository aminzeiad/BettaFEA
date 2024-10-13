using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BettaLib.Geometry
{
    public class Line2
    {
        public Point2 Start { get; set; }
        public Point2 End { get; set; }

        public Line2(Point2 start, Point2 end)
        {
            Start = start;
            End = end;
        }

        public double Length
        {
            get
            {
                return Math.Sqrt(Math.Pow(End.X - Start.X, 2) + Math.Pow(End.Y - Start.Y, 2));
            }
        }

        public double Slope
        {
            get
            {
                return (End.Y - Start.Y) / (End.X - Start.X);
            }
        }

        public double YIntercept
        {
            get
            {
                return Start.Y - Slope * Start.X;
            }
        }

        public bool Contains(Point2 point)
        {
            return Math.Abs((End.Y - Start.Y) * point.X - (End.X - Start.X) * point.Y + End.X * Start.Y - End.Y * Start.X) < 0.0001;
        }

        public Point2 Intersection(Line2 other)
        {
            if (Math.Abs(Slope - other.Slope) < 0.0001)
            {
                return null;
            }
            double x = (other.YIntercept - YIntercept) / (Slope - other.Slope);
            double y = Slope * x + YIntercept;
            return new Point2(x, y);
        }

        public override string ToString()
        {
            return $"({Start}, {End})";
        }
    }
}
