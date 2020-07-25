using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Angelfish
{
    public class Apoint
    {
        // public GH_Point Pos { get; set; }
        public Point3d Pos { get;}

        public double Da { get; set; }
        public double Db { get; set; }
        public double K { get; set; }
        public double F { get; set; }

        public int Index { get; set; }

        public bool InPattern;
        

        public List<int> Neighbours { get; set; }
        public List<int> SecoundNeighbours { get; set; }
        public List<double> Weights { get; set; }

        public  Apoint(Point3d _point, List<double> _values)
        {
            Pos = _point;

            Da = _values[0];
            Db = _values[1];
            F = _values[2];
            K = _values[3];
            InPattern = false;

            Neighbours = new List<int>();
            SecoundNeighbours = new List<int>();
            Weights = new List<double>();
        }

        public Apoint(Point3d _point)
        {
            Pos = _point;

            Da = 0.0;
            Db = 0.0;
            F = 0.0;
            K = 0.0;
            InPattern = false;

            Neighbours = new List<int>();
            SecoundNeighbours = new List<int>();
            Weights = new List<double>();
        }

        public Apoint(Point3d _point, int _index)
        {
            Pos = _point;

            Da = 0.0;
            Db = 0.0;
            F = 0.0;
            K = 0.0;
            Index = _index;

            InPattern = false;

            Neighbours = new List<int>();
            SecoundNeighbours = new List<int>();
            Weights = new List<double>();
        }

    }
}