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
    public class GhcValuesFor2D : GH_Component
    {
        static string path = "C:/Users/julia/OneDrive/Dokument/GitHub/Angelfish/AngelFish/Resources/inputs2D.txt";
       // static string path = "/Data/inputs2D.txt";
        static string file = File.ReadAllText(path);
        Values values = new Values(file, false);
        //  static string current = System.AppDomain.CurrentDomain.BaseDirectory;

        static GH_AssemblyInfo info = Grasshopper.Instances.ComponentServer.FindAssembly(new Guid("cd580808-e54b-481b-8170-88b45a29aa08"));
        static string current = info.Location;

        public GhcValuesFor2D()
          : base("Values for 2D", "Values for 2D",
              "Values to use for 2D reaction diffusion",
              "Angelfish", "Setup")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
             pManager.AddIntegerParameter("Index", "Index", "Index", GH_ParamAccess.item, 0);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {

            //pManager.AddNumberParameter("Varible combinations", "Varibles", "dA, dB, f, k", GH_ParamAccess.tree);
            pManager.AddNumberParameter("Varible combinations", "Varibles", "dA, dB, f, k", GH_ParamAccess.list);
            pManager.AddTextParameter("PrintOut", "PrintOut", "PrintOut", GH_ParamAccess.item);


            //  pManager.AddNumberParameter("Count combinations", "Count", "Number of varible combinations", GH_ParamAccess.item);
            //  pManager.AddNumberParameter("Mass procentages", "Mass", "Mass procentage per pattern", GH_ParamAccess.list);
            //  pManager.AddNumberParameter("Solid edges", "Solid edge", "Solid edge percentages", GH_ParamAccess.list);
            // pManager.AddNumberParameter("Connectivity rate", "Connectivity", "Connectivity rate", GH_ParamAccess.list);
            //pManager.AddNumberParameter("Edge connections", "Edge connect", "Percentages of parts with connetion to the edge", GH_ParamAccess.list);
            //pManager.AddNumberParameter("Select Index", "Select i", "Heighest/lowest mass index", GH_ParamAccess.list);
            //  pManager.AddNumberParameter("Weighted Number", "Weighted", "Weighted Number", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //string[] lines = file.Split('\n');

            //foreach (string line in lines)
            //{
            //    string fixedLine = line.Replace(',', '.');
            //    string[] lineValues = fixedLine.Split('\t');

            //    dAValues.Add(Convert.ToDouble(lineValues[0]));
            //    dBValues.Add(Convert.ToDouble(lineValues[1]));
            //    fValues.Add(Convert.ToDouble(lineValues[2]));
            //    kValues.Add(Convert.ToDouble(lineValues[3]));

            //}

            int index = 0;
            DA.GetData(0, ref index);


            List<GH_Number> varibles = values.varibles.get_Branch(index) as List<GH_Number>;

            DA.SetDataList(0, varibles);


            DA.SetData(1, current);

            //DA.SetData(0, (double)values.ValuesCount);
            //  DA.SetDataTree(0, values.Varibles);
            //DA.SetDataList(2, values.MassProcentages);
            //DA.SetDataList(3, values.SolidEdgeProcentage);
            //DA.SetDataList(4, values.ConnectedProcentages);
            //DA.SetDataList(5, values.EdgeConnectionProcentages);

            //double weightMass, weightConnection, weightEdgeConnection, weightSolidEdge;
            //weightMass = weightConnection = weightEdgeConnection = weightSolidEdge = 1.0;
            //double addRange = 0.0;

            //DA.GetData(0, ref weightMass);
            //DA.GetData(1, ref weightConnection);
            //DA.GetData(2, ref weightEdgeConnection);
            //DA.GetData(3, ref weightSolidEdge);
            //DA.GetData(4, ref addRange);

            //DA.SetDataList(6, values.SelectIndex(weightMass, weightConnection, weightEdgeConnection, weightSolidEdge, addRange));
            //DA.SetData(7, values.WeightedValue);
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
            get { return new Guid("47aa0cce-98d4-4762-9cdf-7e2fc73cd5bc"); }
        }
    }
}