using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Angelfish
{
    public class GhcCalculateRegions : GH_Component
    {
        int currentI;
        Gradient gradient;

        public GhcCalculateRegions()
          : base("Calculate Regions", "Regions",
              "Calulates the Grey Scott Reaction Diffusion pattern on points or vertices in a region",
              "Angelfish", "2.Calculate")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Gradient", "Gradient", "Gradient", GH_ParamAccess.item);
           pManager.AddBooleanParameter("Solid Arround", "Solid", "Solid Arround", GH_ParamAccess.item, true);
            pManager.AddIntegerParameter("Iterations", "Iterations", "Iterations of calculation", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("RD", "RD", "RD", GH_ParamAccess.item);
            pManager.AddNumberParameter("PrintOut", "PrintOut", "PrintOut", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int iterations = 0;
            DA.GetData("Iterations", ref iterations);

            currentI = 0;

            gradient = null;
            DA.GetData(0, ref gradient);

            bool solid = true;
            DA.GetData(1, ref solid);

            while (currentI < iterations)
            {
                for (int i = 0; i < gradient.regions.Count; i++)
                {
                    gradient.regions[i].Update();
                }
                currentI++;
            }

            Pattern outPattern = new Pattern(gradient.Apoints);

            for (int i = 0; i < outPattern.Apoints.Count; i++)
            {
                if (solid) outPattern.a[i] = 0.0;
                else outPattern.a[i] = 1.0;
            }

            for (int i = 0; i < gradient.regions.Count; i++)
            {
                for (int j = 0; j < gradient.regions[i].Apoints.Count; j++)
                {
                    int current = gradient.regions[i].Apoints[j].Index;
                    outPattern.Apoints[current] = gradient.regions[i].Apoints[j];
                    outPattern.a[current] = gradient.regions[i].a[j];
                }
            }

            DA.SetData(0, outPattern);
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

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("9b197e54-28c6-4b5a-b381-6903ba7ca00e"); }
        }
    }
}