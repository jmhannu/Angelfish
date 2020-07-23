using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Angelfish
{
    public class GhcColourGradient : GH_Component
    {
        public GhcColourGradient()
          : base("Colour Gradient", "Gradient",
              "Colour Gradient",
              "Angelfish", "4. Utility")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("¨Mesh", "Mesh", "Mesh", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddColourParameter("Colours", "Colours", "Colours", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh mesh = null;
            DA.GetData(0, ref mesh);

            Interpolate gradient = new Interpolate(mesh);

            DA.SetDataList(0, gradient.colours);
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
            get { return new Guid("73359edc-c671-488e-a02e-e6411d6ad7af"); }
        }
    }
}