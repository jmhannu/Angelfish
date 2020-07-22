using System;
using System.IO;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Angelfish
{
    public class GhcVariblesToFile : GH_Component
    {

        public GhcVariblesToFile()
          : base("Varibles and measures to file", "Varibles To File",
              "Saves varibles and measures to .txt file",
              "Angelfish", "4. Utility")
        {
        }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Path", "Path", "Path to save to", GH_ParamAccess.item);
            pManager.AddNumberParameter("Varibles", "Varibles", "Varibles", GH_ParamAccess.list);
            pManager.AddNumberParameter("Mass P", "MassP", "Mass percentage", GH_ParamAccess.item);
            pManager.AddNumberParameter("Connectivity P", "Connectivity P", "Connected percentage", GH_ParamAccess.item);
            pManager.AddNumberParameter("Solid edge P", "Solid edge P", "Solid edge percentage", GH_ParamAccess.item);

            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string path = null;
            DA.GetData("Path", ref path);

            ReadWrite writer = new ReadWrite();

            List<double> varibles = new List<double>();
            DA.GetDataList("Varibles", varibles);

            double massP = 0.0;
            DA.GetData("Mass P", ref massP);

            double solidEdgeP = 0.0;
            DA.GetData("Solid edge P", ref solidEdgeP);

            double connectivityP = 0.0;
            DA.GetData("Connectivity P", ref connectivityP);

            writer.WriteToFile(path, varibles, massP, connectivityP, solidEdgeP);
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
            get { return new Guid("ca63cee2-84b2-4223-af79-203dc937a945"); }
        }
    }
}