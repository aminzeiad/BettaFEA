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
    public interface IFEElement
    {
        void assembleOnGlobalStiffnessMatrix(Matrix<double> K);
        int DOF { get; }
    }
    public class FEBeam : IEdge, IFEElement
    {
        //public Matrix<double> LocalStiffnessMatrix;
        //public Matrix<double> TransformationMatrix;
        //public Matrix<double> GlobalElementalStiffnessMatrix;
        //public Matrix<double> GlobalStrcuturalStiffnessMatrix;
        public Matrix<double> LocalEquivalentNodalLoad;
        public int DOF => 12;

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

        protected Matrix<double> CalculateLocalStiffnessMatrix()
        {
            Matrix<double> ke = Matrix<double>.Build.Dense(DOF, DOF);


            return ke;
        }


        protected Matrix<double> CalculateTransformationMatrix()
        {
            
            Matrix<double> T = Matrix<double>.Build.Dense(DOF, DOF);

           
            double[,] Lambda = new double[3, 3];

            Lambda[0, 0] = Origin.vx.X; //may be null I am sure there is a better way to do this
            Lambda[0, 1] = Origin.vx.Y;
            Lambda[0, 2] = Origin.vx.Z;
            Lambda[1, 0] = Origin.vy.X;
            Lambda[1, 1] = Origin.vy.Y;
            Lambda[1, 2] = Origin.vy.Z;
            Lambda[2, 0] = Origin.vz.Z;
            Lambda[2, 1] = Origin.vz.Y;
            Lambda[2, 2] = Origin.vz.Z;

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

            return T;
            
        }

        protected Matrix<double> CalculateGlobalElementalStiffnessMatrix()
        {
            var T = CalculateTransformationMatrix();
            var k = CalculateLocalStiffnessMatrix();
            var kg = T.Transpose() * k * T;

            return kg;
        }

  

        public void InitializeLocalEquivalentLoad()
        {
            LocalEquivalentNodalLoad = Matrix<double>.Build.Sparse(DOF, 1);
        }

        public void AssembleOnGlobalStiffnessMatrix(Matrix<double> K)
        {
            var Ke = CalculateGlobalElementalStiffnessMatrix();

            int i0 = N0.Id * 6;
            int i1 = N1.Id * 6;

         
            
            //for(int j =0; j< 6; j++)
            //{
            //    for(int i = 0; i < 6; i++)
            //    {
            //        K[i0 + i, i0 + j] += Ke[i, j];
            //        K[i0 + i, i1 + j] += Ke[i, j + 6];
            //        K[i1 + i, i0 + j] += Ke[i + 6, j];
            //        K[i1 + i, i1 + j] += Ke[i + 6, j + 6];
            //    }
            //}


            int[] global_dof_map = new int[12];
            for(int i = 0; i < 6; i++)
            {
                global_dof_map[i] = i0 + i;
                global_dof_map[i + 6] = i1 + i;
            }


            for(int i = 0; i < 12; i++)
            {
                for(int j = 0; j < 12; j++)
                {
                    K[global_dof_map[i], global_dof_map[j]] += Ke[i, j];
                }
            }
        }
    }
}
