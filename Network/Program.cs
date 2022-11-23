using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NeuralNetwork
{
    internal class Program
    {
        static void Main(string[] args) {
            // fishersIrises

            // var s = Read();
            // Write(s);
            
            

            var inputLayer = new InputLayer($"fishersIrises/{nameof(InputLayer)}.txt", true);
            var hideLayers = new HideLayer[10];
            for (var i = 0; i < hideLayers.Length; i++)
                hideLayers[i] = new HideLayer(7,  i == 0 ? inputLayer.Data[i].Length : hideLayers[i - 1].Neurons.Length, NeuronType.Hiden,
                    $"fishersIrises/{nameof(hideLayers)}{i}.txt", true);
            var outputLayer = new OutputLayer(3, hideLayers[1].Neurons.Length, NeuronType.Output,
                $"fishersIrises/{nameof(OutputLayer)}.txt", true);
            
            var fisherNet = new Network(inputLayer, hideLayers, outputLayer);
            fisherNet.Training();

            //
            // var inputLayer = new InputLayer($"fishersIrisesSingle/{nameof(InputLayer)}.txt", true);
            // var outputLayer = new OutputLayer(1, inputLayer.Data[1].Length, NeuronType.Output,
            //     $"fishersIrisesSingle/{nameof(OutputLayer)}.txt", true);
            //
            // var fisherNet = new Network(inputLayer, null, outputLayer);
            // fisherNet.Training();
            // fisherNet.Test($"fishersIrisesSingle/InputData.txt");
            //
            //
            
            // var inputLayer = new InputLayer($"XOR/InputLayer.txt", true);
            // var outputLayer = new OutputLayer(2, inputLayer.Data[1].Length, NeuronType.Output, $"XOR/OutputLayer.txt");
            //
            // var xorNetwork = new Network(inputLayer, null, outputLayer);
            // // xorNetwork.Training();
            // xorNetwork.Test($"XOR/InputData.txt");



        }

        public static List<string> Read() {
            var str = new List<string>();
            using (var reader = new StreamReader("fishersIrises/InputLayer.txt", Encoding.UTF8)) {
                while (!reader.EndOfStream) {
                    str.Add(reader.ReadLine());
                }
            }

            return str;
        }


        public static void Write(List<string> list)  {
            using (var writer = new StreamWriter("fishersIrises/InputLayer.txt", false)) {
                foreach (var str in list) {
                    string str2 = "";

                    foreach (var ch in str) {
                        if (ch == '.')
                            str2 += ',';
                        else
                            str2 += ch;
                    }
                    
                    writer.WriteLine(str);
                }
            }
        }
    }
}
