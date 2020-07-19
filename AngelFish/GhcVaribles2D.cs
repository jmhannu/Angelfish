using System;
using System.IO;
using System.Collections.Generic;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace Angelfish
{
    public class GhcVaribles2D : GH_Component
    {
        //static string path = "C:/Users/julia/OneDrive/Dokument/GitHub/Angelfish/AngelFish/Resources/inputs2D.txt";
        static string file = Resources.inputs2D;
        //ReadWrite fromFile = new ReadWrite(path, true);
        ReadWrite fromFile = new ReadWrite(file, true);


        public GhcVaribles2D()
          : base("Embedded varibles and measures for 2D", "Varibles 2D",
              "Embedded varibles and measure to use for 2D patterns on meshes or 2D grids of points",
              "Angelfish", "0.Varibles")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("Index", "Index", "Index", GH_ParamAccess.item, 0);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Varible combinations", "Varibles", "dA, dB, f, k", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int index = 0;
            DA.GetData(0, ref index);


            List<GH_Number> varibles = fromFile.Varibles.get_Branch(index) as List<GH_Number>;

            DA.SetDataList(0, varibles);
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
            get { return new Guid("47aa0cce-98d4-4762-9cdf-7e2fc73cd5bc"); }
        }
    }
}