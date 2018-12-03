using FeedForwardNeuralNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trainer
{
    class Gamer
    {
        public int AverageScore;
        public Network Net;
        public bool GameOver => Game.GameOver;
        public int Score => Game.Score;

        public Board Game;

        //Game Variables
        private bool moveOccured;
        private double[] input;
        private bool moveCanOccur;
        private bool canMoveUp;
        private bool canMoveDown;
        private bool canMoveLeft;
        private bool canMoveRight;
        private int pick;
        private double[] outputs;

        public Gamer() // constructor for manual, don't judge
        {
            //Start Game
            Game = new Board();
            moveOccured = false;
        }

        public Gamer(Random random, ActivationType[] acts, int inputSize, int[] netShape) // constructor for trainer
        {
            //Start Net
            Net = new Network(acts, inputSize, netShape);

            Net.Randomize(random);
            input = new double[inputSize];

            //Start Game
            Game = new Board();
            moveOccured = false;
        }

        public void Restart(Random random)
        {
            Game.Init(random);
            moveOccured = false;
        }

        public void Play(Random random, bool manual = false)
        {
            moveCanOccur = false;
            canMoveUp = Game.Up(true);
            canMoveDown = Game.Down(true);
            canMoveLeft = Game.Left(true);
            canMoveRight = Game.Right(true);

            moveCanOccur |= canMoveUp;
            moveCanOccur |= canMoveDown;
            moveCanOccur |= canMoveLeft;
            moveCanOccur |= canMoveRight;

            if (!moveCanOccur)
            {
                Game.GameOver = true;
                return;
            }

            pick = -1;
            if (manual)
            {
                Console.SetCursorPosition(0, 0);
                for (int i = 0; i < Game.Values.GetLength(0); i++)
                {
                    for (int j = 0; j < Game.Values.GetLength(1); j++)
                    {
                        Console.Write($"{Game.Values[j, i]}\t");
                    }
                    Console.WriteLine("");
                }

                ConsoleKey key = Console.ReadKey(true).Key;
                Console.Write("                         "); //clearing error message
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
                input[0] = canMoveUp ? 1 : 0;
                input[1] = canMoveDown ? 1 : 0;
                input[2] = canMoveLeft ? 1 : 0;
                input[3] = canMoveRight ? 1 : 0;

                int index = 4;
                foreach (int num in Game.Values)
                {
                    input[index] = num;
                    index++;
                }

                outputs = Net.Compute(input);
                pick = Array.IndexOf(outputs, outputs.Max());
            }

            //perform move
            moveOccured = false;
            switch (pick)
            {
                case 0:
                    moveOccured |= Game.Up(false);
                    break;
                case 1:
                    moveOccured |= Game.Down(false);
                    break;
                case 2:
                    moveOccured |= Game.Left(false);
                    break;
                case 3:
                    moveOccured |= Game.Right(false);
                    break;
            }

            if (moveOccured)
            {
                Game.AddRandomTile(random);
            }
            else
            {
                Game.GameOver = true;
            }
        }


    }
}
