using BettaLib.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BettaLib.Elements
{
    [Flags]
    public enum Constraints
    {
        None = 0,
        Ux = 1 << 0,
        Uy = 1 << 1,
        Uz = 1 << 2,
        Rxx = 1 << 3,
        Ryy = 1 << 4,
        Rzz = 1 << 5,
        Translations = Ux | Uy | Uz,
        Rotations = Rxx | Ryy | Rzz,
        All = Translations | Rotations
    }


    public class Support
    {
        public bool Ux { get; set; }
        public bool Uy { get; set; }
        public bool Uz { get; set; }
        public bool Rxx { get; set; }
        public bool Ryy { get; set; }
        public bool Rzz { get; set; }
        private double KUx { get; set; }
        private double KUy { get; set; }
        private double KUz { get; set; }
        private double KRxx { get; set; }
        private double KRyy { get; set; }
        private double KRzz { get; set; }

        public Support(Constraints releaseType)
        {
            SetReleaseType(releaseType);
            if (Ux) KUx = Constants.Rigid; else KUx = Constants.Free;
            if (Uy) KUy = Constants.Rigid; else KUy = Constants.Free;
            if (Uz) KUz = Constants.Rigid; else KUz = Constants.Free;
            if (Rxx) KRxx = Constants.Rigid; else KRxx = Constants.Free;
            if (Ryy) KRyy = Constants.Rigid; else KRyy = Constants.Free;
            if (Rzz) KRzz = Constants.Rigid; else KRzz = Constants.Free;
        }

        private void SetReleaseType(Constraints releaseType)
        {
            Ux = releaseType.HasFlag(Constraints.Ux);
            Uy = releaseType.HasFlag(Constraints.Uy);
            Uz = releaseType.HasFlag(Constraints.Uz);
            Rxx = releaseType.HasFlag(Constraints.Rxx);
            Ryy = releaseType.HasFlag(Constraints.Ryy);
            Rzz = releaseType.HasFlag(Constraints.Rzz);
        }

        //toString
        public override string ToString()
        {
            return $"({Ux}, {Uy}, {Uz}, {Rxx}, {Ryy}, {Rzz})";
        }
    }
}
