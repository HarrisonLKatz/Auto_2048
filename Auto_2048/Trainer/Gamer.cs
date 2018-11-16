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
        public Network Net;
        public bool GameOver;
        public Random random;

        int[,] board = new int[4, 4];

        public Gamer(Random random)
        {
            Score = 0;
            GameOver = false;
            Net = new Network(Activations.Sigmoid, 16, 20, 4);
            Net.Randomize(random);
            this.random = random;
            AddRandomTile();
            AddRandomTile();
        }

        public void Play(bool draw)
        {

            bool moveOccured = false;
            moveOccured |= Up(true);
            //moveOccured |= Left(true);
            moveOccured |= Down(true);
            //moveOccured |= Right(true);

            if (!moveOccured)
            {
                GameOver = true;
                return;
            }

            if (draw)
            {
                Console.SetCursorPosition(0, 0);
                for (int i = 0; i < board.GetLength(0); i++)
                {
                    for (int j = 0; j < board.GetLength(1); j++)
                    {
                        Console.Write($"{board[j, i]}\t");
                    }
                    Console.WriteLine("");
                }
            }


            //Pick Move
            double[] outputs = new double[4];

            ConsoleKey key = Console.ReadKey().Key;

            int pick = -1; //Array.IndexOf(outputs, outputs.Max());
            switch (key)
            {
                case ConsoleKey.UpArrow:
                    pick = 0;
                    break;
                case ConsoleKey.DownArrow:
                    pick = 2;
                    break;
                case ConsoleKey.LeftArrow:
                    pick = 1;
                    break;
                case ConsoleKey.RightArrow:
                    pick = 3;
                    break;
            }

            //perform move
            moveOccured = false;
            switch (pick)
            {
                case 0:
                    moveOccured |= Up(false);
                    break;
                case 1:
                    moveOccured |= Left(false);
                    break;
                case 2:
                    moveOccured |= Down(false);
                    break;
                case 3:
                    moveOccured |= Right(false);
                    break;
                default:
                    throw new Exception();
            }

            if (moveOccured)
            {
                AddRandomTile();
            }

            if (draw)
            {
                Console.SetCursorPosition(0, 0);
                for (int i = 0; i < board.GetLength(0); i++)
                {
                    for (int j = 0; j < board.GetLength(1); j++)
                    {
                        Console.Write($"{board[j, i]}\t");
                    }
                    Console.WriteLine("");
                }
            }
        }

        void AddRandomTile()
        {
            //find all open spots
            List<(int, int)> openSpots = new List<(int, int)>(16);
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (board[i, j] == 0)
                    {
                        openSpots.Add((i, j));
                    }
                }
            }

            //If no open spots, game is over
            if (openSpots.Count == 0)
            {
                return;
            }

            //pick a random open spot and fill with either 2 or 4
            (int x, int y) = openSpots[random.Next(openSpots.Count)];
            board[x, y] = random.NextDouble() < 0.9 ? 2 : 4;
        }

        bool Up(bool reset) // 0 top, length bot
        {
            int[,] copy = board.Clone() as int[,];


            bool moveOccured = false;
            //for every column
            for (int col = 0; col < board.GetLength(0); col++)
            {
                HashSet<int> merged = new HashSet<int>();
                for (int row = 0; row < board.GetLength(1); row++)
                {
                    if (board[col, row] != 0)
                    {
                        //move tiles up
                        //once tile is merged, it cannot be merged again
                        //add value of merged tile to score once merged
                        int curr = row;
                        while (curr > 0)
                        {
                            //attempt to move tile up if the spot above it is open
                            //if the spot above it contains a tile of the same value and it is not merged, merge the tiles
                            //if it cannot move up, break the loop

                            if (merged.Contains(curr))
                            {
                                break;
                            }


                            //if the above spot if open
                            if (board[col, curr - 1] == 0)
                            {
                                //move up by swapping values
                                board[col, curr - 1] = board[col, curr];
                                board[col, curr] = 0;
                                curr--;
                                moveOccured = true;
                            }
                            //if the above spot is the same value and not merged
                            else if (board[col, curr - 1] == board[col, curr] && !merged.Contains(curr - 1))
                            {
                                //move up by merging values
                                board[col, curr - 1] += board[col, curr];
                                board[col, curr] = 0;

                                Score += board[col, curr - 1];

                                //add score
                                merged.Add(curr - 1);
                                curr--;
                                moveOccured = true;
                            }
                            //can no longer move
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }

            if (reset)
            {
                board = copy.Clone() as int[,];
            }

            return moveOccured;
        }

        bool Down(bool reset)
        {
            int[,] copy = board.Clone() as int[,];


            bool moveOccured = false;
            //for every column
            for (int col = 0; col < board.GetLength(0); col++)
            {
                HashSet<int> merged = new HashSet<int>();
                for (int row = board.GetLength(1) - 1; row >= 0; row--)
                {
                    if (board[col, row] != 0)
                    {
                        //move tiles down
                        //once tile is merged, it cannot be merged again
                        //add value of merged tile to score once merged
                        int curr = row;
                        while (curr < board.GetLength(1) - 1)
                        {
                            //attempt to move tile down if the spot below it is open
                            //if the spot below it contains a tile of the same value and it is not merged, merge the tiles
                            //if it cannot move down, break the loop

                            if (merged.Contains(curr))
                            {
                                break;
                            }


                            //if the below spot if open
                            if (board[col, curr + 1] == 0)
                            {
                                //move up by swapping values
                                board[col, curr + 1] = board[col, curr];
                                board[col, curr] = 0;
                                curr++;
                                moveOccured = true;
                            }
                            //if the below spot is the same value and not merged
                            else if (board[col, curr + 1] == board[col, curr] && !merged.Contains(curr + 1))
                            {
                                //move down by merging values
                                board[col, curr + 1] += board[col, curr];
                                board[col, curr] = 0;

                                Score += board[col, curr + 1];

                                //add score
                                merged.Add(curr + 1);
                                curr++;
                                moveOccured = true;
                            }
                            //can no longer move
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }

            if (reset)
            {
                board = copy.Clone() as int[,];
            }

            return moveOccured;
        }

        bool Left(bool reset) //left 0, right length
        {
            throw new NotImplementedException();
        }

        bool Right(bool reset)
        {
            throw new NotImplementedException();
        }
    }
}
