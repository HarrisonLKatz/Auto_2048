using FeedForwardNeuralNetwork;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

namespace Trainer
{
    class Program
    {
        static void Main(string[] args)
        {
            // 16, x, 4
            // 10 = 2877
            // 13 = 4872
            // 14 = 7102
            // 15 = 6142
            // 17 = 3378
            // 20 = 2667

            // 1 extra bit for if a move occured last turn
            // 17, x, 4


            int maxGen = 1000;
            int playCount = 10;
            int populationSize = 100;

            int highScore = 0;
            Random random = new Random();
            Gamer[] population = new Gamer[populationSize];
            for (int i = 0; i < population.Length; i++)
            {
                population[i] = new Gamer(random);
            }

            for (int gen = 0; gen < maxGen; gen++)
            {
                //Mutate Generation
                if (gen != 0)
                {
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
                for (int gameNum = 0; gameNum < playCount; gameNum++)
                {
                    //Reset Game
                    for (int i = 0; i < population.Length; i++)
                    {
                        population[i].Initialize();
                    }

                    //Play Game until all nets finish
                    int doneCount;
                    do
                    {
                        doneCount = 0;
                        for (int i = 0; i < population.Length; i++)
                        {
                            if (!population[i].GameOver)
                            {
                                population[i].Play();
                            }
                            else
                            {
                                doneCount++;
                            }
                        }
                    } while (doneCount != population.Length);

                    //calculate avg score after each game
                    for (int i = 0; i < population.Length; i++)
                    {
                        population[i].AverageScore += population[i].Score;
                    }
                }

                for (int i = 0; i < population.Length; i++)
                {
                    population[i].AverageScore /= playCount;
                }

                //Sort Fitness
                Array.Sort(population, (a, b) => b.AverageScore.CompareTo(a.AverageScore));

                if (population[0].AverageScore > highScore)
                {
                    highScore = population[0].AverageScore;
                }

                //Display Progress
                Console.SetCursorPosition(0, 0);
                Console.WriteLine($"%{(int)(gen / (double)maxGen * 100)}");
                Console.WriteLine($"High Score: {highScore}");

            }


            //play game with best net
            string save = JsonConvert.SerializeObject(population[0].Net);

            File.WriteAllText("bestNet.json", save);

            //save best net to json
            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }
}
