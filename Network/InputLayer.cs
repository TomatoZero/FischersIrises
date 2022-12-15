using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using static System.Double;

namespace NeuralNetwork
{
    public class InputLayer
    {
        private List<double[]> _data;
        private List<double[]> _expectedResult;
        
        public List<double[]> Data { get => _data; private set => _data = value; }
        public List<double[]> ExpectedResult { get => _expectedResult; private set => _expectedResult = value; }
        
        public InputLayer(string path, bool trainingData) {
            Data = new List<double[]>();
            ExpectedResult = new List<double[]>();
            GetData(path, trainingData);
        }

        private void GetData(string path, bool trainingData) {
            using (var streamReader = new StreamReader(path, Encoding.UTF8)) {
                while (!streamReader.EndOfStream) {
                    var line = streamReader.ReadLine()?.Trim().Split(" ");

                    if (line == null) continue;

                    var data = new List<double>();
                    for (var j = 0; j < line.Count() - 1; j++)
                        data.Add(Parse(line[j].Trim()));
                    Data.Add(data.ToArray());
                    var count = data.Count;

                    // if (trainingData) {
                    //     var result = new double[2];
                    //     for (var j = data.Length; j < line.Length; j++) 
                    //         result[j - 2] = Parse(line[j].Trim());
                    //     ExpectedResult.Add(result);
                    // }
                    //
                    // if(trainingData)
                    //     switch (line[count].Trim()) {
                    //         case "setosa":
                    //             ExpectedResult.Add(new double[3]{1, 0, 0});
                    //             // ExpectedResult.Add(new double[]{1});
                    //             break;
                    //         case "versicolor":
                    //             ExpectedResult.Add(new double[3]{0, 1, 0});
                    //             // ExpectedResult.Add(new double[]{0});
                    //             break;
                    //         case "virginica":
                    //             ExpectedResult.Add(new double[3]{0, 0, 1});
                    //             // ExpectedResult.Add(new double[]{0});
                    //             break;
                    //         default:
                    //             throw new NotImplementedException();
                    //     }
                    
                    
                    if(trainingData)
                        switch (line[count].Trim()) {
                            case "Pickup":
                                ExpectedResult.Add(new double[] { 1, 0, 0});
                                break;
                            // case "SUV":
                            //     ExpectedResult.Add(new double[] { 1, 0});
                            //     break;
                            case "Sedan":
                                ExpectedResult.Add(new double[] { 0, 1, 0});
                                break;
                            case "Cabriolet":
                                ExpectedResult.Add(new double[] { 0, 0, 1 });
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                }
            }
        }
    }
}