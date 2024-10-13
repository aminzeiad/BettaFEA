using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BettaPixelFieldUI;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Windows.Forms;

namespace BettaPixelFieldUI
{
    public partial class MainWindowForm : Form
    {
        public MainWindowForm()
        {
            Resize += new EventHandler(MainWindowForm_Resize);
            InitializeComponent();            
        }

        Main Main = new Main();
        bool HasLoaded = false;
        System.Windows.Forms.Timer RenderTimer = new System.Windows.Forms.Timer();

        void MainWindowForm_Resize(object sender, EventArgs e)
        {            
            GLviewport.Location = new Point(0, 0);
            GLviewport.Size = new System.Drawing.Size(ClientSize.Width, ClientSize.Height);
        }

        void UpdateFrame()
        {
            if (!HasLoaded) return;
            Main.Render();
            // Update the form's title to display the maximum displacement
            this.Text = $"Max Displacement: {Main.model.MaximumDisplacement()}";
            GLviewport.SwapBuffers();
        }

        private void GLviewport_Load(object sender, EventArgs e)
        {
            Main.Initialization();

            HasLoaded = true;
            RenderTimer.Interval = 15;
            RenderTimer.Enabled = true;
            RenderTimer.Start();
            RenderTimer.Tick += new EventHandler(timer_Tick);


            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.PointSmooth);
            GL.Enable(EnableCap.LineSmooth);

            GL.ColorMaterial(MaterialFace.FrontAndBack, ColorMaterialParameter.Diffuse);
            GL.Enable(EnableCap.ColorMaterial);
            GL.Enable(EnableCap.Normalize);


            GL.LightModel(LightModelParameter.LightModelTwoSide, 1);
            GL.LightModel(LightModelParameter.LightModelLocalViewer, 1);

            GL.BlendFunc((BlendingFactor)BlendingFactorSrc.SrcAlpha, (BlendingFactor)BlendingFactorDest.OneMinusSrcAlpha);
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (!HasLoaded) return;
            UpdateFrame();
        }

        private void GLviewport_Resize(object sender, EventArgs e)
        {
            if (!HasLoaded) return;            
            GL.Viewport(0, 0, GLviewport.Width, GLviewport.Height); 
            UpdateFrame();
        }

        private void GLviewport_Paint(object sender, PaintEventArgs e)
        {
            if (!HasLoaded) return;
            UpdateFrame();
        }

  
        private void MainWindowForm_FormClosing(object sender, FormClosingEventArgs e)
        {
        }


    }
}
