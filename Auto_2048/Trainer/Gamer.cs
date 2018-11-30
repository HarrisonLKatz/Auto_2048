using FeedForwardNeuralNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trainer
{
    class Gamer
    {
        public int Score;
        public int AverageScore;
        public Network Net;

        public Board game;

        private Random random;
        private bool moveOccured;

        public bool GameOver => game.GameOver;

        public Gamer(Random random)
        {
            Net = new Network(ActivationType.Sigmoid, 17, 14, 4);
            Net.Randomize(random);
            this.random = random;
            Initialize();
        }

        public void Initialize()
        {
            Score = 0;
            moveOccured = false;
            game = new Board(random);
        }

        public void Play(bool manual = false)
        {

            bool moveCanOccur = false;
            moveCanOccur |= game.Up(true);
            moveCanOccur |= game.Down(true);
            moveCanOccur |= game.Left(true);
            moveCanOccur |= game.Right(true);

            if (!moveCanOccur)
            {
                game.GameOver = true;
                return;
            }

            int pick = -1;
            if (manual)
            {
                Console.SetCursorPosition(0, 0);
                for (int i = 0; i < game.Values.GetLength(0); i++)
                {
                    for (int j = 0; j < game.Values.GetLength(1); j++)
                    {
                        Console.Write($"{game.Values[j, i]}\t");
                    }
                    Console.WriteLine("");
                }

                ConsoleKey key = Console.ReadKey(true).Key;
                Console.Write("                         "); //clearing error
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        pick = 0;
                        break;
                    case ConsoleKey.DownArrow:
                        pick = 1;
                        break;
                    case ConsoleKey.LeftArrow:
                        pick = 2;
                        break;
                    case ConsoleKey.RightArrow:
                        pick = 3;
                        break;
                    default:
                        Console.SetCursorPosition(0, Console.CursorTop);
                        Console.WriteLine("Not a valid move");
                        break;
                }
            }
            else
            {
                //Pick Move with network
                double[] input = new double[17];

                input[0] = moveOccured ? 0 : 1;

                int index = 1;
                foreach (int num in game.Values)
                {
                    input[index] = num;
                    index++;
                }

                double[] outputs = Net.Compute(input);
                pick = Array.IndexOf(outputs, outputs.Max());
            }

            //perform move
            moveOccured = false;
            switch (pick)
            {
                case 0:
                    moveOccured |= game.Up(false);
                    break;
                case 1:
                    moveOccured |= game.Down(false);
                    break;
                case 2:
                    moveOccured |= game.Left(false);
                    break;
                case 3:
                    moveOccured |= game.Right(false);
                    break;
            }

            if (moveOccured)
            {
                game.AddRandomTile();
            }
        }


    }
}
