using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Angelfish
{
    public class GhcInterpolate : GH_Component
    {
        public GhcInterpolate()
          : base("Interpolate", "Interpolate",
              "Interpolate between regions in 2d",
              "Angelfish", "1.Setup")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Gradient", "Gradient", "Gradient", GH_ParamAccess.item);
            pManager.AddCurveParameter("Convex Hull", "Convex Hull", "Convex Hull", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Angelfish", "Angelfish", "Angelfish", GH_ParamAccess.item);
            //   pManager.AddPointParameter("InHull", "InHull", "InHull", GH_ParamAccess.list);
            pManager.AddPointParameter("Matches", "Matches", "Matches", GH_ParamAccess.tree);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Gradient gradient = null;
            DA.GetData(0, ref gradient);

            Curve temp = null;
            DA.GetData(1, ref temp);

            Polyline hull = null; 
            temp.TryGetPolyline(out hull);

            gradient.InHull(hull);

            //DA.SetDataList(1, gradient.pointsInbetween);
            DA.SetDataTree(1, gradient.matches);
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
            get { return new Guid("f3904bf4-284c-49c7-a6d7-a01f968d4bd3"); }
        }
    }
}