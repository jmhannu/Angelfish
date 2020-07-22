using System;
using System.Collections.Generic;
using System.IO; 
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace Angelfish
{
    public class GhcPreviewToFile : GH_Component
    {

        public GhcPreviewToFile()
          : base("Preview To File", "Preview To File",
              "Saves out pattern as preview to .txt-file",
              "Angelfish", "4. Utility")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Path", "Path", "Path, including .txt", GH_ParamAccess.item);
            pManager.AddNumberParameter("Preview", "Preview", "Preview", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string path = null;
            DA.GetData("Path", ref path);

            List<GH_Number> numbers = new List<GH_Number>();
            DA.GetDataList("Preview", numbers);

            string preview = null; 
            for (int i = 0; i < numbers.Count; i++)
            {
                if(i!=0) preview += '\t'; ;
                preview += numbers[i].Value.ToString();
            }
            File.WriteAllText(path, preview);
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
            get { return new Guid("89c22e81-6c39-4cbc-84d4-cdc5e75e93c5"); }
        }
    }
}