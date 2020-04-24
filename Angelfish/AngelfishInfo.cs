using System;

using System.Drawing;
using Grasshopper.Kernel;

namespace Angelfish
{
    public class AngelfishInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "Angelfish";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                return "Create 2D or 3D RD patterning on geometry";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("cd580808-e54b-481b-8170-88b45a29aa08");
            }
        }

        public override string AuthorName
        {
            get
            {
                return "Julia Hannu";
            }
        }
        public override string AuthorContact
        {
            get
            {
                return "julia_hannu@hotmail.com";
            }
        }
    }
}
