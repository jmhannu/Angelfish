using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Angelfish
{
    public class GhcSaveMeasures : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GhcSaveMeasures class.
        /// </summary>
        public GhcSaveMeasures()
          : base("Save Measures", "Save Measures",
              "Save Measures",
              "Angelfish", "3.Evaluate")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Path", "Path", "Path to save to", GH_ParamAccess.item);
            pManager.AddNumberParameter("Varibles", "Varibles", "Varibles", GH_ParamAccess.list);
            pManager.AddNumberParameter("Mass P", "MassP", "Mass percentage", GH_ParamAccess.item);
            pManager.AddNumberParameter("Solid edge P", "Solid edge P", "Solid edge percentage", GH_ParamAccess.item);
            pManager.AddNumberParameter("Connectivity P", "Connectivity P", "Connected percentage", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string path = null;
            DA.GetData("Path", ref path);

            ReadWrite toFile = new ReadWrite(path, false);

            List<double> varibles = new List<double>();
            DA.GetDataList("Varibles", varibles);

            double massP = 0.0;
            DA.GetData("Mass P", ref massP);

            double solidEdgeP = 0.0;
            DA.GetData("Solid edge P", ref solidEdgeP);

            double connectivityP = 0.0;
            DA.GetData("Connectivity P", ref connectivityP);

            toFile.Write(varibles, massP, connectivityP, solidEdgeP);
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
            get { return new Guid("4bae2cdb-7696-488e-bbe0-d2805b7499b7"); }
        }
    }
}