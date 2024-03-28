using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BettaLib.FEAStructure
{
    public class CrossSection
    {
        public string Name { get; set; }
        public double Area { get; set; }
        public double Ixx { get; set; }
        public double Iyy { get; set; }
        public double Izz { get; set; }
        public double J => Ixx + Iyy;

        public Material Material { get; set; }

        public CrossSection(string name, double area, double ixx, double iyy, double izz, Material material)
        {
            Name = name;
            Area = area;
            Ixx = ixx;
            Iyy = iyy;
            Izz = izz;
            Material = material;
        }

        public static CrossSection MakeRectangular(string name, double width, double height, Material material)
        {
            double area = width * height;
            double ixx = width * Math.Pow(height, 3) / 12;
            double iyy = height * Math.Pow(width, 3) / 12;
            double izz = ixx + iyy;
            return new CrossSection(name, area, ixx, iyy, izz, material);
        }

        public static CrossSection MakeCircular(string name, double diameter, Material material)
        {
            double area = Math.PI * Math.Pow(diameter, 2) / 4;
            double ixx = Math.PI * Math.Pow(diameter, 4) / 64;
            double iyy = ixx;
            double izz = ixx + iyy;
            return new CrossSection(name, area, ixx, iyy, izz, material);
        }

        public static CrossSection MakeIBeam(string name, double width, double height, double flangeWidth, double flangeThickness, Material material)
        {
            double area = width * height - (width - flangeWidth) * (height - flangeThickness);
            double ixx = (width * Math.Pow(height, 3) - (width - flangeWidth) * Math.Pow(height - flangeThickness, 3)) / 12;
            double iyy = (height * Math.Pow(width, 3) - (height - flangeThickness) * Math.Pow(width - flangeWidth, 3)) / 12;
            double izz = ixx + iyy;
            return new CrossSection(name, area, ixx, iyy, izz, material);
        }

        public static CrossSection MakeCircularHollow(string name, double outerDiameter, double innerDiameter, double thickness, Material material)
        {
            double area = Math.PI * (Math.Pow(outerDiameter, 2) - Math.Pow(innerDiameter, 2)) / 4;
            double ixx = Math.PI * (Math.Pow(outerDiameter, 4) - Math.Pow(innerDiameter, 4)) / 64;
            double iyy = ixx;
            double izz = ixx + iyy;
            return new CrossSection(name, area, ixx, iyy, izz, material);
        }

        public static CrossSection MakeRectangularHollow(string name, double outerWidth, double outerHeight, double innerWidth, double innerHeight, Material material)
        {
            double area = outerWidth * outerHeight - innerWidth * innerHeight;
            double ixx = (outerWidth * Math.Pow(outerHeight, 3) - innerWidth * Math.Pow(innerHeight, 3)) / 12;
            double iyy = (outerHeight * Math.Pow(outerWidth, 3) - innerHeight * Math.Pow(innerWidth, 3)) / 12;
            double izz = ixx + iyy;
            return new CrossSection(name, area, ixx, iyy, izz, material);
        }

        public override string ToString()
        {
            return $"CrossSection {Name}" + "\n" +
                $"Area: {Area}" + "\n" +
                $"Ixx: {Ixx}" + "\n" +
                $"Iyy: {Iyy}" + "\n" +
                $"Izz: {Izz}" + "\n" +
                $"Material: {Material.Name}";
        }
    }
}