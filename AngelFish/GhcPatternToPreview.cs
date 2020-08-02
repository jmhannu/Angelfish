using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Angelfish
{
    public class GhcPatternToPreview : GH_Component
    {
        public GhcPatternToPreview()
          : base("Pattern To Preview", "Pattern To Preview",
              "Pattern To Preview",
              "Angelfish", "4. Utility")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Pattern", "Pattern", "Pattern", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Preview", "Preview", "Preview", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Pattern pattern = null;
            DA.GetData("Pattern", ref pattern);

            ReadWrite writer = new ReadWrite();
            string toPreview = writer.WritePattern(pattern);
            string[] lineValues = toPreview.Split('\t');
            List<int> preview = new List<int>();
            for (int i = 0; i < lineValues.Length; i++)
            {
                preview.Add(Convert.ToInt32(lineValues[i]));
            }


            DA.SetDataList(0, preview);
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
            get { return new Guid("a85b61a9-848d-4b0b-801f-4cbb9256fe8b"); }
        }
    }
}