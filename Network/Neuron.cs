﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    enum NeuronType { Hiden, Output}
    enum MemoryMod { Get, Set}
    
    internal class Neuron
    {
        private NeuronType _type;
        private double[] _weights;
        private double[] _inputs;

        public double[] Weights { get => _weights; set => _weights = value; }
        public double[] Inputs { get => _inputs; set => _inputs = value; }
        public double Output { get => Activator(_inputs, _weights); }

        public Neuron(double[] inputs, double[] weights, NeuronType type) {
            _type = type;
            _weights = weights;
            _inputs = inputs;
        }

        private double Activator(double[] input, double[] weights) {
            double sum = 0;
            for(int i = 0; i < input.Length; i++)
                sum += input[i] * weights[i];

            return Math.Pow(1 + Math.Exp(-sum), -1);    //сигмоїдальна функція активації 
        }


        public double Derevativator() => Derevativator(Output); 
        public double Derevativator(double outSignal) => outSignal * (1 - outSignal);
        public double Gradientor(double error, double diff, double gradientSum) => (_type == NeuronType.Output) ? error * diff : gradientSum * diff;
    }
}
