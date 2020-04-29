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
        public List<GH_Number> weights;

        GH_Structure<GH_Number> values;
        List<double> valueList;
        public GH_Structure<GH_Number> neighbours;
        public GH_Structure<GH_Number> secoundNeighbours;

        Mesh mesh;
        public List<Point3d> solid;

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
            // ValuesToTree(_values);
            valueList = new List<double>();
            for (int i = 0; i < _values.Count; i++)
            {
                valueList.Add(_values[i].Value);
            }

            Setup();
            StartRandom();
        }


        private void Setup()
        {


            FindNeighboursDistance();
            //FindNeighbours();

            a = new List<double>();
            b = new List<double>();
            nextA = new List<double>();
            nextB = new List<double>();
            solid = new List<Point3d>();

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

          //  for (int r = 0; r < mesh.Vertices.Count * 0.1; r++)
         //   {
               // int randomIndex = random.Next(0, mesh.Vertices.Count);

                b[mesh.Vertices.Count/2] = 1.0;
               a[mesh.Vertices.Count / 2] = 0.0;

            List<GH_Number> branch = neighbours.get_Branch(mesh.Vertices.Count / 2) as List<GH_Number>;
            List<GH_Number> branch2 = secoundNeighbours.get_Branch(mesh.Vertices.Count/2) as List<GH_Number>;

                for (int nI = 0; nI < branch.Count; nI++)
                {
                    int neigh = (int)branch[nI].Value;

                    b[neigh] = 1.0;
                   // a[neigh] = 0.0;
                }

            for (int i = 0; i < branch2.Count; i++)
            {
                b[(int)branch2[i].Value] = 1.0;
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

        //private void CalculateRD()
        //{
        //    for (int i = 0; i < mesh.Vertices.Count; i++)
        //    {
        //        double thisa = a[i];
        //        double thisb = b[i];
        //        int count = mesh.Vertices.Count;
        //        List<GH_Number> branch = values.get_Branch(i) as List<GH_Number>;

        //        double f = branch[2].Value;

        //            nextA[i] = thisa + (branch[0].Value * Laplace(i, true)) - (thisa * thisb * thisb) + (f * (1 - thisa));
        //            nextB[i] = thisb + (branch[1].Value * Laplace(i, false)) + (thisa * thisb * thisb) - ((branch[3].Value + f) * thisb);

        //    }
        //}

        private void CalculateRD()
        {
            weights = new List<GH_Number>();
            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                double thisa = a[i];
                double thisb = b[i];
                double f = valueList[2];


                nextA[i] = thisa + (valueList[0] * Laplace(i, true)) - (thisa * thisb * thisb) + (f * (1 - thisa));
                nextB[i] = thisb + (valueList[1] * Laplace(i, false)) + (thisa * thisb * thisb) - ((valueList[3] + f) * thisb);
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

        //double Laplace(int index, bool alaplace)
        //{
        //    double sum = 0;

        //    List<GH_Number> branch = neighbours.get_Branch(index) as List<GH_Number>;

        //    int nrNeigbours = branch.Count;

        //    for (int i = 0; i < nrNeigbours; i++)
        //    {
        //        int neighbor = (int)branch[i].Value;

        //        if (alaplace) sum += a[neighbor];
        //        else sum += b[neighbor];
        //    }

        //    sum /= nrNeigbours;

        //    return sum;
        //}

        double Laplace(int index, bool alaplace)
        {
            double sum = 0;

            if (alaplace) sum += a[index] * (-1.0);
            else sum += b[index] * (-1.0);

            List<GH_Number> branch = neighbours.get_Branch(index) as List<GH_Number>;
            List<GH_Number> branch2 = secoundNeighbours.get_Branch(index) as List<GH_Number>;

            double x = 0;
            x += branch.Count;
            x += branch2.Count * 0.25;
            double weight = 1 / x;
            weights.Add(new GH_Number(weight));

            for (int i = 0; i < branch.Count; i++)
            {
                int neighbor = (int)branch[i].Value;

                if (alaplace) sum += a[neighbor] * weight;
                else sum += b[neighbor] * weight;
            }

            for (int i = 0; i < branch2.Count; i++)
            {
                int neighbor = (int)branch2[i].Value;

                if (alaplace) sum += a[neighbor] * (weight*0.25);
                else sum += b[neighbor] * (weight*0.25);
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
            secoundNeighbours = new GH_Structure<GH_Number>();

            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                int[] near = mesh.Vertices.GetConnectedVertices(i);

                List<GH_Number> tempList = new List<GH_Number>();

                for (int j = 0; j < near.Length; j++)
                {
                    tempList.Add(new GH_Number(near[j]));
                }

                neighbours.AppendRange(tempList, new GH_Path(i));
            }
        }

        public void FindNeighboursDistance()
        {
            neighbours = new GH_Structure<GH_Number>();
            secoundNeighbours = new GH_Structure<GH_Number>();

            int[] temp = mesh.Vertices.GetConnectedVertices(0);
            double distance = mesh.Vertices[0].DistanceTo(mesh.Vertices[temp[0]]);
            double searchDistance = distance + (distance / 2);

            RTree rTree = new RTree();

            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                rTree.Insert(mesh.Vertices[i], i);
            }

            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                Point3d vI = mesh.Vertices[i];
                Sphere searchSpehere = new Sphere(vI, searchDistance);

                List<int> near = new List<int>();

                rTree.Search(searchSpehere,
                    (sender, args) => { if (i != args.Id) near.Add(args.Id); }
                    );

                List<GH_Number> tempList = new List<GH_Number>();
                List<GH_Number> tempList2 = new List<GH_Number>();

                for (int j = 0; j < near.Count; j++)
                {
                    if (vI.DistanceTo(mesh.Vertices[near[j]]) <= distance + (distance * 0.1)) tempList.Add(new GH_Number(near[j]));
                    else tempList2.Add(new GH_Number(near[j]));
                }

                neighbours.AppendRange(tempList, new GH_Path(i));
                secoundNeighbours.AppendRange(tempList2, new GH_Path(i));
            }
        }

        public void DividePoints()
        {
            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                if (a[i] < 0.4)
                {
                    solid.Add(mesh.Vertices[i]);
                }
            }
        }

    }
}