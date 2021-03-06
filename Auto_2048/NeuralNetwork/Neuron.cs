﻿using Newtonsoft.Json;
using System;

namespace FeedForwardNeuralNetwork
{
    public class Neuron
    {
        public double Bias;
        public double[] Weights;
        [JsonProperty]
        private readonly ActivationType act;


        [JsonIgnore]
        public double Output;
        [JsonIgnore]
        public Func<double, double> Activation;

        public Neuron(ActivationType act, int inputCount)
        {
            this.act = act;
            switch (act)
            {
                case ActivationType.BinaryStep:
                    Activation = Activations.BinaryStep;
                    break;
                case ActivationType.Sigmoid:
                    Activation = Activations.Sigmoid;
                    break;
                case ActivationType.RELU:
                    Activation = Activations.RELU;
                    break;
                case ActivationType.LRELU:
                    Activation = Activations.LRELU;
                    break;
                default:
                    throw new Exception("u wot m8");
            }
            Weights = new double[inputCount];
        }

        public void Randomize(Random rand)
        {
            Bias = rand.NextDouble(-0.5, 0.5);
            for (int i = 0; i < Weights.Length; i++)
            {
                Weights[i] = rand.NextDouble(-0.5, 0.5);
            }
        }

        public double Compute(double[] input)
        {
            Output = 0;
            for (int i = 0; i < Weights.Length; i++)
            {
                Output += Weights[i] * input[i];
            }
            Output = Activation(Output + Bias);
            return Output;
        }
    }
}
