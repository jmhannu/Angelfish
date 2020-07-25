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
            pManager.AddGenericParameter("A", "A", "A", GH_ParamAccess.list);

        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Asystem", "Asystem", "Asystem", GH_ParamAccess.item);

        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh mesh = null;
            DA.GetData(0, ref mesh);

            double result = 0, aux = 0;
            for (int i = 1; i < Params.Input.Count-1; i++)
            {
                if (DA.GetData(i, ref aux))
                {
                    result += aux;
                }
            }
            DA.SetData(0, result);

        }

        public override Guid ComponentGuid
        {
            get { return new Guid("831F08AB-044A-4897-A936-E30528ADA4F9"); }
        }
        #endregion

        #region Methods of IGH_VariableParameterComponent interface

        bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index)
        {
            //We only let input parameters to be added (output number is fixed at one)
            if (side == GH_ParameterSide.Input)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index)
        {
            //We can only remove from the input
            if (side == GH_ParameterSide.Input && Params.Input.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index)
        {
            Param_Number param = new Param_Number();

            List<string> names = new List<string>();

            param.Name = GH_ComponentParamServer.InventUniqueNickname("BCDEFGHIJKLMNOPQRSTUVWXYZ", Params.Input);
            param.NickName = param.Name;
            param.Description = "Param" + (Params.Input.Count + 1);
            param.SetPersistentData(0.0);

            return param;
        }

        bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index)
        {
            //Nothing to do here by the moment
            return true;
        }

        void IGH_VariableParameterComponent.VariableParameterMaintenance()
        {
            //Nothing to do here by the moment
        }

        #endregion

    }


}
