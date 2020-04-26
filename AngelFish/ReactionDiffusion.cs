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

        GH_Structure<GH_Number> values;
        public GH_Structure<GH_Number> neighbours;

        Mesh mesh;

        public List<Point3d> solid;
        public List<Point3d> other;

        public ReactionDiffusion(GH_Structure<GH_Number> _values, Mesh _mesh)
        {
            mesh = _mesh;
            values = _values;

            Setup();
            StartRandom();
        }

        public ReactionDiffusion(List<GH_Number> _values, Mesh _mesh)
        {
            mesh = _mesh;
            ValuesToTree(_values);

            Setup();
            StartRandom();
        }


        private void Setup()
        {
            FindNeighbours();

            a = new List<double>();
            b = new List<double>();
            nextA = new List<double>();
            nextB = new List<double>();
            solid = new List<Point3d>();
            other = new List<Point3d>();

            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                a.Add(1.0);
                b.Add(0.0);
                nextA.Add(0.0);
                nextB.Add(0.0);
            }
        }

        private void StartRandom()
        {
            Random random = new Random(1);

            for (int r = 0; r < mesh.Vertices.Count*0.1; r ++)
            {
                int randomIndex = random.Next(0, mesh.Vertices.Count);

                b[randomIndex] = 1.0;
                a[randomIndex] = 0.0;

                List<GH_Number> branch = neighbours.get_Branch(randomIndex) as List<GH_Number>;

                for (int nI = 0; nI < branch.Count; nI++)
                {
                    int neigh = (int)branch[nI].Value;
                     
                    b[neigh] = 1.0;
                    a[neigh] = 0.0;
                }
            }
        }

        private void ValuesToTree(List<GH_Number> _values)
        {
            values = new GH_Structure<GH_Number>();

            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                values.AppendRange(_values, new GH_Path(i));
            }
        }

        public void Update()
        {
            CalculateRD();
            Swap();
        }

        private void CalculateRD()
        {
            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                double thisa = a[i];
                double thisb = b[i];
                int count = mesh.Vertices.Count;
                List<GH_Number> branch = values.get_Branch(i) as List<GH_Number>;

                double f = branch[2].Value;

                nextA[i] = thisa + (branch[0].Value * Laplace(i, true)) - (thisa * thisb * thisb) + (f * (1 - thisa));
                nextB[i] = thisb + (branch[1].Value * Laplace(i, false)) + (thisa * thisb * thisb) - ((branch[3].Value + f) * thisb);
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
            double sum = 0;

            if (alaplace) sum += a[index] * -1;
            else sum += b[index] * -1;

            //GH_Path path = neighbours.get_Path(index);
            List<GH_Number> branch = neighbours.get_Branch(index) as List<GH_Number>;

            int nrNeigbours = branch.Count;

            double weight = 1.0 / nrNeigbours;

            for (int i = 0; i < nrNeigbours; i++)
            {
                int neighbor = (int)branch[i].Value;

                if (alaplace) sum += a[neighbor] * weight;
                else sum += b[neighbor] * weight;
            }

            return sum;
        }


        public double PrintOut()
        {
            GH_Path path = values.get_Path(20);

            return values.get_DataItem(path, 0).Value;

        }

        private void FindNeighbours()
        {
            neighbours = new GH_Structure<GH_Number>();

            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                int[] near = mesh.Vertices.GetConnectedVertices(i);
                List<GH_Number> ex = new List<GH_Number>();

                for (int j = 0; j < near.Length; j++)
                {
                    ex.Add(new GH_Number(near[j]));
                }

                neighbours.AppendRange(ex, new GH_Path(i));
            }
        }

        public void DividePoints()
        {

            //mesh.VertexColors.CreateMonotoneMesh(Color.White);

            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                // mesh.VertexColors(i) = Color.Purple;

                if (a[i] < 0.4)
                {
                    solid.Add(mesh.Vertices[i]);
                }

                else other.Add(mesh.Vertices[i]);
            }

            //for (int i = 0; i < 5; i += 10)
            //{
            //    solid.Add(mesh.Vertices[i]);

            //    neighbours.get_Branch(i);


            //    for (int j = 0; j < ; j++)
            //    {


            //    }
            //}
        }
    }
}