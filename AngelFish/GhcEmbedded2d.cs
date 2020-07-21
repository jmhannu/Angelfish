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
            pManager.AddNumberParameter("Varible combinations", "Varibles", "dA, dB, f, k", GH_ParamAccess.list);
            pManager.AddNumberParameter("Mass percentage", "Mass", "Mass percentage", GH_ParamAccess.item);
            pManager.AddNumberParameter("Connected percentage", "Connectivity", "Connected percentage", GH_ParamAccess.item);
            pManager.AddNumberParameter("Solid edges percentage", "Solid edge", "Solid edge percentage", GH_ParamAccess.item);
            pManager.AddNumberParameter("Preview", "Preview", "Preview", GH_ParamAccess.list);
            pManager.AddTextParameter("PrintOut", "PrintOut", "PrintOut", GH_ParamAccess.list);
            pManager.AddTextParameter("PrintOne", "PrintOne", "PrintOne", GH_ParamAccess.item);
            pManager.AddNumberParameter("PrintNumber", "PrintNumber", "PrintNumber", GH_ParamAccess.list);
            pManager.AddTextParameter("PrintTree", "PrintTree", "PrintTree", GH_ParamAccess.tree);
            pManager.AddTextParameter("Name", "Name", "Name", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            ReadWrite readValues = new ReadWrite();
            string file = File.ReadAllText("C:/Users/julia/OneDrive/Dokument/GitHub/Angelfish/Angelfish/Resources/Embedded2D.txt");


            readValues.Read(file);

            int index = 0;
            DA.GetData(0, ref index);

            List<GH_Number> varibles = readValues.Varibles.get_Branch(index) as List<GH_Number>;

            DA.SetDataList(0, varibles);
            DA.SetDataList(1, readValues.MassPercentage);
            DA.SetDataList(3, readValues.ConnectedPercentage);
            DA.SetDataList(2, readValues.SolidEdgePercentage);

            DA.SetDataList(4, readValues.ToPreview(readValues.Names[index]));

            DA.SetDataList("PrintOut", readValues.printOut);
            DA.SetData("PrintOne", readValues.printOne);
            DA.SetDataList("PrintNumber", readValues.printNumber);
            DA.SetDataTree(8, readValues.printStructure);
            DA.SetData("Name", readValues.Names[index]);
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