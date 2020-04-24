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
        private List<double> massProcentages;
        private List<double> connectedProcentages;
        private List<double> edgeConnectionProcentages;
        private List<double> solidEdgeProcentage;
        private List<string> types;
        private DataTree<double> varibles;

        public int ValuesCount { get { return valuesCount; } }
        public List<double> MassProcentages { get { return massProcentages; } }
        public List<double> ConnectedProcentages { get { return connectedProcentages; } }
        public List<double> EdgeConnectionProcentages { get { return edgeConnectionProcentages; } }
        public List<double> SolidEdgeProcentage { get { return solidEdgeProcentage; } }
        public List<string> Types { get { return types; } }
        public DataTree<double> Varibles { get { return varibles; } }

        public Values()
        {
            InitAll();
        }

        public Values(string file)
        {
            InitAll();


            List<double> fValues = new List<double>();
            List<double> kValues = new List<double>();
            List<double> dAValues = new List<double>();
            List<double> dBValues = new List<double>();

            double partMin = 100000;
            double partMax = 0;

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
                double partCount = Convert.ToDouble(lineValues[7]);
                connectedProcentages.Add(partCount);
                solidEdgeProcentage.Add(Convert.ToDouble(lineValues[9]));

                if (partCount > partMax) partMax = partCount;
                if (partCount < partMin) partMin = partCount;

                valuesCount++;
            }

            for (int i = 0; i < connectedProcentages.Count; i++)
            {
                connectedProcentages[i] = ReMap(connectedProcentages[i], partMin, partMax, 0, 1);
            }

            PopulateVaribleTree(dAValues, dBValues, fValues, kValues);
        }

        private void InitAll()
        {
            valuesCount = 0;
            massProcentages = new List<double>();
            connectedProcentages = new List<double>();
            edgeConnectionProcentages = new List<double>();
            solidEdgeProcentage = new List<double>();
            types = new List<string>();
            varibles = new DataTree<double>();
        }

        private void PopulateVaribleTree(List<double> dAValues, List<double> dBValues, List<double> fValues, List<double> kValues)
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

        private double ReMap(double value, double currentMin, double currentMax, double newMin, double newMax)
        {
            double reMaped = newMin + (newMax - newMin) * ((value - currentMin) / (currentMax - currentMin));

            return reMaped;
        }

        public int SelectIndex(double weightMass, double weightConnection, double weightEdgeConnection, double weightSolidEdge)
        {
            Dictionary<int, double> dictonary = new Dictionary<int, double>();
            //List<KeyValuePair<int, double>> weightedList = new List<KeyValuePair<int, double>>();

            for (int i = 0; i < valuesCount; i++)
            {
                double weightedNr = (massProcentages[i] * weightMass) +
                                     (connectedProcentages[i] * weightConnection) +
                                     (edgeConnectionProcentages[i] * weightEdgeConnection) +
                                     (solidEdgeProcentage[i] * weightSolidEdge);

                dictonary.Add(i, weightedNr);
            }

            List<KeyValuePair<int, double>> weightedList = new List<KeyValuePair<int, double>>(dictonary);
            weightedList.Sort(
                delegate (KeyValuePair<int, double> firstPair,
                KeyValuePair<int, double> nextPair)
                {
                    return firstPair.Value.CompareTo(nextPair.Value);
                }
            );

            return weightedList[0].Key;
        }
    }
}