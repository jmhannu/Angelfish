using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Angelfish
{
    public class GhcVariblesByMeasure : GH_Component
    {

        public GhcVariblesByMeasure()
          : base("VariblesByMeasure", "ByMeasures",
              "VariblesByMeasure",
              "Angelfish", "0.Varibles")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
              pManager.AddNumberParameter("Mass Influence", "Mass Influence", "Mass Influence", GH_ParamAccess.item, 1.0);
            pManager.AddNumberParameter("Connectivity Influence", "Connectivity Influence", "Connectivity Influence", GH_ParamAccess.item, 1.0);
            pManager.AddNumberParameter("Edge Connectivity", "Edge Connectivity", "Edge Connectivity", GH_ParamAccess.item, 1.0);
            pManager.AddNumberParameter("Solid Edge", "Solid Edge", "Solid Edge", GH_ParamAccess.item, 1.0);
            pManager.AddNumberParameter("Range", "Range", "Range to include in selection", GH_ParamAccess.item, 0.0);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            //  pManager.AddNumberParameter("Count combinations", "Count", "Number of varible combinations", GH_ParamAccess.item);
            pManager.AddNumberParameter("Varible combinations", "Varibles", "dA, dB, f, k", GH_ParamAccess.tree);
            //  pManager.AddNumberParameter("Mass procentages", "Mass", "Mass procentage per pattern", GH_ParamAccess.list);
            //  pManager.AddNumberParameter("Solid edges", "Solid edge", "Solid edge percentages", GH_ParamAccess.list);
            // pManager.AddNumberParameter("Connectivity rate", "Connectivity", "Connectivity rate", GH_ParamAccess.list);
            //pManager.AddNumberParameter("Edge connections", "Edge connect", "Percentages of parts with connetion to the edge", GH_ParamAccess.list);
            //pManager.AddNumberParameter("Select Index", "Select i", "Heighest/lowest mass index", GH_ParamAccess.list);
            //  pManager.AddNumberParameter("Weighted Number", "Weighted", "Weighted Number", GH_ParamAccess.item);

        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //string[] lines = file.Split('\n');

            //foreach (string line in lines)
            //{
            //    string fixedLine = line.Replace(',', '.');
            //    string[] lineValues = fixedLine.Split('\t');

            //    dAValues.Add(Convert.ToDouble(lineValues[0]));
            //    dBValues.Add(Convert.ToDouble(lineValues[1]));
            //    fValues.Add(Convert.ToDouble(lineValues[2]));
            //    kValues.Add(Convert.ToDouble(lineValues[3]));

            //}

            //DA.SetData(0, (double)values.ValuesCount);
            //DA.SetDataTree(0, values.Varibles);
            //DA.SetDataList(2, values.MassProcentages);
            //DA.SetDataList(3, values.SolidEdgeProcentage);
            //DA.SetDataList(4, values.ConnectedProcentages);
            //DA.SetDataList(5, values.EdgeConnectionProcentages);

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

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("f312fb18-77a7-40df-b801-d4727ce2ae22"); }
        }
    }
}