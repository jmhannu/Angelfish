using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Rhino.Geometry;

namespace Angelfish
{

    public class Values
    {
        private int valuesCount;
        private bool unassigned;
        private bool measured; 
        private List<double> fValues;
        private List<double> kValues;
        private List<double> dAValues;
        private List<double> dBValues;
        private List<double> massProcentages;
        private List<double> parts;
        private List<double> edgeConnectionProcentages;
        private List<double> solidEdgeProcentage;
        private List<string> types;
        private DataTree<double> varibles;

        public int ValuesCount { get { return valuesCount; } set { valuesCount = CountVaribles(); } }
        public List<double> FValues { get { return fValues; }  set { if(unassigned) fValues = value; } }
        public List<double> KValues { get { return kValues; } set { if (unassigned) kValues = value; } }
        public List<double> DAValues { get { return dAValues; } set { if (unassigned) dAValues = value; } }
        public List<double> DBValues { get { return dBValues; } set { if (unassigned) dBValues = value; } }
        public List<double> MassProcentages { get { return massProcentages; } }
        public List<double> Parts { get { return parts; } }
        public List<double> EdgeConnectionProcentages { get { return edgeConnectionProcentages; } }
        public List<double> SolidEdgeProcentage { get { return solidEdgeProcentage; } }
        public List<string> Types { get { return types; } }
        public DataTree<double> Varibles { get { return varibles; } }

        public Values()
        {
            InitAll();
            unassigned = true;
            measured = false; 
        }

        public Values(string file)
        {
            InitAll();

            string[] lines = file.Split('\n');

            foreach (string line in lines)
            {
                string fixedLine = line.Replace(',', '.');
                string[] lineValues = fixedLine.Split('\t');

                dAValues.Add(Convert.ToDouble(lineValues[0]));
                dBValues.Add(Convert.ToDouble(lineValues[1]));
                fValues.Add(Convert.ToDouble(lineValues[2]));
                kValues.Add(Convert.ToDouble(lineValues[3]));
                massProcentages.Add(Convert.ToDouble(lineValues[4]));
                edgeConnectionProcentages.Add(Convert.ToDouble(lineValues[5]));
                types.Add(lineValues[6]);
                parts.Add(Convert.ToDouble(lineValues[7]));
                solidEdgeProcentage.Add(Convert.ToDouble(lineValues[9]));

                valuesCount++;
                unassigned = false;
                measured = true; 
            }

            PopulateVaribleTree();
        }

        private void InitAll()
        {
            valuesCount = 0;
            fValues = new List<double>();
            kValues = new List<double>();
            dAValues = new List<double>();
            dBValues = new List<double>();
            massProcentages = new List<double>();
            parts = new List<double>();
            edgeConnectionProcentages = new List<double>();
            solidEdgeProcentage = new List<double>();
            types = new List<string>();
            varibles = new DataTree<double>();
        }

        private void PopulateVaribleTree()
        {

            for (int i = 0; i < valuesCount; i++)
            {
                List<double> allVaribles = new List<double>();
                allVaribles.Add(dAValues[i]);
                allVaribles.Add(dBValues[i]);
                allVaribles.Add(fValues[i]);
                allVaribles.Add(kValues[i]);

                varibles.AddRange(allVaribles, new GH_Path(i));
            }
        }

        private int CountVaribles()
        {
            int counter = 0;

            return counter;
        }

    }
}