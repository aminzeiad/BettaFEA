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
    public interface IFEElement //to make it generalizable to other elements
    {
        void AssembleOnGlobalStiffnessMatrix(Matrix<double> K);
        int DOF { get; }
    }
    public class FEBeam : IEdge, IFEElement
    {
        public Matrix<double> LocalEquivalentNodalLoad;
        public int DOF => 12; // 6 DOF per node

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
            //Calculating the local stiffness matrix - a 12x12 matrix with the stiffness values
            //For a beam element, the local stiffness matrix is a 12x12 matrix, 6x6 for each node
            Matrix<double> ke = Matrix<double>.Build.Dense(DOF, DOF);

            double E = Origin.CrossSection.Material.ElasticModulus;
            double A = Origin.CrossSection.Area;
            double G = Origin.CrossSection.Material.ShearModulus;
            double Ixx = Origin.CrossSection.Ixx;
            double Iyy = Origin.CrossSection.Iyy;
            double Izz = Origin.CrossSection.Izz;
            double J = Origin.CrossSection.J;
            double L = Origin.Length;

            ke[0, 0] = E * A / L;
            ke[0, 6] = -E * A / L;
            ke[1, 1] = 12 * E * Izz / Math.Pow(L, 3);
            ke[1, 5] = 6 * E * Izz / Math.Pow(L, 2);
            ke[1, 7] = -12 * E * Izz / Math.Pow(L, 3);
            ke[1, 11] = 6 * E * Izz / Math.Pow(L, 2);
            ke[2, 2] = 12 * E * Iyy / Math.Pow(L, 3);
            ke[2, 4] = -6 * E * Iyy / Math.Pow(L, 2);
            ke[2, 8] = -12 * E * Iyy / Math.Pow(L, 3);
            ke[2, 10] = -6 * E * Iyy / Math.Pow(L, 2);
            ke[3, 3] = G * J / L;
            ke[3, 9] = -G * J / L;
            ke[4, 2] = -6 * E * Iyy / Math.Pow(L, 2);
            ke[4, 4] = 4 * E * Iyy / L;
            ke[4, 8] = 6 * E * Iyy / Math.Pow(L, 2);
            ke[4, 10] = 2 * E * Iyy / L;
            ke[5, 1] = 6 * E * Izz / Math.Pow(L, 2);
            ke[5, 5] = 4 * E * Izz / L;
            ke[5, 7] = -6 * E * Izz / Math.Pow(L, 2);
            ke[5, 11] = 2 * E * Izz / L;
            ke[6, 0] = -E * A / L;
            ke[6, 6] = E * A / L;
            ke[7, 1] = -12 * E * Izz / Math.Pow(L, 3);
            ke[7, 5] = -6 * E * Izz / Math.Pow(L, 2);
            ke[7, 7] = 12 * E * Izz / Math.Pow(L, 3);
            ke[7, 11] = -6 * E * Izz / Math.Pow(L, 2);
            ke[8, 2] = -12 * E * Iyy / Math.Pow(L, 3);
            ke[8, 4] = 6 * E * Iyy / Math.Pow(L, 2);
            ke[8, 8] = 12 * E * Iyy / Math.Pow(L, 3);
            ke[8, 10] = 6 * E * Iyy / Math.Pow(L, 2);
            ke[9, 3] = -G * J / L;
            ke[9, 9] = G * J / L;
            ke[10, 2] = -6 * E * Iyy / Math.Pow(L, 2);
            ke[10, 4] = 2 * E * Iyy / L;
            ke[10, 8] = 6 * E * Iyy / Math.Pow(L, 2);
            ke[10, 10] = 4 * E * Iyy / L;
            ke[11, 1] = 6 * E * Izz / Math.Pow(L, 2);
            ke[11, 5] = 2 * E * Izz / L;
            ke[11, 7] = -6 * E * Izz / Math.Pow(L, 2);
            ke[11, 11] = 4 * E * Izz / L;


            return ke;
        }


        protected Matrix<double> CalculateTransformationMatrix()
        {
            //Calculating the trasformation matrix - a 12x12 matrix with Lamda on the diagonal
            Matrix<double> T = Matrix<double>.Build.Dense(DOF, DOF);
            //Calculating the local coordinate system Lamda
            double[,] Lambda = new double[3, 3];

            Lambda[0, 0] = Origin.vx.X; //Later on we can ask the user to specify where is the local y axis instead of just assumin
            Lambda[0, 1] = Origin.vx.Y;
            Lambda[0, 2] = Origin.vx.Z;
            Lambda[1, 0] = Origin.vy.X;
            Lambda[1, 1] = Origin.vy.Y;
            Lambda[1, 2] = Origin.vy.Z;
            Lambda[2, 0] = Origin.vz.Z;
            Lambda[2, 1] = Origin.vz.Y;
            Lambda[2, 2] = Origin.vz.Z;

            //Filling the transformation matrix with lambda in the diagonal
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
