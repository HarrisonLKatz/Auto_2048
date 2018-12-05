using Newtonsoft.Json;
using System;

namespace FeedForwardNeuralNetwork
{
    public class Layer
    {
        public Neuron[] Neurons;
        [JsonProperty]
        private readonly ActivationType act;

        [JsonIgnore]
        public double[] Output;
        [JsonIgnore]
        public ActivationType ActivationType => act;

        public Layer(ActivationType act, int inputCount, int neuronCount)
        {
            this.act = act;
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

        public void SetActivation(ActivationType act)
        {
            for (int i = 0; i < Neurons.Length; i++)
            {
                switch (act)
                {
                    case ActivationType.BinaryStep:
                        Neurons[i].Activation = Activations.BinaryStep;
                        break;
                    case ActivationType.Sigmoid:
                        Neurons[i].Activation = Activations.Sigmoid;
                        break;
                    case ActivationType.RELU:
                        Neurons[i].Activation = Activations.RELU;
                        break;
                    default:
                        throw new Exception("u wot m8");
                }
            }
        }
    }
}
