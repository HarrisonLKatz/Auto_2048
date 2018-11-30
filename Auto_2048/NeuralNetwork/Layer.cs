using Newtonsoft.Json;
using System;
using System.Linq;

namespace FeedForwardNeuralNetwork
{
    public class Layer
    {
        public Neuron[] Neurons;

        [JsonIgnore]
        public double[] Output;

        public Layer(ActivationType act, int inputCount, int neuronCount)
        {
            Neurons = new Neuron[neuronCount];
            for (int i = 0; i < Neurons.Length; i++)
            {
                Neurons[i] = new Neuron(act, inputCount);
            }

            Output = new double[neuronCount];
        }

        public void Randomize(Random rand)
        {
            for (int i = 0; i < Neurons.Length; i++)
            {
                Neurons[i].Randomize(rand);
            }
        }

        public double[] Compute(double[] input)
        {
            for (int i = 0; i < Neurons.Length; i++)
            {
                Output[i] = Neurons[i].Compute(input);
            }
            return Output;
        }
    }
}
