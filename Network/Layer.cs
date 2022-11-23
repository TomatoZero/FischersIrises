using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    internal abstract class Layer
    {
        protected int _numNeurons;
        protected int _numPrevNeurons;
        protected const double _learniingRate = 0.001d;
        protected Neuron[] _neurons;

        private string _weightFilePath;

        public string WeightFilePath => _weightFilePath;
        
        public Neuron[] Neurons { get => _neurons; private set => _neurons = value; }
        public double[] Data {
            set {
                for (int i = 0; i < Neurons.Length; i++)
                    Neurons[i].Inputs = value;
            }
        }

        public Layer(int numNeurons, int numPrevNeurons, NeuronType neurontype, string weightFilePath, bool randomWeights = false)
        {
            _numNeurons = numNeurons;
            _numPrevNeurons = numPrevNeurons;
            Neurons = new Neuron[numNeurons];
            _weightFilePath = weightFilePath;
            var weights = randomWeights == true ? RandomWeights() : WeightsInitialize(MemoryMod.Get);

            for (int i = 0; i < numNeurons; ++i) 
            {
                var tempWeights = new double[numPrevNeurons];

                for(int j = 0; j < numPrevNeurons; ++j) 
                    tempWeights[j] = weights[i, j];

                Neurons[i] = new Neuron(null, tempWeights, neurontype);
            }
        }
        
        private double[,] RandomWeights() {
            var rnd = new Random();
            var weights = new double[_numNeurons, _numPrevNeurons];
            
            for (int i = 0; i < _numNeurons; i++)
                for (int j = 0; j < _numPrevNeurons; j++)
                    weights[i, j] = rnd.NextDouble() * (.5 + .5) - 0.5;

            return weights;
        }

        public double[,] WeightsInitialize(MemoryMod mod) {
            var weights = new double[_numNeurons, _numPrevNeurons];

            if (mod == MemoryMod.Get) {
                using var sr = new StreamReader(_weightFilePath, Encoding.UTF8);
                var i = 0;
                while (!sr.EndOfStream) {
                    var line = sr.ReadLine()?.Trim().Split();
                        
                    if (line == null) continue;

                    for (var k = 0; k < _numPrevNeurons; k++)
                        weights[i,k] = Double.Parse(line[k]);

                    i++;
                }

                if (i != _numNeurons)
                    throw new NotImplementedException();
            }
            else {
                using var sw = new StreamWriter(_weightFilePath, false, Encoding.UTF8);
                for (var i = 0; i < _numNeurons; i++) {    
                    for(var j = 0; j < _numPrevNeurons; j++)
                        sw.Write($"{Neurons[i].Weights[j]} ");
                    sw.Write("\n");
                }
            }
            
            return weights;
        }
        public double[] Recognize() {
            var output = new double[_numNeurons];
            for (int i = 0; i < Neurons.Length; i++) {
                output[i] = Neurons[i].Output;
            }

            return output;
        }

        public abstract double[] BackwardPass(double[] stuff, bool calculateGradient);
    }
}
