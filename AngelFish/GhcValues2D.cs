using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Angelfish
{
    public class GhcValues2D : GH_Component
    {
        static string file = Properties.Resources.inputs2D;
        Values values = new Values(file);

        public GhcValues2D()
          : base("Values2D", "Values2D",
              "Varible combinations for GS RD, suiteble for 2D geometry",
              "Angelfish", "Values")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Mass Influence", "Mass", "Mass Influence", GH_ParamAccess.item, 1.0);
            pManager.AddNumberParameter("Connectivity Influence", "Connectivity", "Connectivity Influence", GH_ParamAccess.item, 1.0);
            pManager.AddNumberParameter("Edge Connectivity", "Edge", "Edge Connectivity", GH_ParamAccess.item, 1.0);
            pManager.AddNumberParameter("Solid Edge", "Solid Edge", "Solid Edge", GH_ParamAccess.item, 1.0);
            pManager.AddNumberParameter("Range", "Range", "Range to include in selection", GH_ParamAccess.item, 0.0);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Count combinations", "Count", "Number of varible combinations", GH_ParamAccess.item);
            pManager.AddNumberParameter("Varible combinations", "Varibles", "dA, dB, f, k", GH_ParamAccess.tree);
            pManager.AddNumberParameter("Mass procentages", "Mass", "Mass procentage per pattern", GH_ParamAccess.list);
            pManager.AddNumberParameter("Solid edges", "Solid edge", "Solid edge percentages", GH_ParamAccess.list);
            pManager.AddNumberParameter("Connectivity rate", "Connectivity", "Connectivity rate", GH_ParamAccess.list);
            pManager.AddNumberParameter("Edge connections", "Edge connect", "Percentages of parts with connetion to the edge", GH_ParamAccess.list);
            pManager.AddNumberParameter("Select Index", "Select i", "Heighest/lowest mass index", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            
            DA.SetData(0, (double)values.ValuesCount);
            DA.SetDataTree(1, values.Varibles);
            DA.SetDataList(2, values.MassProcentages);
            DA.SetDataList(3, values.SolidEdgeProcentage);
            DA.SetDataList(4, values.ConnectedProcentages);
            DA.SetDataList(5, values.EdgeConnectionProcentages);

            double weightMass, weightConnection, weightEdgeConnection, weightSolidEdge;
            weightMass = weightConnection = weightEdgeConnection = weightSolidEdge = 1.0;
            double range = 0.0;

            DA.GetData(0, ref weightMass);
            DA.GetData(1, ref weightConnection);
            DA.GetData(2, ref weightEdgeConnection);
            DA.GetData(3, ref weightSolidEdge);
            DA.GetData(4, ref weightSolidEdge);

            DA.SetDataList(6, values.SelectIndex(weightMass, weightConnection, weightEdgeConnection, weightSolidEdge, range));
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
            get { return new Guid("4894e15d-9b18-4707-ae51-c70f55899012"); }
        }
    }
}