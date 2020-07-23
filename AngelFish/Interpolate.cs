using System;
using System.Drawing;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino.Geometry.Collections;

namespace Angelfish
{
    public class Interpolate
    {
        public List<Color> colours;
        MeshVertexList points; 

        List<float> r;
        List<float> g;
        List<float> b; 


        Point3f max;
        Point3f min;

        bool excludeX;
        bool excludeY;
        bool excludeZ;

        public Interpolate(Mesh mesh)
        {
            points = mesh.Vertices;
            InitAll();
            MinMax();
            OneDim();
        }

        public Interpolate(Mesh mesh, Vector3d _unitVector)
        {
            points = mesh.Vertices;
            InitAll();
            MinMax();

            OneDim(_unitVector);
        }

        public Interpolate(Mesh mesh, Plane _plane)
        {
            points = mesh.Vertices;
            InitAll();
            MinMax();
        }

        void InitAll()
        {
            r = new List<float>();
            g = new List<float>();
            b = new List<float>();

            colours = new List<Color>();
        }

        //OBS. finns i asystem
        void MinMax()
        {
            min = new Point3f(float.MaxValue, float.MaxValue, float.MaxValue);
            max = new Point3f(float.MinValue, float.MinValue, float.MinValue);

            for (int i = 0; i < points.Count; i++)
            {
                if (max.X < points[i].X) max.X = points[i].X;
                if (max.Y < points[i].Y) max.Y = points[i].Y;
                if (max.Z < points[i].Z) max.Z = points[i].Z;

                if (min.X > points[i].X) min.X = points[i].X;
                if (min.Y > points[i].Y) min.Y = points[i].Y;
                if (min.Z > points[i].Z) min.Z = points[i].Z;
            }

            if (min.X == max.X) excludeX = true;
            if (min.Y == max.Y) excludeY = true;
            if (min.Z == max.Z) excludeZ = true;
        }

        void OneDim()
        {
            int red = 255;
            int blue = 0;

            for (int i = 0; i < points.Count; i++)
            {
                float fr;

                if (!excludeX) fr = fraction(points[i].X, max.X, min.X);
                else if (!excludeY) fr = fraction(points[i].Y, max.Y, min.Y);
                else fr = fraction(points[i].Z, max.Z, min.Z);

                float green = Lerp(0, 255, fr);

                colours.Add(Color.FromArgb(red, (int)green, blue));
            }
        }

        void OneDim(Vector3d _unitVector)
        {
            int red = 255;
            int blue = 0;
            double largest = _unitVector.MaximumCoordinate;

            for (int i = 0; i < points.Count; i++)
            {
                float fr; 

                if (_unitVector.X == largest) fr = fraction(points[i].X, max.X, min.X);
                else if (_unitVector.Y == largest) fr = fraction(points[i].Y, max.Y, min.Y);
                else fr = fraction(points[i].Z, max.Z, min.Z);

                float green = Lerp(0, 255, fr);

                colours.Add(Color.FromArgb(red, (int)green, blue));
            }
        }

        void BiLinear()
        {
            for (int i = 0; i < points.Count; i++)
            {
                List<int> minY = new List<int>();
                List<int> maxY = new List<int>();

                if (points[i].Y == min.Y )
                {
                    int red = 255;
                    int blue = 0;

                    float fr = fraction(points[i].X, max.X, min.X);

                    float green = Lerp(0, 255, fr);

                    colours.Add(Color.FromArgb(red, (int)green, blue));
                    minY.Add(i);
                }

                else if(points[i].Y == max.Y)
                {
                    int red = 0;
                    int blue = 255;

                    float fr = fraction(points[i].X, max.X, min.X);

                    float green = Lerp(0, 255, fr);

                    colours.Add(Color.FromArgb(red, (int)green, blue));
                    maxY.Add(i);
                }

            }

            for (int i = 0; i < points.Count; i++)
            {
                 if(points[i].Y != max.Y || points[i].Y != min.Y)
                {

                }
            }
        }

        float fraction(float _pointDim, float max, float min)
        {
            return (_pointDim - min) / (max - min);
        }


        float Lerp (float p1, float p2, float fraction) 
        { 
            //fraction : value between 0 and 1 
            return p1 + (p2 - p1) * fraction; 
        }

    }
}