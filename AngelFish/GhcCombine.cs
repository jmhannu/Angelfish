using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;
using Grasshopper;
using System.Drawing;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;

namespace Angelfish
{
    #region Methods of GH_Component interface

    public class GhcCombine : GH_Component, IGH_VariableParameterComponent
    {

        public GhcCombine()
              : base("Combine", "Combine",
              "Combine regions into one mesh",
              "Angelfish", "1.Setup")
        {
            Params.ParameterSourcesChanged += AddInput;
        }

        private void AddInput(Object _sender, GH_ParamServerEventArgs _eventargs)
        {
            if (_eventargs.ParameterSide == GH_ParameterSide.Input)
            {
                if (Params.Input.Last().Sources.Any())
                {
                    IGH_Param toAdd = CreateParameter(GH_ParameterSide.Input, Params.Input.Count);
                    Params.RegisterInputParam(toAdd, Params.Input.Count);
                    VariableParameterMaintenance();
                    this.Params.OnParametersChanged();
                }
            }
        }
        protected override Bitmap Internal_Icon_24x24
        {
            get
            {
                return null;
            }
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "Mesh", "Mesh", GH_ParamAccess.item);
            pManager.AddGenericParameter("A", "A", "Apoint from region, zoom to add more inputs", GH_ParamAccess.item);

        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Gradient", "Gradient", "Gradient", GH_ParamAccess.item);
            pManager.AddColourParameter("Colours", "Colours", "Colours", GH_ParamAccess.list);
        }

       
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh mesh = null;
            DA.GetData(0, ref mesh);

            List<Pattern> regions = new List<Pattern>();
            Asystem aux = new Asystem();

            for (int i = 1; i < Params.Input.Count; i++)
            {
                if (DA.GetData(i, ref aux))
                {
                    regions.Add(new Pattern(aux));
                }
            }

            Gradient gradient = new Gradient(regions, mesh);

            DA.SetData(0, gradient);
            DA.SetDataList(1, gradient.colors);
        }



        public override Guid ComponentGuid
        {
            get { return new Guid("831F08AB-044A-4897-A936-E30528ADA4F9"); }
        }
        #endregion

        #region Methods of IGH_VariableParameterComponent interface

        public bool CanInsertParameter(GH_ParameterSide side, int index)
        {
            if (side == GH_ParameterSide.Input)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CanRemoveParameter(GH_ParameterSide side, int index)
        {
            if (side == GH_ParameterSide.Input && Params.Input.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public IGH_Param CreateParameter(GH_ParameterSide side, int index)
        {
            Param_GenericObject param = new Param_GenericObject();
            param.Name = GH_ComponentParamServer.InventUniqueNickname("BCDEFGHIJKLMNOPQRSTUVWXYZ", Params.Input);
            param.NickName = param.Name;
            param.Description = "Param" + (Params.Input.Count + 1);
            param.Optional = true;

            return param;
        }

        public bool DestroyParameter(GH_ParameterSide side, int index)
        {
            return true;
        }

        public void VariableParameterMaintenance()
        {
            for (int i = 0; i < Params.Input.Count; i++)
            {
                Params.Input[i].Access = GH_ParamAccess.item;

            }
        }

        #endregion

    }
}
