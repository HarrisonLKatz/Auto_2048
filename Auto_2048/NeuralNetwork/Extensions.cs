using System;

namespace FeedForwardNeuralNetwork
{
    public static class Extensions
    {
        public static double NextDouble(this Random sender, double min, double max)
        {
            return sender.NextDouble() * (max - min) + min;
        }

        public static int RandomSign(this Random sender)
        {
            return sender.Next(0, 2) * 2 - 1;
        }
    }
}
