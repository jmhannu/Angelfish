using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Utility;
using Rhino.Geometry;

namespace Angelfish
{
    public class Asystem
    {
        public List<Apoint> Apoints;
        public int asize;
        public int edgeCount;
        public Point3d min;
        public Point3d max;
        public List<int> Pattern;

        bool excludeX;
        bool excludeY;
        bool excludeZ;


        public Asystem(List<double> _values, Mesh _mesh, List<int> _indicies)
        {
            Apoints = new List<Apoint>();

            for (int i = 0; i < _mesh.Vertices.Count; i++)
            {
                Apoints.Add(new Apoint(_mesh.Vertices[i]));
            }

            for (int i = 0; i < _indicies.Count; i++)
            {
                Apoints[_indicies[i]].Da = _values[0];
                Apoints[_indicies[i]].Db = _values[1];
                Apoints[_indicies[i]].F = _values[2];
                Apoints[_indicies[i]].K = _values[3];
            }

            asize = Apoints.Count;

            excludeX = false;
            excludeY = false;
            excludeZ = false;

            MinMax();

            edgeCount = CountEdge();

            int[] temp = _mesh.Vertices.GetConnectedVertices(0);
            double distance = _mesh.Vertices[0].DistanceTo(_mesh.Vertices[temp[0]]);

            FindNeighbours(true, distance);
        }

        public Asystem(List<double> _values, Mesh _mesh)
        {
            Apoints = new List<Apoint>();

            for (int i = 0; i < _mesh.Vertices.Count; i++)
            {

                Apoints.Add(new Apoint(_mesh.Vertices[i], _values));
            }
            asize = Apoints.Count;

            excludeX = false;
            excludeY = false;
            excludeZ = false;

            MinMax();

            edgeCount = CountEdge();

            int[] temp = _mesh.Vertices.GetConnectedVertices(0);
            double distance = _mesh.Vertices[0].DistanceTo(_mesh.Vertices[temp[0]]);

            FindNeighbours(true, distance);
        }

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

        void MinMax()
        {
            min = new Point3d(double.MaxValue, double.MaxValue, double.MaxValue);
            max = new Point3d(double.MinValue, double.MinValue, double.MinValue);

            for (int i = 0; i < Apoints.Count; i++)
            {
                if (max.X < Apoints[i].Pos.X) max.X = Apoints[i].Pos.X;
                if (max.Y < Apoints[i].Pos.Y) max.Y = Apoints[i].Pos.Y;
                if (max.Z < Apoints[i].Pos.Z) max.Z = Apoints[i].Pos.Z;

                if (min.X > Apoints[i].Pos.X) min.X = Apoints[i].Pos.X;
                if (min.Y > Apoints[i].Pos.Y) min.Y = Apoints[i].Pos.Y;
                if (min.Z > Apoints[i].Pos.Z) min.Z = Apoints[i].Pos.Z;
            }

            if (min.X == max.X) excludeX = true;
            if (min.Y == max.Y) excludeY = true;
            if (min.Z == max.Z) excludeZ = true;
        }
        int CountEdge()
        {
            int count = 0;

            for (int i = 0; i < Apoints.Count; i++)
            {
                if ((!excludeX && (Apoints[i].Pos.X == max.X || Apoints[i].Pos.X == min.X)) ||
                (!excludeY && (Apoints[i].Pos.Y == max.Y || Apoints[i].Pos.Y == min.Y)) ||
                (!excludeZ && (Apoints[i].Pos.Z == max.Z || Apoints[i].Pos.Z == min.Z)))
                {
                    count++;
                }
            }

            return count;
        }
    }
}


