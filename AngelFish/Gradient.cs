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
                    //Point3d cp = mvContour[i].closestPoint(pt);
                    //minDist = min(minDist, cp.distance(pt));
                    minDist = Math.Min(minDist, cp.DistanceTo(pt));
                }
                if (minDist < 1e-10)
                    return true;
            }

            if (oddNodes) return true;

            return false;
        }

        public void InHull(Polyline _hull)
        {
            int pathnr = 0;
            List<bool> inHull = new List<bool>();
            pointsInbetween = new List<Point3d>();
            pointsInHull = new List<Point3d>();
            matches = new GH_Structure<GH_Point>();

            for (int i = 0; i < Apoints.Count; i++)
            {
                if (IsInside(Apoints[i].Pos, _hull))
                {
                    inHull.Add(true);
                    pointsInHull.Add(Apoints[i].Pos);
                }
                else inHull.Add(false);
            }

            for (int i = 0; i < Apoints.Count; i++)
            {
                if (inHull[i] && !inRegion[i])
                {
                    pointsInbetween.Add(Apoints[i].Pos);
                    if (regions.Count > 1)
                    {
                        List<Tuple<double, int>> distIndex = new List<Tuple<double, int>>();

                        for (int j = 0; j < regions.Count; j++)
                        {
                            double smallDistance = double.MaxValue;
                            int smallIndex = -1;

                            for (int k = 0; k < regions[j].Apoints.Count; k++)
                            {
                                double thisDistance = Apoints[i].Pos.DistanceTo(regions[j].Apoints[k].Pos);
                                if (thisDistance < smallDistance)
                                {
                                    smallIndex = regions[j].Apoints[k].Index;
                                    smallDistance = thisDistance;
                                }
                            }

                            distIndex.Add(new Tuple<double, int>(smallDistance, smallIndex));
                        }

                        //distIndex = distIndex.OrderByDescending(t => t.Item2).ToList();
                        //list.Sort((x, y) => y.Item1.CompareTo(x.Item1));

                        //int index1 = distIndex[0].Item2;
                        //int index2 = distIndex[1].Item2;

                        List<GH_Point> branch = new List<GH_Point>();

                        for (int j = 0; j < distIndex.Count; j++)
                        {
                            branch.Add(new GH_Point(Apoints[distIndex[j].Item2].Pos));
                        }

                        //Apoint point1 = Apoints[index1];
                        //Apoint point2 = Apoints[index2];

                        branch.Add(new GH_Point(Apoints[i].Pos));
                        //branch.Add(new GH_Point(Apoints[index1].Pos));
                        //branch.Add(new GH_Point(Apoints[index2].Pos));
                        GH_Path path = new GH_Path(pathnr);
                        matches.AppendRange(branch, path);
                        pathnr++;
                    }
                }
            }
        }
    }
}