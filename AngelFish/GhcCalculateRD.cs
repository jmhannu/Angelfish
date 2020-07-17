using System;
using System.Collections.Generic;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace Angelfish
{
    public class GhcCalculateRD : GH_Component
    {
        int currentI;
        ReactionDiffusion reactDiffuse;

        public GhcCalculateRD()
          : base("Calculate RD", "Calcuate RD",
              "Calulates the Grey Scott Reaction Diffusion pattern on a set of points or vertices",
              "Angelfish", "Calculate")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Angelfish", "Angelfish", "Angelfish", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Iterations", "Iterations", "Iterations of calculation", GH_ParamAccess.item);

        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("RD", "RD", "RD", GH_ParamAccess.item);
            //pManager.AddPointParameter("Solid", "Solid", "Solid or black points", GH_ParamAccess.list);
            //pManager.AddPointParameter("Void", "Void", "Void or white points", GH_ParamAccess.list);
            pManager.AddNumberParameter("PrintOut", "PrintOut", "PrintOut", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {

            int iterations = 0;
            DA.GetData("Iterations", ref iterations);

            currentI = 0;

            //double treshold = 0.0;
            //DA.GetData("Threshold", ref treshold);

            Asystem angelfish = null;
            DA.GetData("Angelfish", ref angelfish);
            reactDiffuse = new ReactionDiffusion(angelfish);


            while (currentI < iterations)
            {
                reactDiffuse.Update();
                currentI++;
            }


            //reactDiffuse.DividePoints(treshold);
            DA.SetData(0, reactDiffuse);
            //DA.SetDataList("Solid", reactDiffuse.Solid);
            //DA.SetDataList("Void", reactDiffuse.Void);
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
            get { return new Guid("164e7e51-a496-4307-bb63-8e2bbd89541f"); }
        }
    }
}