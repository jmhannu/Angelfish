using System;
using System.Drawing;
using System.Collections.Generic;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Rhino.Geometry.Collections;


namespace Angelfish
{
    public class ReactionDiffusion
    {
        private List<double> a;
        private List<double> b;
        private List<double> nextA;
        private List<double> nextB;

        public Asystem Asystem;
        readonly int rdSize;

        public List<int> Solid;
        public List<int> Void;

        public ReactionDiffusion(Asystem _asystem)
        {
            Asystem = _asystem;
            rdSize = Asystem.asize;
            Setup();
            Start();
        }


        private void Setup()
        {
            a = new List<double>();
            b = new List<double>();
            nextA = new List<double>();
            nextB = new List<double>();

            for (int i = 0; i < rdSize; i++)
            {
                a.Add(1.0);
                b.Add(0.0);
                nextA.Add(0.0);
                nextB.Add(0.0);
            }

            Solid = new List<int>();
            Void = new List<int>();
        }

        private void Start()
        {

            b[rdSize / 2] = 1.0;
            a[rdSize / 2] = 0.0;

            List<int> near1 = Asystem.Apoints[rdSize / 2].Neighbours;
            List<int> near2 = Asystem.Apoints[rdSize / 2].SecoundNeighbours;

            for (int i = 0; i < near1.Count; i++)
            {
                b[near1[i]] = 1.0;
            }

            for (int i = 0; i < near2.Count; i++)
            {
                b[near2[i]] = 1.0;
            }
        }

        public void Update()
        {
            CalculateRD();
            Swap();
        }


        private void CalculateRD()
        {

            for (int i = 0; i < rdSize; i++)
            {
                double thisa = a[i];
                double thisb = b[i];
                double f = Asystem.Apoints[i].F;

                nextA[i] = thisa + (Asystem.Apoints[i].Da * Laplace(i, true)) - (thisa * thisb * thisb) + (f * (1 - thisa));
                nextB[i] = thisb + (Asystem.Apoints[i].Db * Laplace(i, false)) + (thisa * thisb * thisb) - ((Asystem.Apoints[i].K + f) * thisb);
            }
        }

        private void Swap()
        {
            for (int i = 0; i < a.Count; i++)
            {
                a[i] = nextA[i];
                b[i] = nextB[i];
            }
        }

        double Laplace(int index, bool alaplace)
        {
            List<GH_Number> weights = new List<GH_Number>();
            double sum = 0;

            if (alaplace) sum += a[index] * (-1.0);
            else sum += b[index] * (-1.0);

            List<int> first = Asystem.Apoints[index].Neighbours;
            List<int> secound = Asystem.Apoints[index].SecoundNeighbours;

            double x = 0;
            x += first.Count;
            x += secound.Count * 0.25;
            double weight = 1 / x;
            weights.Add(new GH_Number(weight));

            for (int i = 0; i < first.Count; i++)
            {
                int neighbor = first[i];

                if (alaplace) sum += a[neighbor] * weight;
                else sum += b[neighbor] * weight;
            }

            for (int i = 0; i < secound.Count; i++)
            {
                int neighbor = secound[i];

                if (alaplace) sum += a[neighbor] * (weight * 0.25);
                else sum += b[neighbor] * (weight * 0.25);
            }

            return sum;
        }

        public void DividePoints(double threshold)
        {
            for (int i = 0; i < rdSize; i++)
            {
                if (a[i] < 0.4)
                {
                    Solid.Add(i);
                }

                else Void.Add(i);
            }
        }

        //public double PrintOut()
        //{
        //    GH_Path path = values.get_Path(20);

        //    return values.get_DataItem(path, 0).Value;

        //}

        //private void ValuesToTree(List<GH_Number> _values)
        //{
        //    values = new GH_Structure<GH_Number>();

        //    for (int i = 0; i < mesh.Vertices.Count; i++)
        //    {
        //        values.AppendRange(_values, new GH_Path(i));
        //    }
        //}
    }
}