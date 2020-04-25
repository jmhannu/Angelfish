using System;
using System.Collections.Generic;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace Angelfish
{
    public class GhcCalculateRD : GH_Component
    {
        ReactionDiffusion reactDiffuse;
        bool first = true; 

        public GhcCalculateRD()
          : base("CalculateRD", "RD",
              "Calulates Grey Scott Reaction Diffusion on a set of points or vertices",
              "Angelfish", "Calculate")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "Mesh", "Mesh", GH_ParamAccess.item);
            pManager.AddNumberParameter("Varibles", "Varibles", "dA, dB, k, f", GH_ParamAccess.tree);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {            
            pManager.AddPointParameter("Solid", "Solid", "Solid or black points", GH_ParamAccess.list);
            pManager.AddPointParameter("Void", "Void", "Void or white points", GH_ParamAccess.list);
            pManager.AddNumberParameter("PrintOut", "PrintOut", "PrintOut", GH_ParamAccess.item);
             pManager.AddNumberParameter("Neighbours", "Neighbours", "Neighbours", GH_ParamAccess.tree);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if(first)
            {
                Mesh mesh = null;
                DA.GetData("Mesh", ref mesh);

                GH_Structure<GH_Number> tree;
                DA.GetDataTree(1, out tree);

                reactDiffuse = new ReactionDiffusion(tree, mesh);

                first = false;
            }

            reactDiffuse.Update();

            DA.SetDataList(0, reactDiffuse.points);
            DA.SetDataList(1, reactDiffuse.points);
            DA.SetData(2, reactDiffuse.PrintOut());
            DA.SetDataTree(3, reactDiffuse.neighbours);
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
            get { return new Guid("164e7e51-a496-4307-bb63-8e2bbd89541f"); }
        }
    }
}