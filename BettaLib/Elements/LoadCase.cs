using BettaLib.Elements;
using BettaLib.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BettaLib.Elements
{
    public enum LoadType
    {
        Dead,
        Live,
        Wind,
        Snow,
        Seismic
    }
    public class LoadCase
    {
        public String Name { get; set; }
        public LoadType Type { get; set; }
        List<Load> Loads { get; set; } = new List<Load>();
        List<Support> Supports { get; set; } = new List<Support>();

        public LoadCase(String name) => Name = name;
        public LoadCase(String name, LoadType type) => (Name, Type) = (name, type);

        public void AddPointLoad(double fx, double fy, double fz, double mx, double my, double mz, Node node)
        {
            Loads.Add(new LoadNodal(fx, fy, fz, mx, my, mz, node));
        }

        public void AddPointLoad(Node node, Vector3 force, Vector3 moment)
        {
            Loads.Add(new LoadNodal(node, force, moment));
        }

        public void AddSupport(Node node, Constraints constraints)
        {
            node.Support = new Support(constraints);
            Supports.Add(node.Support);
        }

        //ToString
        public override string ToString()
        {
            return $"Load Case: {Name} with {Loads.Count} Loads and {Supports.Count} Supports";
        }
    }
}
