using System;
using System.Drawing;
using System.Collections.Generic;
using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Angelfish
{
    public class Gradient : Asystem
    {
        int regionCount;
        public List<Color> colors;
        public List<Point3d> regions; 

        public Gradient(DataTree<Apoint> _apoints, Mesh _mesh)
        {
            colors = new List<Color>();

            regionCount = _apoints.BranchCount;
            asize = _mesh.Vertices.Count;
            regions = new List<Point3d>();
            
            Apoints = new List<Apoint>(); 


            for (int i = 0; i < _mesh.Vertices.Count; i++)
            {
                    Apoints.Add(new Apoint(_mesh.Vertices[i]));
                    colors.Add(Color.White);
            }

            List<Apoint> all = _apoints.AllData();
            int allCount = all.Count;

            for (int i = 0; i < allCount; i++)
            {
                Apoints[all[i].Index] = all[i];
                colors[all[i].Index] = Color.Red;
            }
        }
    }
}