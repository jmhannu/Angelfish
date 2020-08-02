using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace Angelfish
{
    public class GhpPreview : GH_Param<GH_Number>
    {
        public GhpPreview()
          : base("Preview Pattern", "Preview",
              "Preview Pattern",
              "Angelfish", "4. Utility", GH_ParamAccess.list)
        {
        }

        protected override void OnVolatileDataCollected()
        {
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("0da73e9d-5309-4855-a077-951642a2e3a7"); }
        }

        public override void CreateAttributes()
        {
            {
                this.m_attributes = (IGH_Attributes)new AttributePreview(this);
            }
        }
    }
}