using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using RNGCryptoServiceProvider = System.Security.Cryptography.RNGCryptoServiceProvider;

namespace NeuralNetwork
{
    internal class Network
    {
        private InputLayer _inputLayer;
        private HideLayer[] _hideLayers;
        private OutputLayer _outputLayer;

        private bool _single;
        
        public Network(InputLayer inputLayer, HideLayer[] hideLayers, OutputLayer outputLayer) {
            _inputLayer = inputLayer;
            _outputLayer = outputLayer;
            
            _hideLayers = hideLayers;
            _single = hideLayers == null ? true : false;
            _outputLayer._single = _single;
        }

        public void Training() {
            
            if (_single)
                TrainingBinary();
            else
                TrainingMulti();
        }

        private void TrainingBinary() {
            var data = CopyList(_inputLayer.Data);
            var expectedResult = CopyList(_inputLayer.ExpectedResult);
            var averageE = 0d;
            var errorsEnergy = new double[_inputLayer.Data.Count];
            var predicted = new double[_inputLayer.Data.Count][];
            var epohCount = 0;
            var errors = new double[_outputLayer.Neurons.Length];

            do {
                RandomizeData(data, expectedResult);
                for (var i = 0; i < _inputLayer.Data.Count; i++) {
                    _outputLayer.Data = data[i];
                    var output = _outputLayer.Recognize();
                    predicted[i] = output;
                    
                    if (output.Length != expectedResult[i].Length)
                        throw new NotImplementedException();
                    
                    errors = new double[output.Length];
                    for (var j = 0; j < errors.Length; j++) {
                        errors[j] = expectedResult[i][j] - output[j];
                    }
                    errorsEnergy[i] = TotalErrorEnergy(errors);
                    _outputLayer.ChangeWeights(errors, data[i]);
                }
                
                averageE = AverageErrorEnergy(errorsEnergy);
                if(epohCount % 1000 == 0)
                    Console.WriteLine($"Epoch {epohCount} end. Average Energy {averageE}");

                epohCount++;
            } while (averageE > 0.1);
            _outputLayer.WeightsInitialize(MemoryMod.Set);
            Console.WriteLine($"End Learning. Epoch count {epohCount}. Average Energy {averageE}");
        }
        
           public void TrainingMulti() {
            var data = CopyList(_inputLayer.Data);
            var expectedResult = CopyList(_inputLayer.ExpectedResult);
            var averageE = 0d;
            var minAbsoluteDeviation = 0d;
            var errorsEnergy = new double[_inputLayer.Data.Count];
            var absError = new double[_inputLayer.Data.Count];
            var predicted = new double[_inputLayer.Data.Count][];
            var epohCount = 0;

            do {
                data = RandomizeData(data, expectedResult);
                for (var i = 0; i < _inputLayer.Data.Count; i++) {
                    var tempFormHidden = data[i];
                    for (var j = 0; j < _hideLayers.Length; j++) {
                        _hideLayers[j].Data = tempFormHidden;
                        tempFormHidden = _hideLayers[j].Recognize();
                    }
                    
                    _outputLayer.Data = tempFormHidden;
                    var output = _outputLayer.Recognize();
                    predicted[i] = output;
                    
                    if (output.Length != expectedResult[i].Length)
                        throw new NotImplementedException();
                    
                    var errors = new double[output.Length];
                    for (var j = 0; j < errors.Length; j++) {
                        errors[j] = expectedResult[i][j] - output[j];
                    }

                    var softmaxError = Softmax(errors);
          
                    errorsEnergy[i] = TotalErrorEnergy(softmaxError);
                    absError[i] = MinAbsoluteDeviation(softmaxError);
                    
                    var gradientSum = _outputLayer.BackProp(expectedResult[i]);
                    for (int j = _hideLayers.Length - 1; j >= 0; j--) {
                        gradientSum = _hideLayers[j].BackProp(gradientSum);
                    }
                }

                averageE = AverageErrorEnergy(errorsEnergy);
                minAbsoluteDeviation = AverageErrorEnergy(absError);

                if(epohCount % 20 == 0)
                    Console.WriteLine($"Epoch {epohCount} end. Average Energy {averageE}. Min absolute deviation {minAbsoluteDeviation}");

                if(epohCount >= 500)
                    break;
                epohCount++;
            } while (averageE > 0.1 || minAbsoluteDeviation > 0.1);
            _outputLayer.WeightsInitialize(MemoryMod.Set);
            
            foreach (var t in _hideLayers)
                t.WeightsInitialize(MemoryMod.Set);

            Console.WriteLine($"End Learning. Epoch count {epohCount} ");
        }
        
        protected double[] Softmax(double[] t) {
            var exp = new double[t.Length];
            for (int i = 0; i < t.Length; i++) 
                exp[i] = Math.Exp(t[i]);
            var sum = exp.Sum();

            var softmax = new double[exp.Length];
            for (int i = 0; i < exp.Length; i++)
                softmax[i] = exp[i] / sum;
            return softmax;
        }
        
        public void Test(string dataPath) {
            _inputLayer = new InputLayer(dataPath, true);
            
            if(_single) {
                TestSingle();
                _outputLayer = new OutputLayer(_outputLayer.Neurons.Length, _inputLayer.Data[1].Length,
                    NeuronType.Output,
                    _outputLayer.WeightFilePath);
            }else {
                for (var i = 0; i < _hideLayers.Length; i++) {
                    _hideLayers[i] = new HideLayer(_hideLayers[i].Neurons.Length, i == 0 ? _inputLayer.Data[1].Length : _hideLayers[i].Neurons.Length,
                        NeuronType.Hiden, _hideLayers[i].WeightFilePath);
                }
                            
                _outputLayer = new OutputLayer(_outputLayer.Neurons.Length, _hideLayers[_hideLayers.Length - 1].Neurons.Length,
                    NeuronType.Output,
                    _outputLayer.WeightFilePath);

                TestMulti();
            }
        }


        private void TestSingle() {
            for (var i = 0; i < _inputLayer.Data.Count - 1; i++) {
                _outputLayer.Data = _inputLayer.Data[i];
                var output = _outputLayer.Recognize();
                bool isCorrect = false;
                var errors = new double[output.Length];
                var expectedResult = _inputLayer.ExpectedResult[i];
                for (var j = 0; j < errors.Length; j++) {
                    errors[j] = expectedResult[j] - output[j];
                    output[j] *= 10;
                    if ((expectedResult[j] == 1 && output[j] > 0.5) || (expectedResult[j] == 0 && output[j] < 0.5)) 
                        isCorrect = true;
                }
               
                Console.WriteLine($"Is correct: {isCorrect}. Output {string.Join(' ', output)}. Expected {string.Join(' ', expectedResult)}");
            }
        }
        
        private void TestMulti() {
            
            for (var i = 0; i < _inputLayer.Data.Count; i++) {
                var tempFormHidden = _inputLayer.Data[i];

                // Console.WriteLine($"Data {String.Join(' ', tempFormHidden)}");
                
                for (var j = 0; j < _hideLayers.Length; j++) {
                    _hideLayers[j].Data = tempFormHidden;
                    tempFormHidden = _hideLayers[j].Recognize();
                }
                    
                _outputLayer.Data = tempFormHidden;
                var output = _outputLayer.Recognize();

                output = Softmax(output);
                
                bool isCorrect = false;
                var errors = new double[output.Length];
                var expectedResult = _inputLayer.ExpectedResult[i];
                var max = output.Max();
                var maxId = 0;
                for (var j = 0; j < errors.Length; j++) {
                    errors[j] = expectedResult[j] - output[j];

                    if (output[j] == max && max != 0) {
                        if(expectedResult[j] == 1) isCorrect = true;
                        maxId = j;
                    }
                }
                Console.WriteLine($"{i} Is correct: {isCorrect}. Output {string.Join(' ', output)}. Expected {string.Join(' ', expectedResult)}. Max id {maxId + 1}");
            }
        }
        
        private double TotalErrorEnergy(double[] errors) {
            if (errors == null)
                throw new NullReferenceException();
            if (errors.Length != _inputLayer.ExpectedResult[0].Length)
                throw new ArgumentException();
            
            double E = 0;
            foreach (var error in errors) 
                E += error * error;
            
            //TODO:
            
            return E * 0.5;
        }

        private double MinAbsoluteDeviation(double[] absError) {
            if (absError == null)
                throw new NullReferenceException();

            var sum = 0d;
            foreach (var error in absError) {
                sum += Math.Abs(error);
            }
            
            return sum / absError.Length;
        }
        
        private double AverageErrorEnergy(double[] E) {
            if (E == null)
                throw new NullReferenceException();
            return E.Sum() / E.Length;
        }

        private double AverageMinAbsoluteDeviation(double[] absError) {
            if (absError == null)
                throw new NullReferenceException();
            return absError.Sum() / absError.Length;
        }
        
        

        private List<double[]> RandomizeData(List<double[]> list, List<double[]> expected) {
            // RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            // var n = list.Count;
            // while (n > 1) {
            //     byte[] box = new byte[1];
            //     do provider.GetBytes(box);
            //     while (!(box[0] < n * (Byte.MaxValue / n)));
            //     n--;
            //     var k = box[0] % n;
            //     (list[k], list[n]) = (list[n], list[k]);
            // }

            Random rnd = new Random();
            for (int i = list.Count - 1; i > 0; i--) {
                int k = rnd.Next(i + 1);
                (list[k], list[i]) = (list[i], list[k]);
                (expected[k], expected[i]) = (expected[i], expected[k]);
            }
            
            return list;
        }

        private List<double[]> CopyList(List<double[]> list) {
            List<double[]> newList = new List<double[]>();

            foreach (var d in list) {
                var temp = new double[d.Length];
                for (var i = 0; i < d.Length; i++)
                    temp[i] = d[i];
                
                newList.Add(temp);
            }
            
            return newList;
        }
    }
}
