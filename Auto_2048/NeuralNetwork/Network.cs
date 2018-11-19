using Newtonsoft.Json;
using System;
using System.Linq;

namespace FeedForwardNeuralNetwork
{
    public class Network
    {
        public Layer[] Layers;

        [JsonProperty]
        private readonly ActivationType act;
        [JsonProperty]
        private readonly int inputCount;
        [JsonProperty]
        private readonly int[] layerNeurons;

        [JsonIgnore]
        public double[] Output => Layers.Last().Output;

        [JsonConstructor]
        public Network(ActivationType act, int inputCount, params int[] layerNeurons)
        {
            this.act = act;
            this.inputCount = inputCount;
            this.layerNeurons = layerNeurons;

            Layers = new Layer[layerNeurons.Length];
            Layers[0] = new Layer(act, inputCount, layerNeurons[0]);
            for (int i = 1; i < layerNeurons.Length; i++)
            {
                Layers[i] = new Layer(act, Layers[i - 1].Neurons.Length, layerNeurons[i]);
            }
        }

        public void Randomize(Random rand)
        {
            for (int i = 0; i < Layers.Length; i++)
            {
                Layers[i].Randomize(rand);
            }
        }

        public double[] Compute(double[] input)
        {

            double[] output = input;
            for (int i = 0; i < Layers.Length; i++)
            {
                output = Layers[i].Compute(output);
            }
            return output;
        }

        public void Mutate(Random random, double rate)
        {
            for (int l = 0; l < Layers.Length; l++)
            {
                Layer layer = Layers[l];
                for (int n = 0; n < layer.Neurons.Length; n++)
                {
                    Neuron neuron = layer.Neurons[n];
                    if (random.NextDouble() < rate)
                    {
                        neuron.Bias *= random.NextDouble(0.5, 1.5);
                        if (random.NextDouble() < rate)
                        {
                            neuron.Bias *= random.RandomSign();
                        }
                    }

                    for (int w = 0; w < neuron.Weights.Length; w++)
                    {
                        if (random.NextDouble() < rate)
                        {
                            neuron.Weights[w] *= random.NextDouble(0.5, 1.5);

                            if (random.NextDouble() < rate)
                            {
                                neuron.Weights[w] *= random.RandomSign();
                            }
                        }
                    }
                }
            }
        }

        public void Crossover(Random random, Network other)
        {
            for (int l = 0; l < Layers.Length; l++)
            {
                Layer layer = Layers[l];

                int cut = random.Next(layer.Neurons.Length);
                bool flip = random.Next(2) == 0;

                for (int n = (flip ? 0 : cut); n < (flip ? cut : layer.Neurons.Length); n++)
                {
                    Neuron neuron = layer.Neurons[n];
                    Neuron otherNeuron = other.Layers[l].Neurons[n];

                    neuron.Bias = otherNeuron.Bias;
                    otherNeuron.Weights.CopyTo(neuron.Weights, 0);
                }
            }
        }
    }
}
