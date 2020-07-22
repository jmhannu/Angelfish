using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Angelfish
{
    public class GhcPickByMeasure : GH_Component
    {

        public GhcPickByMeasure()
          : base("PickByMeasure", "PickByMeasure",
              "Pick varibles by measure",
              "Angelfish", "0.Varibles")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Measured Varibles", "Measured Varibles", "Measured Varibles", GH_ParamAccess.item);
            pManager.AddNumberParameter("Weight mass", "Mass", "Weight mass", GH_ParamAccess.item, 1.0);
            pManager.AddNumberParameter("Weight connectivity", "Weight connect", "Weight connectivity", GH_ParamAccess.item, 1.0);
            pManager.AddNumberParameter("Weight solid edge", "Weight edge", "Edge Connectivity", GH_ParamAccess.item, 1.0);
            pManager.AddNumberParameter("Range", "Range", "Range to include in selection", GH_ParamAccess.item, 0.0);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Picked indices", "Indices", "Picked indices", GH_ParamAccess.list);
            pManager.AddNumberParameter("Count", "Count", "Count", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            ReadWrite measures = null;

            double weightMass, weightConnection, weightSolidEdge;
            weightMass = weightConnection = weightSolidEdge = 1.0;

            double addRange = 0.0;

            DA.GetData(0, ref measures);
            DA.GetData(1, ref weightMass);
            DA.GetData(2, ref weightConnection);
            DA.GetData(3, ref weightSolidEdge);
            DA.GetData(4, ref addRange);

            DA.SetDataList(0, measures.SelectIndex(weightMass, weightConnection, weightSolidEdge, addRange));
            DA.SetData(1, measures.counted);
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