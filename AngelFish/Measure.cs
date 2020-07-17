using System;
using System.Collections.Generic;
using Angelfish;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Rhino.Geometry.Collections;

class Measure
{
    int asize;
    public Asystem Pattern;
    List<bool> set;


    bool edgepart;
    bool left;
    bool right;
    bool top;
    bool down;
    bool front;
    bool back;
    int parts;
    int edgeparts;
    int edgeCount;
    int one_edge;
    int two_edgescross;
    int two_edgesclose;
    int three_edges_cross;
    int three_edges_corner;
    int four_edges_corner;
    int four_edges_tube;
    int five_edges;
    int six_edges;
    int mass;
    Vector3d min;
    Vector3d max;

    public Measure(Asystem _asystem)
    {
        Setup();
    }

    private void Setup()
    {
        edgepart = false;
        left = false;
        right = false;
        top = false;
        down = false;

        edgeCount = 0;
        parts = 0;
        mass = Mass();
        edgeparts = 0;
        one_edge = 0;
        two_edgescross = 0;
        two_edgesclose = 0;
        three_edges_cross = 0;
        three_edges_corner = 0;
        four_edges_corner = 0;
        four_edges_tube = 0;
        five_edges = 0;
        six_edges = 0;
        normalDistance = 0.0;

        set = new List<bool>();

        for (int i = 0; i < points.Count; i++)
        {
            set.Add(false);
        }
    }

    private void MinMax()
    {
        min = new Vector3d(double.MaxValue, double.MaxValue, double.MaxValue);
        max = new Vector3d(double.MinValue, double.MaxValue, double.MaxValue);

        for (int i = 0; i < points.Count; i++)
        {
            if (max.X < points[i].X) max.X = points[i].X;
            if (max.Y < points[i].Y) max.Y = points[i].Y;
            if (max.Z < points[i].Z) max.Z = points[i].Z;

            if (min.X > points[i].X) min.X = points[i].X;
            if (min.Y > points[i].Y) min.Y = points[i].Y;
            if (min.Z > points[i].Z) min.Z = points[i].Z;
        }
    }

    private int Mass()
    {
        return points.Count;
    }


    private void Connectivity()
    {
        for (int i = 0; i < points.Count; i++)
        {
            //ofColor c(ofRandom(255), ofRandom(255), ofRandom(255));
            set[i] = true;
            // pattern[i].color.set(c);

            edgepart = false;
            parts++;
            left = false;
            right = false;
            top = false;
            down = false;
            front = false;
            back = false;

            if (points[i].X == max.X || points[i].X == min.X ||
                points[i].Y == max.Y || points[i].Y == min.Y ||
                points[i].Z == max.Z || points[i].Z == min.Z)
            {
                if (!edgepart)
                {
                    edgeparts++;
                    edgepart = true;
                }

                if (points[i].X == max.X) left = true;
                if (points[i].Y == max.Y) right = true;
                if (points[i].Z == max.Z) top = true;
                if (points[i].X == min.X) down = true;
                if (points[i].Y == min.Y) front = true;
                if (points[i].Z == min.Z) back = true;
            }

            CheckNeighbours(i);
            //CheckNeighbours(i, c);

            if (left && right && top && down && front && back) six_edges++;
            else if ((left && right && top && down && back) || (left && right && top && down && front) || (left && right && top && front && back) ||
                (left && right && front && down && back) || (left && front && top && down && back) || (front && right && top && down && back)) five_edges++;
            else if ((top && right && down && left) || (front && right && back && left)) four_edges_tube++;
            else if ((top && right && front && back) || (top && left && front && back) || (down && right && front && back) || (down && left && front && back) ||
                (top && left && back && down) || (top && right && back && down) || (top && left && front && down) || (top && right && front && down)) four_edges_corner++;
            else if ((left && right && top) || (left && right && down) || (left && down && top) || (right && down && top) ||
                (left && front && right) || (left && back && right) || (front && right && back) || (front && left && back)) three_edges_cross++;
            else if ((top && right && front) || (top && left && front) || (top && right && back) || (top && left && back) ||
                (down && right && front) || (down && left && front) || (down && right && back) || (down && left && back)) three_edges_corner++;
            else if ((top && down) || (left && right) || (front && back)) two_edgescross++;
            else if ((top && right) || (top && left) || (down && right) || (down && left) ||
                (top && back) || (top && front) || (down && back) || (down && front) ||
                (right && back) || (right && front) || (left && back) || (left && front)) two_edgesclose++;
            else if (top || down || right || left || front || back) one_edge++;
        }
    }

    private void CheckNeighbours(int me)
    {
        List<GH_Number> branch = allNeighbours.get_Branch(me) as List<GH_Number>;

        for (int i = 0; i < branch.Count; i++)
        {
            int me2 = (int)branch[i].Value;

            if (!set[me2])
            {
                set[me2] = true;
                // pattern[me2].color.set(color);

                if (points[me2].X == max.X || points[me2].X == min.X ||
                    points[me2].Y == max.Y || points[me2].Y == min.Y ||
                     points[me2].Z == max.Z || points[me2].Z == min.Z)
                {
                    if (!edgepart)
                    {
                        edgeparts++;
                        edgepart = true;
                    }

                    if (points[me2].X == max.X) left = true;
                    if (points[me2].Y == max.Y) right = true;
                    if (points[me2].Z == max.Z) top = true;
                    if (points[me2].X == min.X) down = true;
                    if (points[me2].Y == min.Y) front = true;
                    if (points[me2].Z == min.Z) back = true;
                }

                //checkNeighbours(me2, color);
                CheckNeighbours(me2);
            }
        }
    }
}