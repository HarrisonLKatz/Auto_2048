using Newtonsoft.Json;
using System;
using System.Linq;

namespace FeedForwardNeuralNetwork
{
    public class Network
    {
        public Layer[] Layers;
        [JsonProperty]
        private readonly int inputCount;
        [JsonProperty]
        private readonly int[] layerNeurons;
        [JsonProperty]
        private readonly ActivationType[] acts;

        [JsonIgnore]
        public double[] Output;

        [JsonConstructor]
        public Network(ActivationType[] acts, int inputCount, params int[] layerNeurons)
        {
            if (acts.Length != layerNeurons.Length)
            {
                throw new ArgumentException("acts length must match layerNeurons length");
            }

            this.acts = acts;
            this.inputCount = inputCount;
            this.layerNeurons = layerNeurons;

            Layers = new Layer[layerNeurons.Length];
            Layers[0] = new Layer(acts[0], inputCount, layerNeurons[0]);
            for (int i = 1; i < layerNeurons.Length; i++)
            {
                Layers[i] = new Layer(acts[i], Layers[i - 1].Neurons.Length, layerNeurons[i]);
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
            Output = input;
            for (int i = 0; i < Layers.Length; i++)
            {
                Output = Layers[i].Compute(Output);
            }
            return Output;
        }

        public void Mutate(Random random, double rate)
        {
            for (int l = 0; l < Layers.Length; l++)
            {
                for (int n = 0; n < Layers[l].Neurons.Length; n++)
                {
                    if (random.NextDouble() < rate)
                    {
                        Layers[l].Neurons[n].Bias *= random.NextDouble(0.5, 1.5);
                        if (random.NextDouble() < rate)
                        {
                            Layers[l].Neurons[n].Bias *= random.RandomSign();
                        }
                    }

                    for (int w = 0; w < Layers[l].Neurons[n].Weights.Length; w++)
                    {
                        if (random.NextDouble() < rate)
                        {
                            Layers[l].Neurons[n].Weights[w] *= random.NextDouble(0.5, 1.5);

                            if (random.NextDouble() < rate)
                            {
                                Layers[l].Neurons[n].Weights[w] *= random.RandomSign();
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
                int cut = random.Next(Layers[l].Neurons.Length);
                bool flip = random.Next(2) == 0;

                for (int n = (flip ? 0 : cut); n < (flip ? cut : Layers[l].Neurons.Length); n++)
                {
                    Layers[l].Neurons[n].Bias = other.Layers[l].Neurons[n].Bias;
                    other.Layers[l].Neurons[n].Weights.CopyTo(Layers[l].Neurons[n].Weights, 0);
                }
            }
        }
    }
}
