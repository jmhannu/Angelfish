using System;
using System.IO;
using System.Resources;
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
        private GH_Structure<GH_Number> varibles;
        private int valuesCount;
        private List<double> massPercentage;
        private List<double> connectedPercentage;
        private List<double> solidEdgePercentage;
        private List<string> names;

        public List<string> printOut;
        public string printOne;
        public List<int> printNumber;
        public GH_Structure<GH_String> printStructure; 

        public GH_Structure<GH_Number> Varibles { get { return varibles; } }
        public List<double> MassPercentage { get { return massPercentage; } }
        public List<double> ConnectedPercentage { get { return connectedPercentage; } }
        public List<double> SolidEdgePercentage { get { return solidEdgePercentage; } }

        public List<string> Names { get { return names; } }

        public double WeightedValue;

        public ReadWrite()
        {
            InitAll();
        }

        private void InitAll()
        {
            valuesCount = 0;
            massPercentage = new List<double>();
            connectedPercentage = new List<double>();
            solidEdgePercentage = new List<double>();
            names = new List<string>();
            varibles = new GH_Structure<GH_Number>();
            WeightedValue = 0.0;
        }


        public void ReadLegacy(string _file)
        {
            List<double> fValues = new List<double>();
            List<double> kValues = new List<double>();
            List<double> dAValues = new List<double>();
            List<double> dBValues = new List<double>();

            string[] lines = _file.Split('\n');

            foreach (string line in lines)
            {
                string fixedLine = line.Replace(',', '.');
                string[] lineValues = fixedLine.Split('\t');

                dAValues.Add(Convert.ToDouble(lineValues[0]));
                dBValues.Add(Convert.ToDouble(lineValues[1]));
                fValues.Add(Convert.ToDouble(lineValues[2]));
                kValues.Add(Convert.ToDouble(lineValues[3]));

                if (lineValues.Length > 6)
                {
                    massPercentage.Add(Convert.ToDouble(lineValues[4]));
                    connectedPercentage.Add(Convert.ToDouble(lineValues[5]));
                    solidEdgePercentage.Add(Convert.ToDouble(lineValues[6]));
                }

                valuesCount++;
            }

            PopulateVaribleTree(dAValues, dBValues, fValues, kValues);
        }


        public void Read(string _file)
        {
            List<double> fValues = new List<double>();
            List<double> kValues = new List<double>();
            List<double> dAValues = new List<double>();
            List<double> dBValues = new List<double>();

            string[] lines = _file.Split('\n');
            printOut = new List<string>();
            printNumber = new List<int>();
            printOne = _file;
            printStructure = new GH_Structure<GH_String>();
            List<GH_String> toStructure = new List<GH_String>();
            int index = 0; 
            foreach (string line in lines)
            {
                string[] lineValues = line.Split('\t');

                names.Add(lineValues[0]);
                printNumber.Add(lineValues.Length);
;
               printOut.Add(line);

                for (int i = 0; i < lineValues.Length; i++)
                {
                    toStructure.Add(new GH_String(lineValues[i]));
                }

                printStructure.AppendRange(toStructure, new GH_Path(index));
                index++;

                if(lineValues.Length > 4)
                {
                    if (lineValues[1] == "NaN") dAValues.Add((double)-999.999);
                    else dAValues.Add(double.Parse(lineValues[1]));
                    if (lineValues[2] == "NaN") dBValues.Add((double)-999.999);
                    else dBValues.Add(Convert.ToDouble(lineValues[2]));

                    if (lineValues[3] == "NaN") fValues.Add((double)-999.999);
                    else fValues.Add(double.Parse(lineValues[3]));

                    if (lineValues[4] == "NaN") kValues.Add((double)-999.999);
                    else kValues.Add(double.Parse(lineValues[4]));

                    if (lineValues.Length > 7)
                    {
                        if (lineValues[5] == "NaN") massPercentage.Add((double)-999.999);
                        else massPercentage.Add(double.Parse(lineValues[5]));

                        if (lineValues[6] == "NaN") connectedPercentage.Add((double)-999.999);
                        else connectedPercentage.Add(double.Parse(lineValues[6]));

                        if (lineValues[7] == "NaN") solidEdgePercentage.Add((double)-999.999);
                        else solidEdgePercentage.Add(double.Parse(lineValues[7]));
                    }

                    valuesCount++;
                }

            }

            PopulateVaribleTree(dAValues, dBValues, fValues, kValues);
        }

        public void WriteToFile(List<double> _varibles, string _path)
        {
            string toWrite = _varibles[0].ToString() + '\t' +
                _varibles[1].ToString() + '\t' +
                _varibles[2].ToString() + '\t' +
                _varibles[3].ToString() + '\n';

            using (StreamWriter sw = File.AppendText(_path))
            {
                sw.WriteLine(toWrite);
            }
        }
        public void WriteToFile(List<double> _varibles, double _massP, double _connectedP, double _solidEdgeP, string _path)
        {
            string toWrite = _varibles[0].ToString() + '\t' +
                _varibles[1].ToString() + '\t' +
                _varibles[2].ToString() + '\t' +
                _varibles[3].ToString() + '\t' +
                _massP.ToString() + '\t' +
                _connectedP.ToString() + '\t' +
                _solidEdgeP.ToString() + '\n';

            using (StreamWriter sw = File.AppendText(_path))
            {
                sw.WriteLine(toWrite);
            }
        }

        public string Embedd(List<double> _varibles)
        {
            Random rand = new Random();
            DateTime dT = DateTime.Now;
            string name = dT.ToString("yyyyMMddHHmmss") + rand.Next(0, 999).ToString();

            string toWrite = name + '\t' +
                _varibles[0].ToString() + '\t' +
                _varibles[1].ToString() + '\t' +
                _varibles[2].ToString() + '\t' +
                _varibles[3].ToString();

            using (StreamWriter sw = File.AppendText("C:/Users/julia/OneDrive/Dokument/GitHub/Angelfish/Angelfish/Resources/inputs2D.txt"))
            {
                sw.WriteLine(toWrite);
            }

            return name;
        }

        public string Embedd(List<double> _varibles, double _massP, double _connectedP, double _solidEdgeP)
        {
            Random rand = new Random();
            DateTime dT = DateTime.Now;
            string name = dT.ToString("yyyyMMddHHmmss") + rand.Next(0, 999).ToString();

            string toWrite = name + '\t' +
                _varibles[0].ToString() + '\t' +
                _varibles[1].ToString() + '\t' +
                _varibles[2].ToString() + '\t' +
                _varibles[3].ToString() + '\t' +
                _massP.ToString() + '\t' +
                _connectedP.ToString() + '\t' +
                _solidEdgeP.ToString();

            using (StreamWriter sw = File.AppendText("C:/Users/julia/OneDrive/Dokument/GitHub/Angelfish/Angelfish/Resources/Embedded2D.txt"))
            {
                sw.WriteLine(toWrite);
            }

            return name;
        }

        public List<int> ToPreview(string _name)
        {
            List<int> read = new List<int>();

            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = "Angelfish.Resources." + _name + ".txt";
            string file;

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();
                file = result;
            }

            string[] lineValues = file.Split('\t');

            for (int i = 0; i < lineValues.Length; i++)
            {
                read.Add(Convert.ToInt32(lineValues[i]));
            }

            return read;
        }

        public void WritePattern(string _name, Asystem _pattern)
        {
            string toWrite = null;

            for (int i = 0; i < _pattern.asize; i++)
            {
                if (i != 0) toWrite += '\t';
                if (_pattern.Apoints[i].InPattern) toWrite += "1";
                else toWrite += "0";

            }

            string path = "C:/Users/julia/OneDrive/Dokument/GitHub/Angelfish/Angelfish/Resources/" + _name + ".txt";
            File.WriteAllText(path, toWrite);
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

                varibles.AppendRange(allVaribles, new GH_Path(i));
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