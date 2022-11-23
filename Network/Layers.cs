using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    internal class HideLayer : Layer
    {
        public HideLayer(int numNeurons, int numPrevNeurons, NeuronType neurontype, string weightFilePath, bool randomWeights = false) : base(numNeurons, numPrevNeurons, neurontype, weightFilePath, randomWeights)
        { }

        public override double[] BackwardPass(double[] gradientsSum, bool calculateGradient = false) {
            var thisNeuronsGradients = new double[_numNeurons];
            
            for (var i = 0; i < _numNeurons; i++) {
                var sumInputErrors = gradientsSum[i]; 
                var neuronOutput = Neurons[i].Output;
                var derivation = Neurons[i].Derevativator(neuronOutput);

                if (calculateGradient)
                    thisNeuronsGradients[i] = derivation * Neurons[i].Gradientor(0d, derivation, gradientsSum[i]);
                
                var magnitudeError = sumInputErrors * derivation;
                var deltaW = _learniingRate * magnitudeError * neuronOutput;

                for (var j = 0; j < Neurons[i].Weights.Length; j++)
                    Neurons[i].Weights[j] += deltaW;
            }

            if (calculateGradient) {
                var newGradientSum = new double[_numPrevNeurons];

                for (var i = 0; i < _numPrevNeurons; i++) {
                    var sum = 0d;
                    for (var j = 0; j < _numNeurons; j++) 
                        sum += _neurons[j].Weights[i] * gradientsSum[j];
                    
                    newGradientSum[i] = sum;
                }

                return newGradientSum;
            }
            
            return null;
        }


    }

    internal class OutputLayer : Layer {
        public bool _single = false;
        public OutputLayer(int numNeurons, int numPrevNeurons, NeuronType neurontype, string weightFilePath, bool randomWeights = false) : base(numNeurons, numPrevNeurons, neurontype, weightFilePath, randomWeights)
        { }

        public override double[] BackwardPass(double[] errors, bool calculateGradient = true) {
            if (_single)
                throw new Exception();
                
            var gradients = new double[_numNeurons];
            
            for (var i = 0; i < _numNeurons; i++) {
                var neuronOutput = Neurons[i].Output;
                var derivation = Neurons[i].Derevativator(neuronOutput);
                gradients[i] = Neurons[i].Gradientor(errors[i], derivation, 0d);

                var deltaW = _learniingRate * gradients[i] * neuronOutput;
                for (var j = 0; j < Neurons[i].Weights.Length; j++) {
                    Neurons[i].Weights[j] += deltaW;
                }
            }

            var gradientsSum = new double[_numPrevNeurons];
            for (var i = 0; i < _numPrevNeurons; i++) {
                var sum = 0d;
                for (var j = 0; j < _numNeurons; j++) 
                    sum += _neurons[j].Weights[i] * gradients[j];
                gradientsSum[i] = sum;
            }

            return gradientsSum;
        }

        public void ChangeWeights(double[] errors, double[] output, double[] data) {
            if (!_single)
                throw new Exception();

            for (var i = 0; i < _numNeurons; i++) {
                for (var j = 0; j < _numPrevNeurons; j++) {
                    _neurons[i].Weights[j] += _learniingRate * errors[i] * data[j];
                }
            }
        }

        public void ChangeWeights(double deltaL) {
            if (!_single)
                throw new Exception();

            for (var i = 0; i < _numNeurons; i++) {
                for (var j = 0; j < _numPrevNeurons; j++) {
                    _neurons[i].Weights[j] -= _learniingRate * deltaL;
                }
            }
        }
    }
}
