using FeedForwardNeuralNetwork;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Trainer
{
    class Program
    {
        static Random random = new Random();

        static void Main(string[] args)
        {
            Console.Clear();
            Console.Write("Play or Train [p/t]? ");
            //string response = Console.ReadLine().ToLower();

            if (false)// (response == "p")
            {
                PlayGame();
            }
            else if (true)//(response == "t")
            {
                TrainNetwork();
            }
        }

        static void PlayGame()
        {
            Console.Clear();

            Gamer player = new Gamer(random);
            while (!player.GameOver)
            {
                player.Play(true);
            }
            Console.WriteLine("Goodbye!");
        }

        static void TrainNetwork()
        {
            Console.Clear();

            // To Beat: 
            // Total: 564,116
            // Highest: 32,768

            // 20, x, 4
            // 10 = 
            // 14 = 4628
            // 20 = 5504
            // 30 = 4096

            int maxGen = 1000;
            int playCount = 16;
            int populationSize = 1000;
            int highAverageScore = 0;
            Gamer[] population = new Gamer[populationSize];

            int inputSize = 20;
            int[] netShape = { 16, 4 };

            //Initialize Gamers
            for (int i = 0; i < population.Length; i++)
            {
                population[i] = new Gamer(random, inputSize, netShape);
            }

            for (int gen = 0; gen < maxGen; gen++)
            {

                if (gen != 0)
                {
                    //Mutate Generation
                    int start = (int)(population.Length * 0.10);
                    int end = (int)(population.Length * 0.85);
                    for (int i = start; i < end; i++)
                    {
                        population[i].Net.Crossover(random, population[random.Next(start)].Net);
                        population[i].Net.Mutate(random, 0.15);
                    }
                    for (int i = end; i < population.Length; i++)
                    {
                        population[i].Net.Randomize(random);
                    }
                }

                for (int i = 0; i < population.Length; i++)
                {
                    population[i].AverageScore = 0;
                }

                //each net should play a # of games and get an average score

                Parallel.For(0, population.Length, (i) =>
                {
                    for (int gameNum = 0; gameNum < playCount; gameNum++)
                    {
                        //Reset Game
                        population[i].Restart();

                        //Each Net Plays game until completed
                        while (!population[i].GameOver)
                        {
                            population[i].Play();
                        }

                        //calculate avg score after each game
                        population[i].AverageScore += population[i].Score;
                    }
                    population[i].AverageScore /= playCount;
                });


                //Sort Fitness
                Array.Sort(population, (a, b) => b.AverageScore.CompareTo(a.AverageScore));

                if (population[0].AverageScore > highAverageScore)
                {
                    highAverageScore = population[0].AverageScore;
                }

                //Display Progress

                Console.SetCursorPosition(0, 0);
                Console.WriteLine($"Highest Average Score: {highAverageScore}");
                Console.WriteLine($"%{(int)(gen / (double)maxGen * 100)}");


            }

            //save best net to json
            string save = JsonConvert.SerializeObject(population[0].Net);
            File.WriteAllText("bestNet.json", save);

            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }
}
