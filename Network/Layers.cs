using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    internal class HideLayer : Layer
    {
        public HideLayer(int numNeurons, int numPrevNeurons, NeuronType neurontype, string weightFilePath, 
            bool randomWeights = false) : base(numNeurons, numPrevNeurons, neurontype, weightFilePath, randomWeights)
        { }

        public override double[] BackProp(double[] deDh) {
            var deDt = new double[_numNeurons];

            for (int i = 0; i < deDh.Length; i++) 
                deDt[i] = deDh[i] * Neurons[i].Derevativator(Neurons[i].AdderResult);
            
            var deDx = new double[_numPrevNeurons];
            for (int i = 0; i < _numPrevNeurons; i++) {
                var sum = 0d;
                for (int j = 0; j < _numNeurons; j++) 
                    sum += deDt[j] * Neurons[j].Weights[i];
                deDx[i] = sum;
            }
            
            
            for (int i = 0; i < _numNeurons; i++) {
                var neuron = Neurons[i];
                var deDw = 0d;
                for (int j = 0; j < _numPrevNeurons; j++) {
                    deDw = deDt[i] * neuron.Inputs[j];
                    neuron.Weights[j] -= _learniingRate * deDw;
                }
            }
            
            return deDx;
        }
    }

    internal class OutputLayer : Layer {
        public bool _single = false;
        public OutputLayer(int numNeurons, int numPrevNeurons, NeuronType neurontype, string weightFilePath, 
            bool randomWeights = false) : base(numNeurons, numPrevNeurons, neurontype, weightFilePath, randomWeights)
        { }

        public override double[] BackProp(double[] expectedResult) {
            var deDh = VectorGradient(expectedResult);
            var deDt = deDh;
            
            var deDx = new double[_numPrevNeurons];
            for (int i = 0; i < _numPrevNeurons; i++) {
                var sum = 0d;
                for (int j = 0; j < _numNeurons; j++) 
                    sum += deDt[j] * Neurons[j].Weights[i];
                deDx[i] = sum;
            }

            for (int i = 0; i < _numNeurons; i++) {
                var neuron = Neurons[i];
                var deDw = 0d;
                for (int j = 0; j < _numPrevNeurons; j++) {
                    deDw = deDt[i] * neuron.Inputs[j];
                    neuron.Weights[j] -= _learniingRate * deDw;
                }
            }
            
            return deDx;
        }

        private double[] VectorGradient(double[] expectedResult) {
            var adderResult = new double[_numNeurons];

            for (int i = 0; i < _numNeurons; i++) 
                adderResult[i] = Neurons[i].AdderResult;
            var z = Softmax(adderResult);
            var gradient = new double[_numNeurons];
            for (int i = 0; i < _numNeurons; i++) 
                gradient[i] = z[i] - expectedResult[i];

            return gradient;
        }
        
        public void ChangeWeights(double[] errors, double[] data) {
            if (!_single)
                throw new Exception();

            for (var i = 0; i < _numNeurons; i++) 
                for (var j = 0; j < _numPrevNeurons; j++) 
                    _neurons[i].Weights[j] += _learniingRate * errors[i] * data[j];
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
