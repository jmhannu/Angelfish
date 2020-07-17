using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Angelfish
{
    public class GhcMeasure : GH_Component
    {
        //static string file = Properties.Resources.inputs2D;
        //Values values = new Values(file);
        Measure measure;

        public GhcMeasure()
          : base("Measure", "Measure",
              "Measure",
              "Angelfish", "3.Evaluate")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Pattern", "Pattern", "Pattern", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Mass percentage", "Mass", "Mass percentage", GH_ParamAccess.item);
            pManager.AddNumberParameter("Solid edges percentage", "Solid edge", "Solid edge percentage", GH_ParamAccess.item);
            pManager.AddNumberParameter("Connected percentage", "Connectivity", "Connected percentage", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Asystem pattern = null;
            DA.GetData("Pattern", ref pattern);

            measure = new Measure(pattern);

            DA.SetData(0, measure.MassPercentage);
            DA.SetData(1, measure.SolidEdgePercentage);
            DA.SetData(2, measure.ConnectedPercentage);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("4894e15d-9b18-4707-ae51-c80f55899012"); }
        }
    }
}