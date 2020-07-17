using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Angelfish
{
    public class GhcSetup : GH_Component
    {
        Asystem angelfishSystem;

        public GhcSetup()
          : base("GhcSetup", "Setup",
              "Prepare for RD",
              "Angelfish", "1.Setup")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "Mesh", "Mesh", GH_ParamAccess.item);
            pManager.AddNumberParameter("Varibles", "Varibles", "dA, dB, k, f", GH_ParamAccess.list);
          //  pManager.AddIntegerParameter("Index", "Index", "Index", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Angelfish", "Angelfish", "Angelfish", GH_ParamAccess.item);
            pManager.AddPointParameter("Min", "Min", "Min", GH_ParamAccess.item);
            pManager.AddPointParameter("Max", "Max", "Max", GH_ParamAccess.item);
            pManager.AddNumberParameter("edgeCount", "edgeCount", "edgeCount", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
                Mesh mesh = null;
                DA.GetData("Mesh", ref mesh);

                List<double> values = new List<double>();
                DA.GetDataList("Varibles", values);

                angelfishSystem = new Asystem(values, mesh);

        //    int index = 0;
        //    DA.GetData("Index", ref index);

            DA.SetData(0, angelfishSystem);
            DA.SetData(1, angelfishSystem.min);
            DA.SetData(2, angelfishSystem.max);
            DA.SetData(3, angelfishSystem.edgeCount);
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
            get { return new Guid("91221fc1-ce76-419c-a808-74eee5d65505"); }
        }
    }
}