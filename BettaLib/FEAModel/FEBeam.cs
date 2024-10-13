using BettaLib.FEAStructure;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using BettaLib.Utils;
using BettaLib.Geometry;

namespace BettaLib.FEAModel
{

    public struct CoordinateSystem
    {
        public static CoordinateSystem FromXAxisAndUp(Point3 origin, Vector3 xAxis, Vector3 up)
        {
            //x y -> z
            //y z -> x
            //z x -> y
            var zAxis = up;
            xAxis.Normalize();



            var yAxis = Vector3.Cross(xAxis, zAxis);


            var l = yAxis.Length;
            if (l < Constants.Epsilon)
            {
                if (Math.Abs(xAxis.Z) > 0.5)
                {
                    yAxis = Vector3.UnitX;
                }
                else
                {
                    yAxis = Vector3.UnitZ;
                }
            }
            else
            {
                yAxis.Normalize();
            }
           
            zAxis = Vector3.Cross(xAxis, yAxis);
            zAxis.Normalize();
            return new CoordinateSystem
            {
                Origin = origin,
                XAxis = xAxis,
                YAxis = yAxis,
                ZAxis = zAxis
            };
        }
        public Point3 Origin;
        public Vector3 XAxis, YAxis, ZAxis;

        public Matrix<double> GetRotationMatrix()
        {
           var mat = Matrix<double>.Build.Dense(3, 3);
            mat[0, 0] = XAxis.X;
            mat[0, 1] = YAxis.X;
            mat[0, 2] = ZAxis.X;
            mat[1, 0] = XAxis.Y;
            mat[1, 1] = YAxis.Y;
            mat[1, 2] = ZAxis.Y;
            mat[2, 0] = XAxis.Z;
            mat[2, 1] = YAxis.Z;
            mat[2, 2] = ZAxis.Z;
            return mat;        
        }
    }

    public class FEBeam : IEdge, IFEElement
    {
        public Matrix<double>? LocalEquivalentNodalLoad;
        public int DOF => 12; // 6 DOF per node

        bool _coordSystemNeedsUpdate = true;

        public void InvalidateCoordinateSystem()
        {
            _coordSystemNeedsUpdate = true;
        }

        public INode N0 { get; set; }
        public INode N1 { get; set; }

        public Point3 cG => (N0.Position + N1.Position) / 2;

        private Vector3 _upVector = Vector3.UnitZ;
        public Vector3 UpVector { get => _upVector; set { _upVector = value; InvalidateCoordinateSystem(); } } 

        public double Length => N0.Position.DistanceTo(N1.Position);

        public CrossSection CrossSection;

        public Beam? Origin { get; set; }

        private CoordinateSystem _localCoordinateSystem;

        public CoordinateSystem LocalCoordinateSystem {
            get { 
                if (_coordSystemNeedsUpdate)
                {
                    RefreshCoordinates();
                }
                return _localCoordinateSystem;
            }
        }

        public FEBeam(FENode node1, FENode node2, Beam? Origin = null)
        {
            if (Origin != null)
            {
                CrossSection = Origin.CrossSection;
                UpVector = Origin.UpVector;
                //Vxx = Origin.Vxx;
                //Vyy = Origin.Vyy;
                //Vzz = Origin.Vzz;
            }
            else
            {
                CrossSection = new CrossSection();
                RefreshCoordinates();
            }
            N0 = node1;
            N1 = node2;
            this.Origin = Origin;
        }

        public FEBeam(INode node1, INode node2, CrossSection cs, Vector3 upVector)
        {
            N0 = node1;
            N1 = node2;
            CrossSection = cs;
            UpVector = upVector;
        }

        public FEBeam(INode node1, INode node2, CrossSection cs)
        {
            N0 = node1;
            N1 = node2;
            CrossSection = cs;
        }


        public FEBeam() {
            N0 = new FENode();
            N1 = new FENode();
            CrossSection = new CrossSection();
            RefreshCoordinates();
        }

        protected Matrix<double> CalculateLocalStiffnessMatrix()
        {
            //Calculating the local stiffness matrix - a 12x12 matrix with the stiffness values
            //For a beam element, the local stiffness matrix is a 12x12 matrix, 6x6 for each node
            Matrix<double> ke = Matrix<double>.Build.Dense(DOF, DOF);

            double E = CrossSection.Material.ElasticModulus;
            double A = CrossSection.Area;
            double G = CrossSection.Material.ShearModulus;
            double Jxx = CrossSection.Jxx;
            double Iyy = CrossSection.Iyy;
            double Izz = CrossSection.Izz;
            double L = Length;

            var L2 = L * L;
            var L3 = L2 * L;

            ke[0, 0] = E * A / L;
            ke[0, 6] = -E * A / L;
            ke[1, 1] = 12 * E * Izz / Math.Pow(L, 3);
            ke[1, 5] = 6 * E * Izz / L2;
            ke[1, 7] = -12 * E * Izz / Math.Pow(L, 3);
            ke[1, 11] = 6 * E * Izz / L2;
            ke[2, 2] = 12 * E * Iyy / Math.Pow(L, 3);
            ke[2, 4] = -6 * E * Iyy / L2;
            ke[2, 8] = -12 * E * Iyy / Math.Pow(L, 3);
            ke[2, 10] = -6 * E * Iyy / L2;
            ke[3, 3] = G * Jxx / L;
            ke[3, 9] = -G * Jxx / L;
            ke[4, 2] = -6 * E * Iyy / L2;
            ke[4, 4] = 4 * E * Iyy / L;
            ke[4, 8] = 6 * E * Iyy / L2;
            ke[4, 10] = 2 * E * Iyy / L;
            ke[5, 1] = 6 * E * Izz / L2;
            ke[5, 5] = 4 * E * Izz / L;
            ke[5, 7] = -6 * E * Izz / L2;
            ke[5, 11] = 2 * E * Izz / L;
            ke[6, 0] = -E * A / L;
            ke[6, 6] = E * A / L;
            ke[7, 1] = -12 * E * Izz / Math.Pow(L, 3);
            ke[7, 5] = -6 * E * Izz / L2;
            ke[7, 7] = 12 * E * Izz / Math.Pow(L, 3);
            ke[7, 11] = -6 * E * Izz / L2;
            ke[8, 2] = -12 * E * Iyy / Math.Pow(L, 3);
            ke[8, 4] = 6 * E * Iyy / L2;
            ke[8, 8] = 12 * E * Iyy / Math.Pow(L, 3);
            ke[8, 10] = 6 * E * Iyy / L2;
            ke[9, 3] = -G * Jxx / L;
            ke[9, 9] = G * Jxx / L;
            ke[10, 2] = -6 * E * Iyy / L2;
            ke[10, 4] = 2 * E * Iyy / L;
            ke[10, 8] = 6 * E * Iyy / L2;
            ke[10, 10] = 4 * E * Iyy / L;
            ke[11, 1] = 6 * E * Izz / L2;
            ke[11, 5] = 2 * E * Izz / L;
            ke[11, 7] = -6 * E * Izz / L2;
            ke[11, 11] = 4 * E * Izz / L;


            return ke;
        }


        protected Matrix<double> CalculateTransformationMatrix()
        {
            //Calculating the trasformation matrix - a 12x12 matrix with Lamda on the diagonal
            Matrix<double> T = Matrix<double>.Build.Dense(DOF, DOF);
            //Calculating the local coordinate system Lamda
            //double[,] Lambda = new double[3, 3];

            ////Here I get an error because sometime the FEBeam has no Origin
            ////the only reason to use the origin is to get the local coordinate system
            ////so maybe I can get the local cordiante system by creating Beam object with the same N0 and N1
            ////and then use the Beam object to get the local coordinate system

            //Lambda[0, 0] = Vxx.X; //Later on we can ask the user to specify where is the local y axis instead of just assumin
            //Lambda[0, 1] = Vxx.Y;
            //Lambda[0, 2] = Vxx.Z;
            //Lambda[1, 0] = Vzz.X;
            //Lambda[1, 1] = Vzz.Z;
            //Lambda[1, 2] = Vzz.Y;
            //Lambda[2, 0] = Vyy.Z;
            //Lambda[2, 1] = Vyy.Z;
            //Lambda[2, 2] = Vyy.Y;

            var Lambda = LocalCoordinateSystem.GetRotationMatrix();//.ToArray();

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
            var Tt = T.Transpose();
            var kg = Tt * k * T;

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
            for (int i = 0; i < 6; i++)
            {
                global_dof_map[i] = i0 + i;
                global_dof_map[i + 6] = i1 + i;
            }


            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    K[global_dof_map[i], global_dof_map[j]] += Ke[i, j];
                }
            }
        }

        //Methods
        public void RefreshCoordinates()
        {
           _localCoordinateSystem = CoordinateSystem.FromXAxisAndUp(N0.Position, N1.Position - N0.Position, UpVector);
            _coordSystemNeedsUpdate = false;

        }

        public String PrintLocalStiffnessMatrix()
        {
            return CalculateLocalStiffnessMatrix().ToString(30, 30);
        }

        public String PrintGlobalElementalStiffnessMatrix()
        {
            return CalculateGlobalElementalStiffnessMatrix().ToString(30, 30);
        }

        public String PrintTransformationMatrix()
        {
            return CalculateTransformationMatrix().ToString(30, 30);
        }


    }
}
