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
    public class Pattern : Asystem
    {
        public List<double> a;
        private List<double> b;
        private List<double> nextA;
        private List<double> nextB;

        readonly int rdSize;
        public List<int> InPattern;


        public List<int> Solid;
        public List<int> Void;

        public Pattern() : base()
        {
            Apoints = new List<Apoint>();
            InitAll();
        }

        public Pattern(Pattern _pattern) : base(_pattern)
        {
            this.Apoints = _pattern.Apoints;
            this.a = _pattern.a;
            this.b = _pattern.b;
            this.nextA = _pattern.nextA;
            this.nextB = _pattern.nextB;
            this.rdSize = _pattern.rdSize;
            this.Solid = _pattern.Solid;
            this.Void = _pattern.Void;
            this.InPattern = _pattern.InPattern;
        }

        public Pattern(Asystem _asystem) : base(_asystem)
        {
            Apoints = _asystem.Apoints;
            rdSize = Apoints.Count;

            Setup();
            Start();
        }

        public Pattern(List<Apoint> _apoints) : base()
        {
            Apoints = _apoints;
            rdSize = Apoints.Count;

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

            List<int> near1 = Apoints[rdSize / 2].Neighbours;
            List<int> near2 = Apoints[rdSize / 2].SecoundNeighbours;

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
                double f = Apoints[i].F;

                nextA[i] = thisa + (Apoints[i].Da * Laplace(i, true)) - (thisa * thisb * thisb) + (f * (1 - thisa));
                nextB[i] = thisb + (Apoints[i].Db * Laplace(i, false)) + (thisa * thisb * thisb) - ((Apoints[i].K + f) * thisb);
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

            List<int> first = Apoints[index].Neighbours;
            List<int> secound = Apoints[index].SecoundNeighbours;

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
                if (a[i] < threshold)
                {
                    Solid.Add(i);
                }

                else
                {
                    Void.Add(i);
                }
            }
        }
    }
}