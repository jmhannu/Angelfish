using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Angelfish
{
    public class Gradient : Pattern
    {
        public List<Color> colors;
        public List<Pattern> regions;
        public List<bool> inRegion;
        public List<Point3d> pointsInHull;
        public List<Point3d> pointsInbetween;
        public GH_Structure<GH_Point> matches;
        public Point3d[] midpoints;
        public List<Line> pairings;
        List<bool> inHull; 


        public Gradient(List<Pattern> _regions, Mesh _mesh)
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

        Point3d GetCentroid(List<Apoint> _apoints)
        {
            double[] centroid = new double[3];

            foreach (Apoint point in _apoints)
            {
                centroid[0] += point.Pos.X;
                centroid[1] += point.Pos.Y;
                centroid[2] += point.Pos.Z;
            }

            centroid[0] /= _apoints.Count;
            centroid[1] /= _apoints.Count;
            centroid[2] /= _apoints.Count;

            return new Point3d(centroid[0], centroid[1], centroid[2]);
        }

        List<bool> CheckHull(Polyline _hull)
        {
            inHull = new List<bool>();
            pointsInbetween = new List<Point3d>();
            pointsInHull = new List<Point3d>();
            matches = new GH_Structure<GH_Point>();

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

            if(index1 != -1 && index2 != -1)
            {
                regions[_i].Apoints[index1].Burned = true;
                regions[_j].Apoints[index2].Burned = true;

                Line newLine = new Line(regions[_i].Apoints[index1].Pos, regions[_j].Apoints[index2].Pos);
                pairings.Add(newLine);

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

        double fraction(float _pointDim, double max, double min)
        {
            return (_pointDim - min) / (max - min);
        }


        double Lerp(float p1, float p2, float fraction)
        {
            //fraction : value between 0 and 1 
            return p1 + (p2 - p1) * fraction;
        }

        public void InHull(Polyline _hull)
        {
            List<bool> inHull = CheckHull(_hull);
            int counter = inHull.Count;
            pairings = new List<Line>();

            int nrRegions = regions.Count;
            List<int> regionCounts = new List<int>();

            for (int i = 0; i < nrRegions; i++)
            {
                regionCounts.Add(regions[i].Apoints.Count);
            }

            bool stop = false; 

            while(counter > 0 || stop)
            {
                for (int i = 0; i < nrRegions; i++)
                {
                    for (int j = 0; j < nrRegions; j++)
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