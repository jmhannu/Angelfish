using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Rhino.Geometry.Collections;

namespace Angelfish
{
    class Measure : Pattern
    {
        public double MassPercentage;
        public double ConnectedPercentage;
        public double SolidEdgePercentage;

        private List<bool> set;
        public List<int> parts;
        List<int> neighbourCount;
        int solidEdge;

        public Measure(Pattern _pattern) : base(_pattern)
        {
            neighbourCount = new List<int>();
            parts = new List<int>();
            set = new List<bool>();

            for (int i = 0; i < Apoints.Count; i++)
            {
                set.Add(false);
            }

            solidEdge = 0;

            Connectivity();

            MassPercentage = MassPercent();
            ConnectedPercentage = ConnectivityRate();
            SolidEdgePercentage = SolidEdge();
        }


        private double MassPercent()
        {
            int inPattern = 0; 

            if(SolidPattern)
            {
                inPattern = Solid.Count;
            }

            else
            {
                inPattern = Void.Count;
            }

            return (double)inPattern / (double)Apoints.Count;
        }


        private void Connectivity()
        {
            for (int i = 0; i < Apoints.Count; i++)
            {
                if (!set[i])
                {
                    set[i] = true;

                    if (Apoints[i].InPattern)
                    {
                        parts.Add(1);
                        int index = parts.Count - 1;

                        Point3d position = Apoints[i].Pos;

                        if (
                            (!excludeX && (position.X == max.X || position.X == min.X)) ||
                            (!excludeY && (position.Y == max.Y || position.Y == min.Y))
                            || (!excludeZ && (position.Z == max.Z || position.Z == min.Z))
                            )
                        {
                            solidEdge++;
                        }

                        List<int> allNeighbours = new List<int>();
                        allNeighbours.AddRange(Apoints[i].Neighbours);
                        allNeighbours.AddRange(Apoints[i].SecoundNeighbours);

                        List<int> burned = new List<int>();

                        for (int j = 0; j < allNeighbours.Count; j++)
                        {

                            int myNeighbour = allNeighbours[j];

                            if (!set[myNeighbour])
                            {
                                set[myNeighbour] = true;

                                if (Apoints[myNeighbour].InPattern)
                                {
                                    burned.Add(myNeighbour);
                                    parts[index]++;
                                    Point3d mypos = Apoints[myNeighbour].Pos;

                                    if (
                                        (!excludeX && (mypos.X == max.X || mypos.X == min.X)) ||
                                        (!excludeY && (mypos.Y == max.Y || mypos.Y == min.Y))
                                        || (!excludeZ && (mypos.Z == max.Z || mypos.Z == min.Z))
                                        )
                                    {
                                        solidEdge++;
                                    }
                                }
                            }
                        }

                        for (int k = 0; k < burned.Count; k++)
                        {
                            List<int> burnedNeighbours = new List<int>();
                            burnedNeighbours.AddRange(Apoints[burned[k]].Neighbours);
                            burnedNeighbours.AddRange(Apoints[burned[k]].SecoundNeighbours);

                            for (int m = 0; m < burnedNeighbours.Count; m++)
                            {
                                int thisNeighbour = burnedNeighbours[m];

                                if (!set[thisNeighbour])
                                {
                                    set[thisNeighbour] = true;

                                    if (Apoints[thisNeighbour].InPattern)
                                    {
                                        burned.Add(thisNeighbour);
                                        parts[index]++;
                                        Point3d thispos = Apoints[thisNeighbour].Pos;

                                        if (
                                            (!excludeX && (thispos.X == max.X || thispos.X == min.X)) ||
                                            (!excludeY && (thispos.Y == max.Y || thispos.Y == min.Y))
                                            || (!excludeZ && (thispos.Z == max.Z || thispos.Z == min.Z))
                                            )
                                        {
                                            solidEdge++;
                                        }
                                    }
                                }
                            }

                        }
                    }
                }
            }
        }

        private void CheckNeighbours(int me)
        {
            List<int> allNeighbours = new List<int>();
            allNeighbours.AddRange(Apoints[me].Neighbours);
            allNeighbours.AddRange(Apoints[me].SecoundNeighbours);

            neighbourCount.Add(allNeighbours.Count);

            for (int i = 0; i < allNeighbours.Count; i++)
            {

                int me2 = allNeighbours[i];

                if (!set[me2])
                {
                    set[me2] = true;

                    if (Apoints[me2].InPattern)
                    {

                        Point3d position = Apoints[me2].Pos;

                        if (
                            (!excludeX && (position.X == max.X || position.X == min.X)) ||
                            (!excludeY && (position.Y == max.Y || position.Y == min.Y))
                            || (!excludeZ && (position.Z == max.Z || position.Z == min.Z))
                            )
                        {
                            solidEdge++;
                        }

                        CheckNeighbours(me2);
                    }
                }
            }
        }

        private double ConnectivityRate()
        {
            int excluded = 0;
            int included = 0;

            for (int i = 0; i < parts.Count; i++)
            {
                if (parts[i] < 5) excluded += parts[i];
                else included++;
            }

            int inPattern = 0;
            if (SolidPattern) inPattern = Solid.Count;
            else inPattern = Void.Count;

            return (1.0 / ((double)included - ((double)excluded / (double)inPattern)));
        }

        private double SolidEdge()
        {
            return ((double)solidEdge / (double)edgeCount);
        }
    }
}