using System;

namespace Trainer
{
    class Program
    {
        static void Main(string[] args)
        {
            Random random = new Random();
            //train population
            //save best to json
            Gamer[] population = new Gamer[100];
            for (int i = 0; i < population.Length; i++)
            {
                population[i] = new Gamer(random);
            }

            int maxGen = 1000;

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

                //Run Fitness
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

                //Sort Fitness
                Array.Sort(population, (a, b) => b.Score.CompareTo(a.Score));

                //Display Progress
                Console.SetCursorPosition(0, 0);
                Console.WriteLine($"%{(int)(gen / (double)maxGen * 100)}");
                Console.WriteLine($"High Score: {population[0].Score}");

            }

            //save best net to json

            Console.ReadKey();
        }
    }
}
