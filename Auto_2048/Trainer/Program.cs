using FeedForwardNeuralNetwork;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TrainerGpu
{
    class Program
    {
        // To Beat: 
        // Total: 564,116
        // Highest: 32,768

        static void Main(string[] args)
        {
            Console.Clear();

            Console.Write("Play or Train [p/t]? ");
            //string response = Console.ReadLine().ToLower();

            if (false) //(response == "p")
            {
                PlayGame();
            }
            else //if (response == "t")
            {
                StringBuilder csv = new StringBuilder();
                for (int i = 1; i <= 1; i++)
                {
                    Console.Clear();
                    Console.WriteLine($"% {((i - 1) / (float)17) * 100}");
                    Console.WriteLine($"Current Value: {i}");
                    int score = TrainNetwork(i);
                    csv.AppendLine($"{i}, {score}");
                }
                Console.Clear();
                Console.WriteLine("% 100");
                File.WriteAllText($"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}/results.csv", csv.ToString());
            }
        }

        static void PlayGame()
        {
            Random playerRand = new Random();

            Console.Clear();
            Gamer player = new Gamer();
            player.Restart(playerRand);
            while (!player.GameOver)
            {
                player.Play(playerRand, true);
            }
            Console.WriteLine("Goodbye!");
        }

        static int TrainNetwork(int hiddenSize)
        {
            int maxGen = 1;
            int playCount = 16;
            int populationSize = 1;
            int currBestAvg = 0;
            int highAverageScore = 0;
            Gamer[] population = new Gamer[populationSize];

            int inputSize = 20;
            int[] netShape = { hiddenSize, 4 };
            ActivationType[] acts = Enumerable.Repeat(ActivationType.RELU, netShape.Length).ToArray();
            acts[acts.Length - 1] = ActivationType.Sigmoid;

            Random trainRand = new Random();

            //Initialize Gamers
            for (int i = 0; i < population.Length; i++)
            {
                population[i] = new Gamer(trainRand, acts, inputSize, netShape);
            }

            for (int gen = 0; gen < maxGen; gen++)
            {

                if (gen != 0)
                {
                    //Mutate Generation
                    int start = (int)(population.Length * 0.10);
                    int end = (int)(population.Length * 0.90);
                    for (int i = start; i < end; i++)
                    {
                        population[i].Net.Crossover(trainRand, population[trainRand.Next(start)].Net);
                        population[i].Net.Mutate(trainRand, 0.15);
                    }
                    for (int i = end; i < population.Length; i++)
                    {
                        population[i].Net.Randomize(trainRand);
                    }
                }

                currBestAvg = 0;
                for (int i = 0; i < population.Length; i++)
                {
                    population[i].AverageScore = 0;
                }

                //thread local data
                int seed = Guid.NewGuid().GetHashCode();



                Parallel.For(0, population.Length, (i) =>
                {
                    Random gameRand = new Random(seed);

                    //each net should play a # of games and get an average score
                    for (int gameNum = 0; gameNum < playCount; gameNum++)
                    {
                        //Reset Game
                        population[i].Restart(gameRand);

                        //Each Net Plays game until completed
                        while (!population[i].GameOver)
                        {
                            population[i].Play(gameRand);
                        }

                        //calculate avg score after each game
                        population[i].AverageScore += population[i].Score;
                    }

                    //average score of all games
                    population[i].AverageScore /= playCount;
                });


                //Sort Fitness
                Array.Sort(population, (a, b) => b.AverageScore.CompareTo(a.AverageScore));

                currBestAvg = population[0].AverageScore;
                if (population[0].AverageScore > highAverageScore)
                {
                    highAverageScore = population[0].AverageScore;
                }

                //Display Progress
                //Console.SetCursorPosition(0, 0);
                //Console.WriteLine($"Gen: {gen}");
                //Console.WriteLine($"Gen Top Avg Score: {currBestAvg}");
                //Console.WriteLine($"All Top Avg Score: {highAverageScore}");
                //Console.WriteLine($"% {(int)(gen / (double)maxGen * 100)}");
            }

            //save best net to json
            string save = JsonConvert.SerializeObject(population[0].Net);
            File.WriteAllText($"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}/network.json", save);
            return highAverageScore;
        }
    }
}
