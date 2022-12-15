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
            
            // var inputLayer = new InputLayer($"fishersIrises/{nameof(InputLayer)}.txt", true);
            // var hideLayers = new HideLayer[4];
            // for (var i = 0; i < hideLayers.Length; i++)
            //     hideLayers[i] = new HideLayer(5,  i == 0 ? inputLayer.Data[i].Length : hideLayers[i - 1].Neurons.Length, NeuronType.Hiden,
            //         $"fishersIrises/{nameof(hideLayers)}{i}.txt", true);
            // var outputLayer = new OutputLayer(3, hideLayers[hideLayers.Length - 1].Neurons.Length, NeuronType.Output,
            //     $"fishersIrises/{nameof(OutputLayer)}.txt", true);
            //
            // var fisherNet = new Network(inputLayer, hideLayers, outputLayer);
            // fisherNet.NewTrainingMulti();
            // fisherNet.Test($"fishersIrises/InputData.txt");
            //     
            
            
            // var inputLayer = new InputLayer($"fishersIrisesSingle/{nameof(InputLayer)}.txt", true);
            // var outputLayer = new OutputLayer(1, inputLayer.Data[1].Length, NeuronType.Output,
            //     $"fishersIrisesSingle/{nameof(OutputLayer)}.txt", true);
            //
            // var fisherNet = new Network(inputLayer, null, outputLayer);
            // fisherNet.Training();
            // fisherNet.Test($"fishersIrisesSingle/InputLayer.txt");
            //
            //
            
            // var inputLayer = new InputLayer($"XOR/InputLayer.txt", true);
            // var outputLayer = new OutputLayer(2, inputLayer.Data[1].Length, NeuronType.Output, $"XOR/OutputLayer.txt");
            //
            // var xorNetwork = new Network(inputLayer, null, outputLayer);
            // // xorNetwork.Training();
            // xorNetwork.Test($"XOR/InputData.txt");

            //
            
            var inputLayer = new InputLayer($"Cars/{nameof(InputLayer)}.txt", true);
            var hideLayers = new HideLayer[3];
            for (var i = 0; i < hideLayers.Length; i++)
                hideLayers[i] = new HideLayer(7,  i == 0 ? inputLayer.Data[i].Length : hideLayers[i - 1].Neurons.Length, NeuronType.Hiden,
                    $"Cars/{nameof(hideLayers)}{i}.txt", true);
            var outputLayer = new OutputLayer(3, hideLayers[0].Neurons.Length, NeuronType.Output,
                $"Cars/{nameof(OutputLayer)}.txt", true);
            
            var carsNet = new Network(inputLayer, hideLayers, outputLayer);
            carsNet.TrainingMulti();
            carsNet.Test($"Cars/InputData.txt");

        }
    }
}
