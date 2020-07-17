using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Angelfish
{

    public class GhcPattern : GH_Component
    {
        public GhcPattern()
          : base("Pattern", "Pattern",
              "Make Pattern",
              "Angelfish", "Calcuate")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("RD", "RD", "RD", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Solid Pattern", "Solids", "Solid Pattern", GH_ParamAccess.item, true);
            pManager.AddNumberParameter("Threshold value", "Threshold", "Threshold value, boundary value", GH_ParamAccess.item, 0.4);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Pattern", "Pattern", "Pattern", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            ReactionDiffusion reactDiffuse = null;
            DA.GetData("RD", ref reactDiffuse);

            double treshold = 0.0;
            DA.GetData("Threshold", ref treshold);

            bool solidPattern = true;
            DA.GetData(1, ref solidPattern);

            reactDiffuse.DividePoints(treshold);
           
            if(solidPattern)
            {
                for (int i = 0; i < reactDiffuse.Solid.Count; i++)
                {

                }
            }

            else
            {

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
            get { return new Guid("47786be3-cf0d-4062-9ef6-d109ccb196b5"); }
        }
    }
}