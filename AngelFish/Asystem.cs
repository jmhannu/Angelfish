using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace Angelfish
{
    public class Asystem
    {
        public List<Apoint> Apoints;
        public int asize;

        public Asystem(List<Apoint> _points)
        {
            Apoints = new List<Apoint>();

            for (int i = 0; i < _points.Count; i++)
            {
                Apoints.Add(_points[i]);
            }

            asize = Apoints.Count;
        }

        public Asystem(List<double> _values, List<Point3d> _points)
        {
            Apoints = new List<Apoint>();

            for (int i = 0; i < _points.Count; i++)
            {
                Apoints.Add(new Apoint(_points[i], _values));
            }

            asize = Apoints.Count;
            FindNeighbours(false, 0.0);
        }

        //public Asystem(GH_Structure<GH_Number> _values, List<Point3d> _points)
        //{
        //    Apoints = new List<Apoint>();

        //    for (int i = 0; i < _points.Count; i++)
        //    {
        //        Apoints.Add(new Apoint(_points[i], _values[i]));
        //    }

        //    asize = Apoints.Count;
        //    FindNeighbours(false, 0.0);
        //}

        public Asystem(List<double> _values, Mesh _mesh)
        {
            Apoints = new List<Apoint>();

            for (int i = 0; i < _mesh.Vertices.Count; i++)
            {
                
                Apoints.Add(new Apoint(_mesh.Vertices[i], _values));
            }

            asize = Apoints.Count;

            int[] temp = _mesh.Vertices.GetConnectedVertices(0);
            double distance = _mesh.Vertices[0].DistanceTo(_mesh.Vertices[temp[0]]);

            FindNeighbours(true, distance);

        }

        //public Asystem(GH_Structure<GH_Number> _values, Mesh _mesh)
        //{
        //    Apoints = new List<Apoint>();

        //    for (int i = 0; i < _mesh.Vertices.Count; i++)
        //    {

        //        Apoints.Add(new Apoint(_mesh.Vertices[i], _values[i]));
        //    }

        //    asize = Apoints.Count;

        //    int[] temp = _mesh.Vertices.GetConnectedVertices(0);
        //    double distance = _mesh.Vertices[0].DistanceTo(_mesh.Vertices[temp[0]]);

        //    FindNeighbours(true, distance);
        //}

        void FindNeighbours(bool _byDistance, double _distance)
        {


            if (_byDistance)
            {
                ByDistance(_distance);
            }

            else
            {
                ByCount(8);
            }
        }

        void ByDistance(double _distance)
        {
            RTree rTree = new RTree();

            for (int i = 0; i < asize; i++)
            {
                rTree.Insert(Apoints[i].Pos, i);
            }

            double searchDistance = _distance + (_distance / 2);

            for (int i = 0; i < asize; i++)
            {
                Point3d vI = Apoints[i].Pos;
                Sphere searchSpehere = new Sphere(vI, searchDistance);

                List<int> near = new List<int>();

                rTree.Search(searchSpehere,
                    (sender, args) => { if (i != args.Id) near.Add(args.Id); }
                    );

                for (int j = 0; j < near.Count; j++)
                {
                    if (vI.DistanceTo(Apoints[near[j]].Pos) <= _distance + (_distance * 0.1)) Apoints[i].Neighbours.Add(near[j]);
                    else Apoints[i].SecoundNeighbours.Add(near[j]);
                }
            }
        }

        void ByCount(int nr)
        {
            List<Point3d> allPoints = new List<Point3d>();
            for (int i = 0; i < asize; i++)
            {
                allPoints.Add(Apoints[i].Pos);
            }


                int[][] found = RTree.Point3dKNeighbors(allPoints, allPoints, 8).ToArray();

            for (var i = 0; i < found.Length; i++)
            {
                for (int j = 0; j < found[i].Length; ++j)
                {
                    Apoints[i].Neighbours.Add(found[i][j]);
                }
            }
        }


    }

    public struct Apoint
    {
        // public GH_Point Pos { get; set; }
        public Point3d Pos { get; set; }

        public double Da { get; set; }
        public double Db { get; set; }
        public double K { get; set; }
        public double F { get; set; }


        public List<int> Neighbours { get; set; }
        public List<int> SecoundNeighbours { get; set; }
        public List<double> Weights { get; set; }

        public Apoint(Point3d _point,  double _dA, double _dB, double _f, double _k)
        {
            Pos = _point; 

            Da = _dA;
            Db = _dB;
            F = _f;
            K = _k;

            Neighbours = new List<int>();
            SecoundNeighbours = new List<int>();
            Weights = new List<double>();
        }

        public Apoint(Point3d _point, List<double> _values)
        {
            Pos = _point;

            Da = _values[0];
            Db = _values[1];
            F = _values[2];
            K = _values[3];

            Neighbours = new List<int>();
            SecoundNeighbours = new List<int>();
            Weights = new List<double>();
        }
    }
}
