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
              "Angelfish", "Evaluate")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Angelfish", "Angelfish", "Angelfish", GH_ParamAccess.item);
            pManager.AddNumberParameter("Threshold value", "Threshold", "Threshold value, boundary value", GH_ParamAccess.item, 0.4);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Mass procentages", "Mass", "Mass procentage per pattern", GH_ParamAccess.list);
            pManager.AddNumberParameter("Solid edges", "Solid edge", "Solid edge percentages", GH_ParamAccess.list);
            pManager.AddNumberParameter("Connectivity rate", "Connectivity", "Connectivity rate", GH_ParamAccess.list);
            pManager.AddNumberParameter("Edge connections", "Edge connect", "Percentages of parts with connetion to the edge", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {



            DA.SetDataList(0, measure.MassProcentages);
            DA.SetDataList(1, measure.SolidEdgeProcentage);
            DA.SetDataList(2, measure.ConnectedProcentages);
            DA.SetDataList(3, measure.EdgeConnectionProcentages);

            //double weightMass, weightConnection, weightEdgeConnection, weightSolidEdge;
            //weightMass = weightConnection = weightEdgeConnection = weightSolidEdge = 1.0;
            //double addRange = 0.0;

            //DA.GetData(0, ref weightMass);
            //DA.GetData(1, ref weightConnection);
            //DA.GetData(2, ref weightEdgeConnection);
            //DA.GetData(3, ref weightSolidEdge);
            //DA.GetData(4, ref addRange);

            //DA.SetDataList(6, values.SelectIndex(weightMass, weightConnection, weightEdgeConnection, weightSolidEdge, addRange));
            //DA.SetData(7, values.WeightedValue);
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