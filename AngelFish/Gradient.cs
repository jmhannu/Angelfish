using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Geometry.Delaunay;
using Grasshopper.Kernel.Types;
using Rhino.Geometry.Collections;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
using Grasshopper;
using System.Runtime.InteropServices;

namespace Angelfish
{
    public class Gradient : Pattern
    {
        public List<Color> colors;
        public List<Region> regions;
        public List<bool> inRegion;
        public List<Point3d> pointsInHull;
        public List<Point3d> pointsInbetween;
        public List<Line> pairings;
        public List<Point3d> pointsOnEdge;
        public List<Polyline> regionHulls;

        public Gradient(List<Region> _regions, Mesh _mesh)
        {
            colors = new List<Color>();
            regions = _regions;
            Apoints = new List<Apoint>();
            inRegion = new List<bool>();

            for (int i = 0; i < _mesh.Vertices.Count; i++)
            {
                Apoints.Add(new Apoint(_mesh.Vertices[i], i));
                colors.Add(Color.White);
                inRegion.Add(false);
            }

            mesh = _mesh;

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

        bool IsInside(Point3d pt, Polyline crv)
        {
            Point3d pt1, pt2;
            bool oddNodes = false;

            for (int i = 0; i < crv.SegmentCount; i++) //for each contour line
            {

                pt1 = crv.SegmentAt(i).From; //get start and end pt
                pt2 = crv.SegmentAt(i).To;

                if ((pt1[1] < pt[1] && pt2[1] >= pt[1] || pt2[1] < pt[1] && pt1[1] >= pt[1]) && (pt1[0] <= pt[0] || pt2[0] <= pt[0])) //if pt is between pts in y, and either of pts is before pt in x
                    oddNodes ^= (pt2[0] + (pt[1] - pt2[1]) * (pt1[0] - pt2[0]) / (pt1[1] - pt2[1]) < pt[0]); //^= is xor
                                                                                                             //end.X + (pt-end).Y   * (start-end).X  /(start-end).Y   <   pt.X
            }

            if (!oddNodes)
            {
                double minDist = 1e10;
                for (int i = 0; i < crv.SegmentCount; i++)
                {
                    Point3d cp = crv.SegmentAt(i).ClosestPoint(pt, true);
                    minDist = Math.Min(minDist, cp.DistanceTo(pt));
                }
                if (minDist < 1e-10)
                    return true;
            }

            if (oddNodes) return true;

            return false;
        }

        Point3d GetCentroid(List<Apoint> _apoints, int _regionI)
        {
            int cordinates = 3;

            double[] centroid = new double[cordinates];

            foreach (Apoint point in _apoints)
            {
                centroid[0] += point.Pos.X;
                centroid[1] += point.Pos.Y;
                centroid[2] += point.Pos.Z;

                point.regionIndex = _regionI;
            }

            for (int i = 0; i < cordinates; i++)
            {
                centroid[i] /= _apoints.Count;
            }

            return new Point3d(centroid[0], centroid[1], centroid[2]);
        }

        List<bool> CheckHull(Polyline _hull)
        {
            pointsOnEdge = new List<Point3d>();
            List<bool> inHull = new List<bool>();
            pointsInHull = new List<Point3d>();

            for (int i = 0; i < Apoints.Count; i++)
            {
                if (IsInside(Apoints[i].Pos, _hull) && !inRegion[i])
                {
                    inHull.Add(true);
                    pointsInHull.Add(Apoints[i].Pos);
                }

                else inHull.Add(false);
            }

            return inHull;
        }

        void CheckPoints(int _i, int _j, List<bool> _inHull, List<int> _regionCounts, int _counter)
        {
            double minDistance = double.MaxValue;
            int index1 = -1;
            int index2 = -1;

            for (int k = 0; k < regions[_i].Apoints.Count; k++)
            {
                if (!regions[_i].Apoints[k].Burned)
                {
                    for (int m = 0; m < regions[_j].Apoints.Count; m++)
                    {
                        if (!regions[_j].Apoints[m].Burned)
                        {
                            double thisDistance = regions[_i].Apoints[k].Pos.DistanceTo(regions[_j].Apoints[m].Pos);
                            if (thisDistance < minDistance)
                            {
                                minDistance = thisDistance;
                                index1 = k;
                                index2 = m;
                            }
                        }
                    }
                }
            }

            if (index1 != -1 && index2 != -1)
            {
                regions[_i].Apoints[index1].Burned = true;
                regions[_j].Apoints[index2].Burned = true;

                Line newLine = new Line(regions[_i].Apoints[index1].Pos, regions[_j].Apoints[index2].Pos);
                //pairings.Add(newLine);

                _regionCounts[_i] = _regionCounts[_i] - 1;
                _regionCounts[_j] = _regionCounts[_j] - 1;

                for (int i = 0; i < _inHull.Count; i++)
                {
                    double threshold = 0.1;
                    if (newLine.DistanceTo(Apoints[i].Pos, true) < threshold)
                    {

                        _counter = _counter - 1;
                    }
                }
            }
        }

        double Fraction(float _pointDim, double max, double min)
        {
            return (_pointDim - min) / (max - min);
        }


        double Lerp(float p1, float p2, float fraction)
        {
            return p1 + (p2 - p1) * fraction;
        }

        Vector3d Direction(Point3d start, Point3d stop)
        {
            Vector3d newVector = new Vector3d(stop - start);
            newVector.Unitize();
            return newVector;
        }

        Dictionary<string, Vector3d> Directions(int _nrRegions, List<Point3d> _midpoints)
        {
            Dictionary<string, Vector3d> directions = new Dictionary<string, Vector3d>();

            for (int i = 0; i < _nrRegions; i++)
            {
                for (int j = 0; j < _nrRegions; j++)
                {
                    if (i != j)
                    {
                        string key = i.ToString() + ">" + j.ToString();

                        Vector3d direction = Direction(_midpoints[i], _midpoints[j]);
                        directions.Add(key, direction);
                    }
                }
            }

            return directions;
        }

        void AddEdges(bool _solid)
        {
            bool[] external = mesh.GetNakedEdgePointStatus();
            Polyline[] array = mesh.GetNakedEdges();

            List<Line> externalEdges = new List<Line>();

            for (int i = 0; i < array.Length; i++)
            {
                Line[] newLines = array[i].GetSegments();
                    externalEdges.AddRange(newLines);
            }

            List<Apoint>[] edgeRegions = new List<Apoint>[externalEdges.Count];

            for (int i = 0; i < externalEdges.Count; i++)
            {
                edgeRegions[i] = new List<Apoint>();
            }

            List<Apoint> edgePoints = new List<Apoint>();

            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                if (external[i])
                {
                    edgePoints.Add(Apoints[i]);
                }
            }

            for (int i = 0; i < edgePoints.Count; i++)
            {
                double smallest = double.MaxValue;
                int index = -1;

                for (int j = 0; j < externalEdges.Count; j++)
                {
                        double distance = externalEdges[j].DistanceTo(edgePoints[i].Pos, true);

                        if (distance < smallest)
                        {
                            smallest = distance;
                            index = j;
                        }
                }

                if (_solid)
                {
                    edgePoints[i].Da = 0.4;
                    edgePoints[i].Db = 0.2;
                    edgePoints[i].F = 0.1;
                    edgePoints[i].K = 0.047;
                }

                else
                {
                    edgePoints[i].Da = 1.0;
                    edgePoints[i].Db = 1.0;
                    edgePoints[i].F = 0.0;
                    edgePoints[i].K = 0.0;
                }

                edgeRegions[index].Add(edgePoints[i]);
            }

            for (int i = 0; i < edgeRegions.Length; i++)
            {
                Region newRegion = new Region(edgeRegions[i]);
                regions.Add(newRegion);
            }
        }

        public void InHull(Polyline _hull, bool _solid)
        {
            List<bool> inHull = CheckHull(_hull);
            int counter = inHull.Count;
            pairings = new List<Line>();
            List<Point3d> midpoints = new List<Point3d>();
            List<int> regionCounts = new List<int>();
             regionHulls = new List<Polyline>();

            for (int i = 0; i < regions.Count; i++)
            {
                regionCounts.Add(regions[i].Apoints.Count);
                midpoints.Add(GetCentroid(regions[i].Apoints, i));

                List<Point3d> forHull = new List<Point3d>();

                for (int j = 0; j < regions[i].Apoints.Count; j++)
                {
                    if (regions[i].Apoints[i].EdgePoint)
                    {
                        forHull.Add(regions[i].Apoints[i].Pos);
                    }
                }

                regionHulls.Add(new Polyline(forHull));
            }

            pointsInbetween = new List<Point3d>();
            pointsInbetween = midpoints;
            
            List<Apoint> edgeApoints = new List<Apoint>();
            MeshTopologyVertexList connected = mesh.TopologyVertices; 

            for (int i = 0; i < Apoints.Count; i++)
            {
                if (Apoints[i].EdgePoint)
                {
                    bool inside = false;



                    int[] neighbours = connected.ConnectedTopologyVertices(i);

                    for (int j = 0; j < neighbours.Length; j++)
                    {
                        if (inHull[neighbours[j]]) inside = true; 
                    }

                    if(inside)
                    {
                        pointsOnEdge.Add(Apoints[i].Pos);
                        edgeApoints.Add(Apoints[i]);
                    }
                }
            }

            Dictionary<string, Vector3d> directions = Directions(regions.Count, midpoints);

            for (int i = 0; i < edgeApoints.Count; i++)
            {
                double distance = double.MaxValue;
                int region = -1;

                for (int j = 0; j < regions.Count; j++)
                {
                    if (edgeApoints[i].regionIndex != j)
                    {
                        for (int k = 0; k < regions[j].Apoints.Count; k++)
                        {
                            double thisdist = edgeApoints[i].Pos.DistanceTo(regions[j].Apoints[k].Pos);

                            if (thisdist < distance)
                            {
                                distance = thisdist;
                                region = j;
                            }
                        }
                    }
                }

                string key = edgeApoints[i].regionIndex.ToString() + ">" + region.ToString();
                Vector3d v = directions[key];

                Point3d pointResult = new Point3d();
                pointResult = Point3d.Add(edgeApoints[i].Pos, v);

                pairings.Add(new Line(edgeApoints[i].Pos, pointResult));
            }

            //if (!mesh.IsClosed)
            //{
            //    AddEdges(_solid);
            //}

            //for (int i = 0; i < _hull.SegmentCount; i++)
            //{
            //    for (int j = 0; j < Apoints.Count; j++)
            //    {
            //        if(inRegion[j])
            //        {
            //            double distance = _hull.SegmentAt(i).DistanceTo(Apoints[j].Pos, true);
            //            if (distance < 0.1) pointsOnEdge.Add(Apoints[j].Pos);
            //        }
            //    }
            //}

            bool stop = false;

            while (counter > 0 || stop)
            {
                for (int i = 0; i < regions.Count; i++)
                {
                    for (int j = 0; j < regions.Count; j++)
                    {
                        if (i != j)
                        {
                            CheckPoints(i, j, inHull, regionCounts, counter);
                        }
                    }
                }

                for (int i = 0; i < regionCounts.Count; i++)
                {
                    if (regionCounts[i] < 1)
                    {
                        for (int j = 0; j < inHull.Count; j++)
                        {
                            double smallest = double.MaxValue;
                            int index = -1;

                            for (int k = 0; k < pairings.Count; k++)
                            {
                                //   pairings[k].DistanceTo()
                            }
                        }
                        goto out1;
                    }
                }
            }

        out1:
            ;

        }
    }
}