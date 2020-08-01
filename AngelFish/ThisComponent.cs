using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;

namespace thisComponent
{
    public class ThisComponent : GH_Component, IGH_VariableParameterComponent
    {
        #region fields
        private int _outputCounter;
        #endregion

        #region constructor code
        public ThisComponent()
          : base("ThisComponent", "TC", "ThisComponent", "ThisComponent", "ThisComponent")
        {
            _outputCounter = 2;
            FixOutputParameters();
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            // All parameters are managed via the IGH_VariableParameterComponent implementation.
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            // All parameters are managed via the IGH_VariableParameterComponent implementation.
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("E8AD8EC8-F1B7-4B30-BD99-61323FBE4E49"); }
        }
        #endregion

        #region variable parameter implementation
        // Note: the variable parameters are maintained entirely through custom code and the user gets no UI.
        bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, Int32 index)
        {
            return false;
        }
        bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, Int32 index)
        {
            return false;
        }
        bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, Int32 index)
        {
            return false;
        }
        IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, Int32 index)
        {
            return null;
        }

        /// <summary>
        /// Make sure all output parameters have correct settings.
        /// </summary>
        public void VariableParameterMaintenance()
        {
            for (int i = 0; i < Params.Output.Count; i++)
            {
                IGH_Param param = Params.Output[i];
                param.Access = GH_ParamAccess.item;
                param.MutableNickName = false;
                param.NickName = (i + 1).ToString();
                param.Name = "Output " + param.NickName;
                param.Description = "Output for data stream " + param.NickName;
            }
        }

        /// <summary>
        /// Make sure the number of output params is identical to _outputCounter. 
        /// This method may never be called during solutions.
        /// </summary>
        public void FixOutputParameters()
        {
            while (true)
            {
                int currentCount = Params.Output.Count;
                if (currentCount == _outputCounter)
                    break;

                if (_outputCounter < 0 && currentCount == 0)
                    break;

                if (currentCount < _outputCounter)
                    Params.RegisterOutputParam(new Param_GenericObject());

                if (currentCount > _outputCounter)
                    Params.UnregisterOutputParameter(Params.Output[Params.Output.Count - 1], true);
            }
            Params.OnParametersChanged();
            VariableParameterMaintenance();
        }
        #endregion

        #region solution code
        public override void ExpireSolution(bool recompute)
        {
            // Here may be a good time to mess with the component topology.
            FixOutputParameters();
            base.ExpireSolution(recompute);
        }

        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            Menu_AppendItem(menu, "Randomize output", RandomizeOutputClicked);
        }
        private void RandomizeOutputClicked(object sender, EventArgs e)
        {
            Random random = new Random();

            while (true)
            {
                int newCount = random.Next(1, 11);
                if (newCount == _outputCounter)
                    continue;

                _outputCounter = newCount;
                break;
            }

            ExpireSolution(true);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Complete Variable Declaration
            List<string> nameVariable = new List<string>();
            List<string> dataVariable = new List<string>();

            // A temporary process for this demo to fill name and data variables for param.
            for (int i = 0; i < _outputCounter; i++)
            {
                nameVariable.Add(i.ToString());
                dataVariable.Add("Some string data for " + i);
            }

            // With params set fill properties of params here and send data to canvas.
            for (int i = 0; i < Params.Output.Count; i++)
                DA.SetData(i, dataVariable[i]);
        }
        #endregion
    }
}