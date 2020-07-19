using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using Grasshopper;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Data;
using System.Linq;
using Grasshopper.Kernel;

namespace Angelfish
{

    public class ReadWrite
    {
        public GH_Structure<GH_Number> Varibles;
        private int valuesCount;
        private List<double> massPercentage;
        private List<double> connectedPercentage;
        private List<double> solidEdgePercentage;

  
        string path;
        bool measured; 
        public double WeightedValue;

        public ReadWrite()
        {
            InitAll();
        }

        public ReadWrite(string _string, bool _readFile)
        {
            InitAll();

            if (_readFile) Read(_string);

            else path = _string;
        }

        private void InitAll()
        {
            valuesCount = 0;
            massPercentage = new List<double>();
            connectedPercentage = new List<double>();
            solidEdgePercentage = new List<double>();
            Varibles = new GH_Structure<GH_Number>();
        }

        private void Read(string file)
        {
            //string file = File.ReadAllText(path);

            List<double> fValues = new List<double>();
            List<double> kValues = new List<double>();
            List<double> dAValues = new List<double>();
            List<double> dBValues = new List<double>();

            string[] lines = file.Split('\n');

            foreach (string line in lines)
            {
                string fixedLine = line.Replace(',', '.');
                string[] lineValues = fixedLine.Split('\t');

                int len = lineValues.Length;

                dAValues.Add(Convert.ToDouble(lineValues[0]));
                dBValues.Add(Convert.ToDouble(lineValues[1]));
                fValues.Add(Convert.ToDouble(lineValues[2]));
                kValues.Add(Convert.ToDouble(lineValues[3]));

                if (lineValues.Length > 6) measured = true;
                else measured = false; 
                if (measured)
                {
                    massPercentage.Add(Convert.ToDouble(lineValues[4]));
                    connectedPercentage.Add(Convert.ToDouble(lineValues[5]));
                    solidEdgePercentage.Add(Convert.ToDouble(lineValues[6]));
                }

                valuesCount++;
            }

            PopulateVaribleTree(dAValues, dBValues, fValues, kValues);
        }

        public void WriteToFile(List<double> _varibles)
        {
            Random rand = new Random();
            DateTime dT = DateTime.Now;
            string name = dT.ToString("yyyyMMddHHmmss") + rand.Next(0, 999).ToString();

            string toWrite = _varibles[0].ToString() + '\t' +
                _varibles[1].ToString() + '\t' +
                _varibles[2].ToString() + '\t' +
                _varibles[3].ToString() + '\n';

            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(toWrite);
            }
        }
        public void WriteToFile(List<double>_varibles, double _massP, double _connectedP, double _solidEdgeP)
        {
            string toWrite = _varibles[0].ToString() + '\t' +
                _varibles[1].ToString() + '\t' +
                _varibles[2].ToString() + '\t' +
                _varibles[3].ToString() + '\t' +
                _massP.ToString() + '\t' +
                _connectedP.ToString() + '\t' +
                _solidEdgeP.ToString() + '\n';

            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(toWrite);
            }
        }

        public void Embedd(List<double> _varibles, double _massP, double _connectedP, double _solidEdgeP)
        {
            string toWrite = _varibles[0].ToString() + '\t' +
                _varibles[1].ToString() + '\t' +
                _varibles[2].ToString() + '\t' +
                _varibles[3].ToString() + '\t' +
                _massP.ToString() + '\t' +
                _connectedP.ToString() + '\t' +
                _solidEdgeP.ToString() + '\n';

            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(toWrite);
            }
        }

        private void PopulateVaribleTree(List<double> dAValues, List<double> dBValues, List<double> fValues, List<double> kValues)
        {

            for (int i = 0; i < valuesCount; i++)
            {
                List<GH_Number> allVaribles = new List<GH_Number>();
                allVaribles.Add(new GH_Number(dAValues[i]));
                allVaribles.Add(new GH_Number(dBValues[i]));
                allVaribles.Add(new GH_Number(fValues[i]));
                allVaribles.Add(new GH_Number(kValues[i]));

                Varibles.AppendRange(allVaribles, new GH_Path(i));
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
                double remapMass = ReMap(massPercentage[i], 0, 1, 1, 0);
                double weightedNr = (remapMass * weightMass) +
                                     (connectedPercentage[i] * weightConnection) +
                                     (solidEdgePercentage[i] * weightSolidEdge);

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