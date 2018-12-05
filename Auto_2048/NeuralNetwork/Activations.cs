using System;

namespace FeedForwardNeuralNetwork
{
    public static class Activations
    {
        public static double BinaryStep(double x)
        {
            return x < 0 ? 0 : 1;
        }

        public static double Sigmoid(double x)
        {
            return 1 / (1 + Math.Exp(-x));
        }

        public static double RELU(double x)
        {
            return x < 0 ? 0 : x;
        }

        public static double LRELU(double x)
        {
            return x < 0 ? 0.01 * x : x;
        }
    }
}
