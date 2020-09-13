using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Angelfish
{
    public class GhcRegionCurve : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GhcRegionCurve class.
        /// </summary>
        public GhcRegionCurve()
          : base("RegionCurve", "RegionCurve",
              "Set varibles to region",
              "Angelfish", "1.Setup")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "Mesh", "Mesh", GH_ParamAccess.item);
            pManager.AddNumberParameter("Values", "Values", "Values", GH_ParamAccess.list);
            pManager.AddCurveParameter("Curve", "Curve", "Curve", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Region", "Region", "Region", GH_ParamAccess.item);
            pManager.AddPointParameter("Points", "Points", "Points", GH_ParamAccess.list);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {

            Mesh mesh = null;
            DA.GetData(0, ref mesh);

            List<double> values = new List<double>();
            DA.GetDataList(1, values);

            Curve curve = new LineCurve();
            DA.GetData(0, ref curve);

            Region region = new Region(mesh, values, curve);

            List<Point3d> output = new List<Point3d>();
            List<int> indicies = new List<int>();

            for (int i = 0; i < region.Apoints.Count; i++)
            {
                output.Add(region.Apoints[i].Pos);
                indicies.Add(region.Apoints[i].Index);
            }

            DA.SetData(0, region);
            DA.SetDataList(1, output);
            DA.SetDataList(2, indicies);
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
            get { return new Guid("28e8c6e2-f6b9-4139-9ffc-f389bb211f7c"); }
        }
    }
}