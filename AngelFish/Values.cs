using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using Grasshopper;
using Grasshopper.Kernel.Data;

namespace Angelfish
{

    public class Values
    {
        private int valuesCount;
        private List<double> massProcentages;
        private List<double> connectivityRate;
        private List<double> edgeConnectionProcentages;
        private List<double> solidEdgeProcentage;
        private List<string> types;
        private DataTree<double> varibles;
   
        public double WeightedValue; 
        public int ValuesCount { get { return valuesCount; } }
        public List<double> MassProcentages { get { return massProcentages; } }
        public List<double> ConnectedProcentages { get { return connectivityRate; } }
        public List<double> EdgeConnectionProcentages { get { return edgeConnectionProcentages; } }
        public List<double> SolidEdgeProcentage { get { return solidEdgeProcentage; } }
        public List<string> Types { get { return types; } }
        public DataTree<double> Varibles { get { return varibles; } }

        public Values()
        {
            InitAll();
        }

        public Values(string file, bool measured)
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

                if(measured)
                {
                    massProcentages.Add(Convert.ToDouble(lineValues[4]));
                    edgeConnectionProcentages.Add(Convert.ToDouble(lineValues[5]));
                    types.Add(lineValues[6]);
                    double partCount = Convert.ToDouble(lineValues[7]);
                    connectivityRate.Add(partCount);
                    solidEdgeProcentage.Add(Convert.ToDouble(lineValues[9]));

                    if (partCount > partMax) partMax = partCount;
                    if (partCount < partMin) partMin = partCount;
                }

                valuesCount++;
            }

            if(measured)
            {
                for (int i = 0; i < connectivityRate.Count; i++)
                {
                    connectivityRate[i] = ReMap(connectivityRate[i], partMin, partMax, 0, 1);
                }
            }

            PopulateVaribleTree(dAValues, dBValues, fValues, kValues);
        }

        private void InitAll()
        {
            valuesCount = 0;
            massProcentages = new List<double>();
            connectivityRate = new List<double>();
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

        public List<int> SelectIndex(double weightMass, double weightConnection, double weightEdgeConnection, double weightSolidEdge, double addRange)
        {
            Dictionary<int, double> dictonary = new Dictionary<int, double>();
            for (int i = 0; i < valuesCount; i++)
            {
                double remapMass = ReMap(massProcentages[i], 0, 1, 1, 0);
                double weightedNr = (remapMass * weightMass) +
                                     (connectivityRate[i] * weightConnection) +
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

            weightedList.Reverse();
            List<int> allIndex = new List<int>();

            double compareTo = weightedList[0].Value - addRange;
            WeightedValue = compareTo;

            bool checking = true;
            int j = 0;

            while (checking)
            {
                if (weightedList[j].Value >= compareTo)
                {
                    allIndex.Add(weightedList[j].Key);
                    j++;
                }


                else checking = false;

            }

            return allIndex;
        }


    }
}