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
        List<Apoint> Apoints;
        int asize;

        public Asystem(List<GH_Number> _values, List<Point3d> _points)
        {
            Apoints = new List<Apoint>();

            for (int i = 0; i < _points.Count; i++)
            {
                Apoints.Add(new Apoint(_points[i], _values));
            }

            asize = Apoints.Count;
        }

        public Asystem(GH_Structure<GH_Number> _values, List<Point3d> _points)
        {
            Apoints = new List<Apoint>();

            for (int i = 0; i < _points.Count; i++)
            {
                Apoints.Add(new Apoint(_points[i], _values[i]));
            }

            asize = Apoints.Count;
        }

        public Asystem(List<GH_Number> _values, Mesh _mesh)
        {
            Apoints = new List<Apoint>();

            for (int i = 0; i < _mesh.Vertices.Count; i++)
            {
                
                Apoints.Add(new Apoint(_mesh.Vertices[i], _values));
            }

            asize = Apoints.Count;

            int[] temp = _mesh.Vertices.GetConnectedVertices(0);
            double distance = _mesh.Vertices[0].DistanceTo(_mesh.Vertices[temp[0]]);

            FindNeighbours(distance);

        }

        public Asystem(GH_Structure<GH_Number> _values, Mesh _mesh)
        {
            Apoints = new List<Apoint>();

            for (int i = 0; i < _mesh.Vertices.Count; i++)
            {

                Apoints.Add(new Apoint(_mesh.Vertices[i], _values[i]));
            }

            asize = Apoints.Count;

            int[] temp = _mesh.Vertices.GetConnectedVertices(0);
            double distance = _mesh.Vertices[0].DistanceTo(_mesh.Vertices[temp[0]]);

            FindNeighbours(distance);
        }

        public void FindNeighbours(double _distance)
        {
            double searchDistance = _distance + (_distance / 2);

            GH_Structure<GH_Number> neighbours = new GH_Structure<GH_Number>();
            GH_Structure<GH_Number> secoundNeighbours = new GH_Structure<GH_Number>();

            RTree rTree = new RTree();

            for (int i = 0; i < asize; i++)
            {
                rTree.Insert(Apoints[i].Pos, i);
            }

            for (int i = 0; i < asize; i++)
            {
                Point3d vI = Apoints[i].Pos;
                Sphere searchSpehere = new Sphere(vI, searchDistance);

                List<int> near = new List<int>();

                rTree.Search(searchSpehere,
                    (sender, args) => { if (i != args.Id) near.Add(args.Id); }
                    );

                List<GH_Number> first = new List<GH_Number>();
                List<GH_Number> secound = new List<GH_Number>();

                for (int j = 0; j < near.Count; j++)
                {
                    if (vI.DistanceTo(Apoints[near[j]].Pos) <= _distance + (_distance * 0.1)) first.Add(new GH_Number(near[j]));
                    else secound.Add(new GH_Number(near[j]));
                }

                neighbours.AppendRange(first, new GH_Path(i));
                secoundNeighbours.AppendRange(secound, new GH_Path(i));
            }
        }

        //------------------------- CURRENTLY NOT USED -----------------------------------
        //private void FindMeshNeighbours(Mesh _mesh)
        //{
        //    GH_Structure<GH_Number> neighbours = new GH_Structure<GH_Number>();

        //    for (int i = 0; i < _mesh.Vertices.Count; i++)
        //    {
        //        int[] near = _mesh.Vertices.GetConnectedVertices(i);

        //        List<GH_Number> tempList = new List<GH_Number>();

        //        for (int j = 0; j < near.Length; j++)
        //        {
        //            tempList.Add(new GH_Number(near[j]));
        //        }

        //        neighbours.AppendRange(tempList, new GH_Path(i));
        //    }
        //}
        //-------------------------------------------------------------------------------



    }

    struct Apoint
    {
        // public GH_Point Pos { get; set; }
        public Point3d Pos { get; set; }

        public double Da { get; set; }
        public double Db { get; set; }
        public double K { get; set; }
        public double F { get; set; }


        public GH_Structure<GH_Number> neighbours;
        public GH_Structure<GH_Number> secoundNeighbours;

        public Apoint(Point3d _point,  double _dA, double _dB, double _f, double _k)
        {
            Pos = _point; 

            Da = _dA;
            Db = _dB;
            F = _f;
            K = _k;

            neighbours = new GH_Structure<GH_Number>();
            secoundNeighbours = new GH_Structure<GH_Number>();
        }

        public Apoint(Point3d _point, List<GH_Number> _values)
        {
            Pos = _point;

            Da = _values[0].Value;
            Db = _values[1].Value;
            F = _values[2].Value;
            K = _values[3].Value;

            neighbours = new GH_Structure<GH_Number>();
            secoundNeighbours = new GH_Structure<GH_Number>();
        }
    }
}
