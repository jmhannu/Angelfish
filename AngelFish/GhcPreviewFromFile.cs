using System;
using System.IO;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Angelfish
{
    public class GhcPreviewFromFile : GH_Component
    {
        public GhcPreviewFromFile()
          : base("Pattern preview from file", "Preview file",
              "Read pattern preview from tab-seperated .txt file. 1 - black, 0 - white.",
              "Angelfish", "0.Varibles")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Path", "Path", "Path", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Preview", "Preview", "Preview", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string path = null;
            DA.GetData(0, ref path);

            ReadWrite readValues = new ReadWrite();
            string file = File.ReadAllText(path);

            DA.SetDataList(0, readValues.FilePreview(file));
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
            get { return new Guid("d6ad0373-aaf6-46c7-b3af-4913194a3ff4"); }
        }
    }
}