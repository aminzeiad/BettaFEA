using BettaLib.FEAModel;
using BettaLib.FEAStructure;
using BettaLib.Geometry;
using BettaLib.Utils;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BettaLib.FEAModelPix
{
    public class FEPixField
    {
        //Matripies
        public Matrix<double> A { get; set; } //Global Stiffness Matrix
        public Matrix<double> X { get; set; } //Displacement Vector
        public Vector<double> B { get; set; } //Load Vector
        public FEPixel[] Pixels { get; set; }
        public FENode2[] Nodes { get; set; }
        public Material Material { get; set; }
        public int PixCountX { get; set; }
        public int PixCountY { get; set; }
        public int NodeCountX => PixCountX + 1;
        public int NodeCountY => PixCountY + 1;
        public double[,] Ke = new double[8, 8];

        //optimization parameters
        public double TargetVolumeFraction = 0.4;
        public double Penalization = 3.0;

        ////smoothing filter parameters
        //public double FilterRadius = 1.2;
        //public double[] FilterKernel;
        //public int FilterHalfWidth;
        //public int FilterWidth => FilterHalfWidth * 2 + 1;

        public FEPixField(int pixCountX, int pixCountY, Material material)
        {
            PixCountX = pixCountX;
            PixCountY = pixCountY;
            Material = material;
            Nodes = new FENode2[NodeCountX * NodeCountY];
            for (int i = 0; i < Nodes.Length; i++)
            {
                Nodes[i] = new FENode2();
            }
            Pixels = new FEPixel[pixCountX * pixCountY];
            for (int i = 0; i < Pixels.Length; i++)
            {
                Pixels[i] = new FEPixel(Nodes[i], Nodes[i + 1], Nodes[i + pixCountX + 1], Nodes[i + pixCountX]);
            }

            //FilterKernel = BuildFilterKernel(FilterRadius, out FilterHalfWidth);
            Ke = FEPixel.BuildElementMatrix(material);

            //initialize density to target volume fraction
            Reset();
        }

        public void Reset()
        {
            for (int ni = 0; ni < Nodes.Length; ++ni)
            {
                Nodes[ni].U = Vector2.Zero;
                Nodes[ni].F = Vector2.Zero;
                Nodes[ni].SupportType = FENode2SupportType.None;
            }

            for (int pi = 0; pi < Pixels.Length; ++pi)
            {
                Pixels[pi].Density = TargetVolumeFraction;
                Pixels[pi].Sensitivity = 0.0;

                int i = pi % PixCountX;
                int j = pi / PixCountY;

                Pixels[pi].N0.Id = (i) + (j) * NodeCountX;
                Pixels[pi].N1.Id = (i + 1) + (j) * NodeCountX;
                Pixels[pi].N2.Id = (i + 1) + (j + 1) * NodeCountX;
                Pixels[pi].N3.Id = (i) + (j + 1) * NodeCountX;
            }
        }

        public void PeformAnalysis()
        {
            int totalDOF = Nodes.Length * 2;
            A = Matrix<double>.Build.Sparse(totalDOF, totalDOF);
            X = Matrix<double>.Build.Sparse(totalDOF, 1);
            B = Vector<double>.Build.Sparse(totalDOF);

            //populate the sparse stiffness matrix with the element stiffness matrices
            foreach (FEPixel p in Pixels)
            {
                //these are the indices of the DOFs of the 4 nodes of the element (where in the global sparse matrix each DOF is located)
                int[] cellNodeDOF = new int[]{
                                (2*p.N0.Id), (2*p.N0.Id+1),
                                (2*p.N1.Id), (2*p.N1.Id+1),
                                (2*p.N2.Id), (2*p.N2.Id+1),
                                (2*p.N3.Id), (2*p.N3.Id+1)
                             };

                //add the element stiffness matrix to the global sparse stiffness matrix at the correct location
                //we multpily the element stiffness matrix by the element density to the power of the penalization factor
                //the density makes the element stiffer or softer and the penalization factor increases the contrast between the soft and stiff elements
                for (int j = 0; j < 8; ++j)
                {
                    for (int i = j; i < 8; ++i)
                    {

                        A[cellNodeDOF[i], cellNodeDOF[j]] += Math.Pow(p.Density, Penalization) * Ke[j, i];
                    }
                }
            }

            //add forces to the sparse system vector B (the right hand side of the equation A.X=B)
            for (int i = 0; i < Nodes.Length; ++i)
            {
                B[2 * i] = Nodes[i].F.X;
                B[2 * i + 1] = Nodes[i].F.Y;
            }

            //add boundary conditions to the sparse system (Knock out rows and columns of known values)
            for (int i = 0; i < Nodes.Length; ++i)
            {
                if (Nodes[i].SupportType.HasFlag(FENode2SupportType.X))
                {
                    LockMatrixVariable((2 * i), 0.0);
                }
                if (Nodes[i].SupportType.HasFlag(FENode2SupportType.Y))
                {
                    LockMatrixVariable((2 * i + 1), 0.0);
                }
            }

            //solve the system
            Solve();
            //store the displacements in the nodes
            StoreDisplacements();

        }

        private void Solve()
        {
            //(A.X = B)
            //convert X to a column matrix and solve the system
            X = A.Inverse() * B.ToColumnMatrix();

            //you can make the solver more effipient by using skyline solver //IDK what that is
        }

        private void StoreDisplacements()
        {

            for (int i = 0; i < Nodes.Length; ++i)
            {
                Nodes[i].U = new Vector2(X[i * 2, 0], X[i * 2 + 1, 0]);
            }
        }

        public void ComputeSensitivity()
        {
            for (int pi = 0; pi < Pixels.Length; ++pi)
            {
                var pixel = Pixels[pi];
                // Nodal displacements of cell corners
                Vector<double> Ue = Vector<double>.Build.DenseOfArray(new double[]{
                Nodes[pixel.N0.Id].U.X,
                Nodes[pixel.N0.Id].U.Y,
                Nodes[pixel.N1.Id].U.X,
                Nodes[pixel.N1.Id].U.Y,
                Nodes[pixel.N2.Id].U.X,
                Nodes[pixel.N2.Id].U.Y,
                Nodes[pixel.N3.Id].U.X,
                Nodes[pixel.N3.Id].U.Y
            });

                // You can think of Ke.U as the element force vector and U.Ke.U as the element energy
                // The sensitivity is the derivative of the element energy with respect to the density
                Matrix<double> KeMatrix = Matrix<double>.Build.DenseOfArray(Ke);
                double mu = Ue.DotProduct(KeMatrix.Multiply(Ue));
                Pixels[pi].Sensitivity = -Penalization * Math.Pow(pixel.Density, Penalization - 1) * mu;
            }
        }

        public void TopologyOptimizationStep()
        {
            PeformAnalysis();
            ComputeSensitivity();
            //Filter(); // this step is not necessary, it is just a blur filter basically that smooths out the result
            UpdateDensity();
            //UpdateDensityUsingLagrangeMultipliers();
        }

        private void LockMatrixVariable(int id, double value)
        {
            int nx = A.RowCount;
            for (int i = 0; i < nx; ++i)
            {
                var entry = A[i, id];
                A[i, id] = 0.0;
                A[id, i] = 0.0;
                B[i] -= value * entry; //fixing the other side subtract the value that we know
            }

            A[id, id] = 1.0;
            B[id] = value;
        }

        private void LockMatrixVariableToZero(int id)
        {
            int nx = A.RowCount;
            for (int i = 0; i < nx; ++i)
            {
                A[i, id] = 0.0;
                A[id, i] = 0.0;
            }

            A[id, id] = 1.0;
            B[id] = 0.0;
        }

        private void LockMatrixVariableFast(int id, double value)
        {
            const double veryLarge = 90000000000000.0;

            A[id, id] = veryLarge;
            B[id] = value * veryLarge;
        }

        //build a convolution kernel (basically a square image with coeffipients that decay with distance from the center)
        //this could be a gaussian, but we use a simple linear decay
        public static double[] BuildFilterKernel(double radius, out int halfWidth, bool normalize = true)
        {
            halfWidth = (int)Math.Ceiling(radius);
            int size = 2 * halfWidth + 1;
            double[] kernel = new double[size * size];
            int index = 0;
            double sum = 0.0;
            for (int i = -halfWidth; i <= halfWidth; ++i)
            {
                for (int j = -halfWidth; j <= halfWidth; ++j)
                {
                    double fac = radius - Math.Sqrt(i * i + j * j);
                    if (fac < 0.0) fac = 0.0;
                    kernel[index++] = fac;
                    sum += fac;
                }
            }

            if (normalize)
            {
                for (int i = 0; i < kernel.Length; ++i)
                {
                    kernel[i] /= sum;
                }
            }

            return kernel;
        }

        public void UpdateDensity()
        {
            double[] newDensity = new double[Pixels.Length];
            double targetVolume = TargetVolumeFraction * PixCountX * PixCountY;
            double maximumChange = 0.2;
            double dt = 0.1;
            double newVolume = 0.0;

            for (int i = 0; i < Pixels.Length; ++i)
            {
                FEPixel pix = Pixels[i];
                newDensity[i] = pix.Density * Math.Sqrt(-pix.Sensitivity) * dt;
                newDensity[i] = Clamp(newDensity[i], pix.Density - maximumChange, pix.Density + maximumChange);
                newDensity[i] = Clamp(newDensity[i], 0.001, 1.0);
                newVolume += newDensity[i];
            }

            //copy new densities back to cells
            double volumeAdjust = 0.5 * (targetVolume - newVolume) / Pixels.Length;
            for (int i = 0; i < Pixels.Length; ++i)
            {
                Pixels[i].Density = newDensity[i] + volumeAdjust;
                Pixels[i].Density = Clamp(Pixels[i].Density, 0.001, 1.0);
            }
        }

        public static double Clamp(double v, double min, double max)
        {
            return Math.Max(min, Math.Min(max, v));
        }


        public double MaximumDisplacement() => Nodes.Cast<FENode2>().Max(n => n.U.Length);



        public FEPixel CellClamped(int x, int y)
        {
            return Pixels[CellIdClamped(x, y)];
        }


        public int CellIdClamped(int x, int y)
        {
            int pixX = Math.Max(0, Math.Min(PixCountX - 1, x));
            int pixY = Math.Max(0, Math.Min(PixCountY - 1, y));
            return pixX + pixY * PixCountX;
        }


        //public void Filter()
        //{
        //    //create temporary array for filtered sensitivities
        //    double[] filteredValues = new double[Pixels.Length];

        //    //apply convolution smoothing to sensitivities
        //    int cellId = 0;
        //    for (int j = 0; j < PixCountY; ++j)
        //    {
        //        for (int i = 0; i < PixCountX; ++i)
        //        {
        //            filteredValues[cellId] = 0.0;
        //            int kernelIndex = 0;
        //            //double sum = 0.0;
        //            for (int kj = -FilterHalfWidth; kj <= FilterHalfWidth; ++kj)
        //            {
        //                for (int ki = -FilterHalfWidth; ki <= FilterHalfWidth; ++ki)
        //                {
        //                    FEPixel ncell = CellClamped(i + ki, j + kj);
        //                    double kernelFac = FilterKernel[kernelIndex++] * ncell.Density;
        //                    filteredValues[cellId] += kernelFac * ncell.Sensitivity;// * ncell.Density;
        //                                                                            // sum += kernelFac;
        //                }
        //            }
        //            // filteredValues[cellId] /= sum;
        //            ++cellId;
        //        }
        //    }

        //    //copy smoothed sensitivities back to cells
        //    for (int i = 0; i < Pixels.Length; ++i)
        //    {
        //        Pixels[i].Sensitivity = filteredValues[i];
        //    }


        //}

    }

}
