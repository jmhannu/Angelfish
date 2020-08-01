using System;
using System.Collections.Generic;
using GH_IO.Types;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Angelfish
{
    public class GhcRegion : GH_Component
    {
        public GhcRegion()
          : base("Region", "Region",
              "Set varibles to region",
              "Angelfish", "1.Setup")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "Mesh", "Mesh", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Cull pattern", "Cull", "Cull pattern, list of true/false of the same length as the number of mesh verticies", GH_ParamAccess.list);
            pManager.AddNumberParameter("Values", "Values", "Values", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Region", "Region", "Region", GH_ParamAccess.list);
            pManager.AddPointParameter("Points", "Points", "Points", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Indicies", "Indicies", "Indicies", GH_ParamAccess.list);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {

            Mesh mesh = null;
            DA.GetData(0, ref mesh);

            List<bool> cullPattern = new List<bool>();
            DA.GetDataList(1, cullPattern);

            List<double> values = new List<double>();
            DA.GetDataList(2, values);


            List<Apoint> apoints = new List<Apoint>();

            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                if(cullPattern[i]) apoints.Add(new Apoint(mesh.Vertices[i], i, values));
            }

            List<Point3d> output = new List<Point3d>();
            List<int> indicies = new List<int>();
                
            for (int i = 0; i < apoints.Count; i++)
            {
                output.Add(apoints[i].Pos);
                indicies.Add(apoints[i].Index);
            }

            DA.SetDataList(0, apoints);
            DA.SetDataList(1, output);
            DA.SetDataList(2, indicies);
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
            get { return new Guid("3d18152a-b4f5-4d5c-bfad-a16ffd55bede"); }
        }
    }
}