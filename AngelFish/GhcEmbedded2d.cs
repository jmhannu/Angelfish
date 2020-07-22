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
    public class GhcEmbedded2d : GH_Component
    {
        public GhcEmbedded2d()
          : base("Embedded Patterns 2D", "Patterns 2D",
              "Embedded varibles and measures for 2D patterns on meshes or 2D grids of points",
              "Angelfish", "0.Varibles")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("Index", "Index", "Index", GH_ParamAccess.item, 0);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Measured varibles", "Measured", "Measured varibles", GH_ParamAccess.item);
            pManager.AddNumberParameter("Varible combinations", "Varibles", "dA, dB, f, k", GH_ParamAccess.list);
            pManager.AddNumberParameter("Mass percentage", "Mass", "Mass percentage", GH_ParamAccess.item);
            pManager.AddNumberParameter("Connected percentage", "Connectivity", "Connected percentage", GH_ParamAccess.item);
            pManager.AddNumberParameter("Solid edges percentage", "Solid edge", "Solid edge percentage", GH_ParamAccess.item);
            pManager.AddNumberParameter("Preview", "Preview", "Preview", GH_ParamAccess.list);
            pManager.AddNumberParameter("Count", "Count", "Count", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            ReadWrite readValues = new ReadWrite();
            string file = File.ReadAllText("C:/Users/julia/OneDrive/Dokument/GitHub/Angelfish/Angelfish/Resources/Embedded2D.txt");

            readValues.Read(file);

            int index = 0;
            DA.GetData(0, ref index);

            List<GH_Number> varibles = readValues.Varibles.get_Branch(index) as List<GH_Number>;

            DA.SetData(0, readValues);
            DA.SetDataList(1, varibles);
            DA.SetData(2, readValues.MassPercentage[index]);
            DA.SetData(3, readValues.ConnectedPercentage[index]);
            DA.SetData(4, readValues.SolidEdgePercentage[index]);

            DA.SetDataList(5, readValues.ToPreview(readValues.Names[index]));
            DA.SetData(6, readValues.Varibles.PathCount);
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
        public override Guid ComponentGuid
        {
            get { return new Guid("a7c10145-1337-401e-8050-1f4bd122400d"); }
        }
    }
}