using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Angelfish
{
    public class GhcSetRegion : GH_Component
    {
        public GhcSetRegion()
          : base("Set Region", "Region",
              "Set varibles to region",
              "Angelfish", "1.Setup")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Asystem", "Asystem", "Asystem", GH_ParamAccess.item);
            pManager.AddNumberParameter("Vertices indices", "Indices", "All vertices indices to region", GH_ParamAccess.list);
            pManager.AddNumberParameter("Values", "Values", "Values", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Asystem", "Asystem", "Asystem", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Asystem asystem = null;
            DA.GetData(0, ref asystem);

            List<int> indices = new List<int>();
            DA.GetDataList(1, indices);

            List<double> values = new List<double>();
            DA.GetDataList(2, values);

            for (int i = 0; i < indices.Count; i++)
            {
                asystem.Apoints[indices[i]].Da = values[0];
                asystem.Apoints[indices[i]].Db = values[1];
                asystem.Apoints[indices[i]].F = values[2];
                asystem.Apoints[indices[i]].K = values[3];
            }
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
            get { return new Guid("3d18152a-b4f5-4d5c-bfad-a16ffd55bede"); }
        }
    }
}