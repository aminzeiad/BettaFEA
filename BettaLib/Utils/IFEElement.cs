using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BettaLib.Utils
{
    public interface IFEElement //to make it generalizable to other elements
    {
        void AssembleOnGlobalStiffnessMatrix(Matrix<double> K);
        int DOF { get; }
    }
}
