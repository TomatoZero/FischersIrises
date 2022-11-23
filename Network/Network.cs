using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                TrainingBinar();
            else
                TrainingMulti();
        }

        private void TrainingBinar() {
            var data = CopyList(_inputLayer.Data);
            var averageE = 0d;
            var errorsEnergy = new double[_inputLayer.Data.Count];
            var crossEntropy = new double[_inputLayer.Data.Count];
            var predicted = new double[_inputLayer.Data.Count][];
            var epohCount = 0;
            
            var errors = new double[_outputLayer.Neurons.Length];

            do {
                RandomizeData(data);
                for (var i = 0; i < _inputLayer.Data.Count; i++) {
                    _outputLayer.Data = data[i];
                    var output = _outputLayer.Recognize();
                    predicted[i] = output;
                    
                    if (output.Length != _inputLayer.ExpectedResult[i].Length)
                        throw new NotImplementedException();
                    
                    errors = new double[output.Length];
                    var expectedResult = _inputLayer.ExpectedResult[i];
                    for (var j = 0; j < errors.Length; j++) {
                        // errors[j] = - expectedResult[j] * Math.Log(predicted[i][j]) ;
                        errors[j] = expectedResult[j] - predicted[i][j];
                    }
                    errorsEnergy[i] = TotalErrorEnergy(errors);
                    // crossEntropy[i] = BinaryCrosEntropy(expectedResult[0], predicted[i][0]);
                    _outputLayer.ChangeWeights(errors, output, data[i]);

                    // Console.WriteLine();
                    
                    // if(epohCount % 1000 == 0)
                    
                }
                
                averageE = AverageErrorEnergy(errorsEnergy);
                // averageE = crossEntropy.Sum();
                // _outputLayer.ChangeWeights(averageE);
                Console.WriteLine($"Epoch {epohCount++} end. Average Energy {averageE}");

            } while (averageE > 0.1);
            _outputLayer.WeightsInitialize(MemoryMod.Set);
            Console.WriteLine($"End Learning. Epoch count {epohCount}. Average Energy {averageE}");

        }
        
        private void TrainingMulti() {
            var data = CopyList(_inputLayer.Data);
            var averageE = 0d;
            var errorsEnergy = new double[_inputLayer.Data.Count];
            var predicted = new double[_inputLayer.Data.Count][];
            var epohCount = 0;
            
            do {
                RandomizeData(data);
                for (var i = 0; i < _inputLayer.Data.Count - 1; i++) {
                    // Console.WriteLine($"     Start iteration {i}");

                    var tempFormHidden = data[i];
                    for (var j = 0; j < _hideLayers.Length; j++) {
                        _hideLayers[j].Data = tempFormHidden;
                        tempFormHidden = _hideLayers[j].Recognize();
                    }
                    
                    _outputLayer.Data = tempFormHidden;
                    var output = _outputLayer.Recognize();
                    predicted[i] = output;
                    
                    if (output.Length != _inputLayer.ExpectedResult[i].Length)
                        throw new NotImplementedException();
                    
                    var errors = new double[output.Length];
                    var expectedResult = _inputLayer.ExpectedResult[i];
                    for (var j = 0; j < errors.Length; j++) {
                        errors[j] = expectedResult[j] - output[j];
                    }
                    errorsEnergy[i] = TotalErrorEnergy(errors);
                    
                    var gradientSum = _outputLayer.BackwardPass(errors);
                    for (int j = _hideLayers.Length - 1; j >= 0; j--) {
                        gradientSum = _hideLayers[j].BackwardPass(gradientSum, true);
                    }
                    
                    // Console.WriteLine($"    Iteration {i} end. Error energy {errorsEnergy[i]}");
                }

                averageE = AverageErrorEnergy(errorsEnergy);
                
                if(epohCount % 1000 == 0)
                    Console.WriteLine($"Epoch {epohCount} end. Average Energy {averageE}");

                epohCount++;
            } while (averageE > 0.01);
            _outputLayer.WeightsInitialize(MemoryMod.Set);
            
            for(var i = 0; i < _hideLayers.Length; i ++) {
                _hideLayers[i].WeightsInitialize(MemoryMod.Set);
            }
            
            Console.WriteLine($"End Learning. Epoch count {epohCount} ");
        }

        public void Test(string dataPath) {
            _inputLayer = new InputLayer(dataPath, true);
            _outputLayer = new OutputLayer(_outputLayer.Neurons.Length, _inputLayer.Data[1].Length, NeuronType.Output,
                _outputLayer.WeightFilePath);
            
            if(_single)
                TestSingle();
            else {
                for (var i = 0; i < _hideLayers.Length; i++) {
                    _hideLayers[i] = new HideLayer(_hideLayers[i].Neurons.Length, i == 0 ? _inputLayer.Data[1].Length : _hideLayers[i - 1].Neurons.Length,
                        NeuronType.Hiden, _hideLayers[i].WeightFilePath);
                }
                
                TestMulty();
            }
        }

        private void TestSingle() {
            
            for (var i = 0; i < _inputLayer.Data.Count - 1; i++) {
                _outputLayer.Data = _inputLayer.Data[i];
                var output = _outputLayer.Recognize();
                   
                var errors = new double[output.Length];

                for (var j = 0; j < errors.Length; j++) {
                    errors[j] = output[j] - _inputLayer.ExpectedResult[i][j];
                }
                
                Console.WriteLine($"Output {string.Join(' ', output)}, Error {string.Join(' ', errors)}");
            }
        }
        
        private void TestMulty() {
            
            for (var i = 0; i < _inputLayer.Data.Count - 1; i++) {
                var tempFormHidden = _inputLayer.Data[i];
                for (var j = 0; j < _hideLayers.Length; j++) {
                    _hideLayers[j].Data = tempFormHidden;
                    tempFormHidden = _hideLayers[j].Recognize();
                }
                    
                _outputLayer.Data = tempFormHidden;
                var output = _outputLayer.Recognize();

                var errors = new double[output.Length];
                var expectedResult = _inputLayer.ExpectedResult[i];
                for (var j = 0; j < errors.Length; j++) {
                    errors[j] = expectedResult[j] - output[j];
                }

                Console.WriteLine($"Output {string.Join(' ', output)}, Error {string.Join(' ', errors)}");
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

        private double AverageErrorEnergy(double[] E) {
            if (E == null)
                throw new NullReferenceException();
            return E.Sum() / E.Length;
        }

        private void RandomizeData(List<double[]> list) {
            int c = 0;
            Random random = new Random();
            for (int i = list.Count - 1; i >= 1; i--) {
                int j = random.Next(i + 1);

                try {
                    (list[j], list[i]) = (list[i], list[j]);
                }
                catch {
                    
                }
            }
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
