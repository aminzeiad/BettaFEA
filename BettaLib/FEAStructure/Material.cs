using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BettaLib.FEAStructure
{
    public class Material
    {
        public string Name { get; set; }
        public double ElasticModulus { get; set; }
        public double PoissonRatio { get; set; }
        public double Density { get; set; }
        public double YieldStrength { get; set; }
        public double ThermalExpansionCoefficient { get; set; }
        public double ShearModulus => ElasticModulus / (2 * (1 + PoissonRatio));

        public static Material MakeGeneric(string name)
        {
            return new Material(name, 210000, 0.3, 7850, 235, 0.000012);
        }

        public static Material MakeSteel(string name)
        {
            return new Material(name, 210000, 0.3, 7850, 235, 0.000012);
        }

        public static Material MakeConcrete(string name)
        {
            return new Material(name, 30000, 0.2, 2400, 20, 0.000010);
        }

        public Material(string name, double elasticModulus, double poissonRatio, double density, double yieldStrength, double thermalExpansionCoefficient)
        {
            Name = name;
            ElasticModulus = elasticModulus;
            PoissonRatio = poissonRatio;
            Density = density;
            YieldStrength = yieldStrength;
            ThermalExpansionCoefficient = thermalExpansionCoefficient;
        }

        public override string ToString()
        {
            return Name + "\n" +
                "E: " + ElasticModulus + " | " +
                "V: " + PoissonRatio + " | " +
                "p: " + Density + " | " +
                "σ: " + YieldStrength + " | " +
                "α: " + ThermalExpansionCoefficient;
        }
    }
}
