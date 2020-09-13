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
              "Angelfish", "2.Calculate")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("RD", "RD", "RD", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Solid Pattern", "Solids", "Solid Pattern", GH_ParamAccess.item, true);
            pManager.AddNumberParameter("Threshold", "Threshold", "Threshold, boundary value", GH_ParamAccess.item, 0.4);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Pattern", "Pattern", "Pattern", GH_ParamAccess.item);
            pManager.AddPointParameter("Points", "Points", "Points", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Pattern reactDiffuse = null;
            DA.GetData("RD", ref reactDiffuse);

            double treshold = 0.0;
            DA.GetData("Threshold", ref treshold);

            bool solidPattern = true;
            DA.GetData(1, ref solidPattern);

            reactDiffuse.DividePoints(treshold);

            List<Point3d> outputPoints = new List<Point3d>();

            if (solidPattern)
            {
                List<int> solids = new List<int>(reactDiffuse.Solid);
                reactDiffuse.SolidPattern = true; 

                for (int i = 0; i < solids.Count; i++)
                {
                    reactDiffuse.Apoints[solids[i]].InPattern = true;
                    outputPoints.Add(reactDiffuse.Apoints[solids[i]].Pos);
                }
            }

            else
            {
                List<int> voids = new List<int>(reactDiffuse.Void);
                reactDiffuse.SolidPattern = false; 

                for (int i = 0; i < voids.Count; i++)
                {
                    reactDiffuse.Apoints[voids[i]].InPattern = true;
                    outputPoints.Add(reactDiffuse.Apoints[voids[i]].Pos);
                }
            }

            DA.SetData(0, reactDiffuse);
            DA.SetDataList(1, outputPoints);
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