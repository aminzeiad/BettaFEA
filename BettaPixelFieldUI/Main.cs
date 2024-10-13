using BettaLib.FEAModel;
using BettaLib.FEAModelPix;
using BettaLib.FEAStructure;
using BettaLib.Geometry;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using OpenTK.Graphics;


namespace BettaPixelFieldUI
{
    public class Main
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
            public FEPixField model;

            double time = 0.0;
            int frame = 0;

            public void Initialization()
            {
                model = new FEPixField(3, 3, new Material("Dummy", 1.0, 0.3, 1.0, 1.0, 1.0));


                //fix left side nodes
                for (int j = 0; j < model.NodeCountY; ++j)
                {
                    model.Nodes[0 + j * model.NodeCountX].SupportType = FENode2SupportType.All;
                }

                //apply load to right side nodes
                for (int j = 0; j < model.NodeCountY; ++j)
                {
                    model.Nodes[model.NodeCountX - 1 + j * model.NodeCountX].F = new BettaLib.Geometry.Vector2(0.0, -1.0);
                }
            }




            public void Render()
            {
                time += 0.1;
                frame += 1;

            if (frame % 10 == 0)
            {
                model.TopologyOptimizationStep();
            }

            GL.ClearColor(0.5f, 0.5f, 0.5f, 1.0f);     //set the background colour to black
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit); //clear both the colour and depth buffers [used for 3d graphics]


                int w = model.PixCountX;
                int h = model.PixCountY;
                double maxU = model.MaximumDisplacement();





                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadIdentity();
                GL.Ortho(-5.0, w + 5.0, -5.0, h + 5.0, -10.0, 100.0);

                GL.Disable(EnableCap.DepthTest);
                GL.MatrixMode(MatrixMode.Modelview);


                double disp = 15.0 * (Math.Sin(time) * 0.5 + 0.5) / maxU;
                //render quads for the cells
                GL.Begin(BeginMode.Quads);
                for (int j = 0; j < h; ++j)
                {
                    for (int i = 0; i < w; ++i)
                    {
                        var pixel = model.Pixels[i + j * model.PixCountX];
                        GL.Color4(pixel.Density, pixel.Density, pixel.Density, 1.0);

                        var du0 = model.Nodes[pixel.N0.Id].U * disp;
                        var du1 = model.Nodes[pixel.N1.Id].U * disp;
                        var du2 = model.Nodes[pixel.N2.Id].U * disp;
                        var du3 = model.Nodes[pixel.N3.Id].U * disp;
                        GL.Vertex2(i + du0.X, j + du0.Y);
                        GL.Vertex2(i + 1 + du1.X, j + du1.Y);
                        GL.Vertex2(i + 1 + du2.X, j + 1 + du2.Y);
                        GL.Vertex2(i + du3.X, j + 1 + du3.Y);
                    }
                }
                GL.End();

            }
        }
    }
    
