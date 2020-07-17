using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Angelfish;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Rhino.Geometry.Collections;

class Measure
{
    public double MassPercentage;
    public double ConnectedPercentage;
    public double SolidEdgePercentage;

    private List<Apoint> apoints;
    private List<int> pattern;
    private List<bool> set;

     int parts;
     int edgeCount;
     List<int> neighbourCount;

      Point3d min;
      Point3d max;

     bool excludeX;
     bool excludeY;
     bool excludeZ;


    //bool edgepart;
    //bool left;
    //bool right;
    //bool top;
    //bool down;
    //bool front;
    //bool back;

    //int edgeparts;
    //int one_edge;
    //int two_edgescross;
    //int two_edgesclose;
    //int three_edges_cross;
    //int three_edges_corner;
    //int four_edges_corner;
    //int four_edges_tube;
    //int five_edges;
    //int six_edges;

    public Measure(Asystem _asystem)
    {
        apoints = _asystem.Apoints;
        pattern = _asystem.Pattern;
        neighbourCount = new List<int>();
        parts = 0;
        edgeCount = 0;
 
        set = new List<bool>();

        for (int i = 0; i < apoints.Count; i++)
        {
            set.Add(false);
        }

        min = _asystem.min;
        max = _asystem.max;

        if (min.X == max.X) excludeX = true;
        else excludeX = false;
        if (min.Y == max.Y) excludeY = true;
        else excludeY = false;
        if (min.Z == max.Z) excludeZ = true;
        else excludeZ = false;

        Connectivity();

        MassPercentage = MassPercent();
        ConnectedPercentage = ConnectivityRate();
        SolidEdgePercentage = SolidEdge(_asystem.edgeCount);
    }

    //private void Setup()
    //{
    //edgepart = false;
    //left = false;
    //right = false;
    //top = false;
    //down = false;

    //edgeparts = 0;
    //one_edge = 0;
    //two_edgescross = 0;
    //two_edgesclose = 0;
    //three_edges_cross = 0;
    //three_edges_corner = 0;
    //four_edges_corner = 0;
    //four_edges_tube = 0;
    //five_edges = 0;
    //six_edges = 0;
    //}


    private double MassPercent()
    {
        return (double)pattern.Count / (double)apoints.Count;
    }


    private void Connectivity()
    {
        for (int i = 0; i < apoints.Count; i++)
        {
            if (!set[i])
            {
                set[i] = true;

                if (apoints[i].InPattern)
                {
                    parts++;

                    Point3d position = apoints[i].Pos;

                    if (
                        (!excludeX && (position.X == max.X || position.X == min.X)) ||
                        (!excludeY && (position.Y == max.Y || position.Y == min.Y))
                        //|| (!excludeZ && (position.Z == max.Z || position.Z == min.Z))
                        )
                    {
                        edgeCount++;
                    }

                    CheckNeighbours(i);
                }
            }
        }
    }

    private void CheckNeighbours(int me)
    {
        List<int> allNeighbours = new List<int>();
        allNeighbours.AddRange(apoints[me].Neighbours);
        allNeighbours.AddRange(apoints[me].SecoundNeighbours);

        neighbourCount.Add(allNeighbours.Count);

        for (int i = 0; i < allNeighbours.Count; i++)
        {

            int me2 = allNeighbours[i];

            if (!set[me2])
            {
                set[me2] = true;

                if (apoints[me2].InPattern)
                {

                    Point3d position = apoints[me2].Pos;

                    if (
                        (!excludeX && (position.X == max.X || position.X == min.X)) ||
                        (!excludeY && (position.Y == max.Y || position.Y == min.Y))
                        //|| (!excludeZ && (position.Z == max.Z || position.Z == min.Z))
                        )
                    {
                        edgeCount++;
                    }

                    CheckNeighbours(me2);
                }
            }
        }
    }

    private double ConnectivityRate()
    {
        return (1.0 / (double)parts);
    }

    private double SolidEdge(int _totalEdge)
    {
        return ((double)edgeCount / (double)_totalEdge);
    }
}