using BettaLib.FEAStructure;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using BettaLib.Utils;

namespace BettaLib.FEAModel
{
    public class FEBeam : IEdge
    {
        public Matrix<double> LocalStiffnessMatrix;
        public Matrix<double> TransformationMatrix;
        public Matrix<double> LocalEquivalentLoad;
        public Matrix<double> GlobalStiffnessMatrix;
        const int DOF = 6;

        public INode N0 { get; set; }

        public INode N1 { get; set; }

        public Beam? Origin { get; set; }

        public FEBeam(FENode node1, FENode node2, Beam? Origin = null) 
        {
            N0 = node1;
            N1 = node2;
            this.Origin = Origin;
        }

        public FEBeam(){ }   

        internal void CalculateLocalStiffnessMatrix()
        {
            throw new NotImplementedException();
        }

        internal void CalculateGlobalStiffnessMatrix()
        {
            //T' * k * T
            GlobalStiffnessMatrix = TransformationMatrix.Transpose() * LocalStiffnessMatrix * TransformationMatrix;
        }

        internal void CalculateTransformationMatrix(int numNodes)
        {
            int size = numNodes * DOF;
            Matrix<double> T = Matrix<double>.Build.Sparse(size, size);

            double[,] lambda = new double[3, 3];
            /*
            lambda[0, 0] = vxx;
            lambda[0,1] = vxy;
            lambda[0,2] = vxz;
            lambda[1,0] = vyx;
            lambda[1,1] = vyy;
            lambda[1,2] = vyz;
            lambda[2,0] = vzx;
            lambda[2,1] = vzy;
            lambda[2,2] = vzz;

            T[0, 0] = Lambda[0, 0];
            T[0, 1] = Lambda[0, 1];
            T[0, 2] = Lambda[0, 2];
            T[1, 0] = Lambda[1, 0];
            T[1, 1] = Lambda[1, 1];
            T[1, 2] = Lambda[1, 2];
            T[2, 0] = Lambda[2, 0];
            T[2, 1] = Lambda[2, 1];
            T[2, 2] = Lambda[2, 2];

            T[3, 3] = Lambda[0, 0];
            T[3, 4] = Lambda[0, 1];
            T[3, 5] = Lambda[0, 2];
            T[4, 3] = Lambda[1, 0];
            T[4, 4] = Lambda[1, 1];
            T[4, 5] = Lambda[1, 2];
            T[5, 3] = Lambda[2, 0];
            T[5, 4] = Lambda[2, 1];
            T[5, 5] = Lambda[2, 2];

            T[6, 6] = Lambda[0, 0];
            T[6, 7] = Lambda[0, 1];
            T[6, 8] = Lambda[0, 2];
            T[7, 6] = Lambda[1, 0];
            T[7, 7] = Lambda[1, 1];
            T[7, 8] = Lambda[1, 2];
            T[8, 6] = Lambda[2, 0];
            T[8, 7] = Lambda[2, 1];
            T[8, 8] = Lambda[2, 2];

            T[9, 9] = Lambda[0, 0];
            T[9, 10] = Lambda[0, 1];
            T[9, 11] = Lambda[0, 2];
            T[10, 9] = Lambda[1, 0];
            T[10, 10] = Lambda[1, 1];
            T[10, 11] = Lambda[1, 2];
            T[11, 9] = Lambda[2, 0];
            T[11, 10] = Lambda[2, 1];
            T[11, 11] = Lambda[2, 2];

            TransformationMatrix = T;
            */
        }

        internal void InitializeLocalEquivalentLoad()
        {
            LocalEquivalentLoad = Matrix<double>.Build.Sparse(DOF, 1);
        }
    }
}
