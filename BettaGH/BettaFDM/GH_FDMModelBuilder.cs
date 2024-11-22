using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel.Attributes;
using Rhino.Geometry;

namespace BettaGH.BettaFDM
{
    public class GH_FDMModelBuilder : GH_Component
    {
        public GH_FDMModelBuilder()
          : base("Betta FDM Builder", "FDM Builder",
              "Build an FDM model using a mesh and fixed Nodes",
              "Betta", "Builders")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Input Mesh", "M", "A mesh to perform the Finite Density Method on", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Fixed Points", "FP", "A list of fixed points", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Betta FDM Model", "M", "A Betta Model of branches and nodes", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Placeholder logic for inputs and outputs
            Mesh mesh = new Mesh();
            List<int> fixedNodeIndices = new List<int>();
            DA.GetData(0, ref mesh);
            DA.GetDataList(1, fixedNodeIndices);

            // Create the FDMModel instance (assuming FDMModel is a valid class defined elsewhere)
            FDMModel fdmModel = new FDMModel(mesh, fixedNodeIndices);

            // Output the model
            DA.SetData(0, fdmModel);
        }

        // Override to assign custom attributes
        public override void CreateAttributes()
        {
            m_attributes = new GH_FDMModelBuilderAttributes(this);
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("5301388C-BCD1-40F0-AB83-5362A42FC04E"); }
        }

        protected override Bitmap Icon
        {
            get
            {
                // Your icon logic here
                var imageBytes = Properties.Resources.icon_FDM_Model;
                using (var ms = new System.IO.MemoryStream(imageBytes))
                {
                    return new Bitmap(ms);
                }
            }
        }
    }

    // Custom attributes class for custom rendering
    public class GH_FDMModelBuilderAttributes : GH_ComponentAttributes
    {
        public GH_FDMModelBuilderAttributes(GH_Component owner) : base(owner) { }

        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            if (channel == GH_CanvasChannel.Objects)
            {
                RectangleF bounds = Bounds;

                // Draw a sharp-edged rectangle with white fill and black border
                using (SolidBrush whiteBrush = new SolidBrush(Color.White))
                {
                    graphics.FillRectangle(whiteBrush, bounds);
                }
                using (Pen blackPen = new Pen(Color.Black, 1))
                {
                    graphics.DrawRectangle(blackPen, bounds.X, bounds.Y, bounds.Width, bounds.Height);
                }

                // Draw the icon at the center of the bounds if available
                if (Owner.Icon_24x24 != null)
                {
                    int iconSize = 24; // Set icon size (modify as needed)
                    float iconX = bounds.X + (bounds.Width - iconSize) / 2;
                    float iconY = bounds.Y + (bounds.Height - iconSize) / 2;
                    graphics.DrawImage(Owner.Icon_24x24, iconX, iconY, iconSize, iconSize);
                }

                // Offset for placing grips outside the capsule boundary
                int offset = 2;

                // Draw input grips and labels outside the left boundary
                foreach (IGH_Param input in Owner.Params.Input)
                {
                    PointF gripLocation = input.Attributes.InputGrip;
                    graphics.FillRectangle(Brushes.Black, gripLocation.X - 3 - offset, gripLocation.Y - 3, 3, 3);

                    // Draw the input parameter name
                    string inputName = input.NickName;
                    graphics.DrawString(inputName, GH_FontServer.Small, Brushes.Black, gripLocation.X , gripLocation.Y - 7);
                }

                // Draw output grips and labels outside the right boundary
                foreach (IGH_Param output in Owner.Params.Output)
                {
                    PointF gripLocation = output.Attributes.OutputGrip;
                    graphics.FillRectangle(Brushes.Black, gripLocation.X - 3 + offset, gripLocation.Y - 3, 3, 3);

                    // Draw the output parameter name
                    string outputName = output.NickName;
                    graphics.DrawString(outputName, GH_FontServer.Small, Brushes.Black, gripLocation.X - 7, gripLocation.Y - 7);
                }
            }

            // Note: Do not call the base Render method to keep the custom appearance
        }
    }
}
