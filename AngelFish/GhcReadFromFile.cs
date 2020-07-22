using System;
using System.IO;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace Angelfish
{
    public class GhcVariblesFromFile : GH_Component
    {
        public GhcVariblesFromFile()
          : base("Varibles from file", "Varibles file",
              "Read varibles and measures from tab-seperated .txt file. One line per varible combination. Order: dA /t/ dB /t/ f /t/ k /t/ mass /t/ connectivity /t/ edge",
              "Angelfish", "0.Varibles")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Path", "Path", "Path", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Index", "Index", "Index", GH_ParamAccess.item, 0);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Varible combinations", "Varibles", "dA, dB, f, k", GH_ParamAccess.list);
            pManager.AddNumberParameter("Mass percentage", "Mass", "Mass percentage", GH_ParamAccess.item);
            pManager.AddNumberParameter("Connected percentage", "Connectivity", "Connected percentage", GH_ParamAccess.item);
            pManager.AddNumberParameter("Solid edges percentage", "Solid edge", "Solid edge percentage", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string path = null; 
            DA.GetData(0, ref path);

            int index = 0;
            DA.GetData(1, ref index);

            ReadWrite readValues = new ReadWrite();
            string file = File.ReadAllText(path);

            readValues.ReadLegacy(file);

            List<GH_Number> varibles = readValues.Varibles.get_Branch(index) as List<GH_Number>;

            DA.SetDataList(0, varibles);
            DA.SetDataList(1, readValues.MassPercentage);
            DA.SetDataList(3, readValues.ConnectedPercentage);
            DA.SetDataList(2, readValues.SolidEdgePercentage);
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
            get { return new Guid("701cd6c1-8563-4bb0-8508-d1a16ce79f95"); }
        }
    }
}