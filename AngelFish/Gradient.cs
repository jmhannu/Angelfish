using System;
using System.Drawing;
using System.Collections.Generic;
using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Angelfish
{
    public class Gradient : Pattern
    {
        public List<Color> colors;
        public List<Pattern> regions;
        public List<bool> inRegion; 

        public Gradient(List<Pattern> _regions, Mesh _mesh)
        {
            colors = new List<Color>();
            regions = _regions;
            Apoints = new List<Apoint>();
            inRegion = new List<bool>();

            for (int i = 0; i < _mesh.Vertices.Count; i++)
            {
                Apoints.Add(new Apoint(_mesh.Vertices[i]));
                colors.Add(Color.White);
                inRegion.Add(false);
            }

            for (int i = 0; i < regions.Count; i++)
            {
                for (int j = 0; j < regions[i].Apoints.Count; j++)
                {
                    Apoints[regions[i].Apoints[j].Index] = regions[i].Apoints[j];
                    colors[regions[i].Apoints[j].Index] = Color.Red;
                    inRegion[regions[i].Apoints[j].Index] = true;
                }
            }

            InitAll();
        }
    }
}