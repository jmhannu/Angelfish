using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Angelfish
{
    public class GhcRegion : GH_Component
    {
        public GhcRegion()
          : base("Region", "Region",
              "Set varibles to region",
              "Angelfish", "1.Setup")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Vertices", "Vertices", "Vertices", GH_ParamAccess.list);
            pManager.AddNumberParameter("Values", "Values", "Values", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Region", "Region", "Region", GH_ParamAccess.list);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {

            List<Point3d> vertices = new List<Point3d>();
            DA.GetDataList(0, vertices);

            List<double> values = new List<double>();
            DA.GetDataList(1, values);

            List<Apoint> apoints = new List<Apoint>();

            for (int i = 0; i < vertices.Count; i++)
            {
                apoints.Add(new Apoint(vertices[i], values));
            }

            DA.SetDataList(0, apoints);
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