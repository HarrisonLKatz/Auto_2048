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
        public bool GameOver;
        public int[,] Board;

        private Random random;

        public Gamer(Random random)
        {
            Net = new Network(ActivationType.Sigmoid, 16, 14, 4);
            Net.Randomize(random);
            this.random = random;
            Initialize();
        }

        public void Initialize()
        {
            Score = 0;
            GameOver = false;
            Board = new int[4, 4];
            AddRandomTile();
            AddRandomTile();
        }

        public void Play(bool manual = false)
        {

            bool moveOccured = false;
            moveOccured |= Up(true);
            moveOccured |= Down(true);
            moveOccured |= Left(true);
            moveOccured |= Right(true);

            if (!moveOccured)
            {
                GameOver = true;
                return;
            }

            int pick = -1;
            if (manual)
            {
                Console.SetCursorPosition(0, 0);
                for (int i = 0; i < Board.GetLength(0); i++)
                {
                    for (int j = 0; j < Board.GetLength(1); j++)
                    {
                        Console.Write($"{Board[j, i]}\t");
                    }
                    Console.WriteLine("");
                }

                ConsoleKey key = Console.ReadKey().Key;
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
            }
            else
            {
                //Pick Move with network
                double[] input = new double[16];
                int index = 0;
                foreach (int num in Board)
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
                    moveOccured |= Up(false);
                    break;
                case 1:
                    moveOccured |= Down(false);
                    break;
                case 2:
                    moveOccured |= Left(false);
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
            else
            {
                GameOver = true; //if a direction is picked that does not result in a move, game is over (net will pick the same output constantly if input is the same)
            }
        }

        void AddRandomTile()
        {
            //find all open spots
            List<(int, int)> openSpots = new List<(int, int)>(16);
            for (int i = 0; i < Board.GetLength(0); i++)
            {
                for (int j = 0; j < Board.GetLength(1); j++)
                {
                    if (Board[i, j] == 0)
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
            Board[x, y] = random.NextDouble() < 0.9 ? 2 : 4;
        }

        bool Up(bool reset) // 0 top, length bot
        {
            int[,] copy = Board.Clone() as int[,];


            bool moveOccured = false;
            //for every column
            for (int col = 0; col < Board.GetLength(0); col++)
            {
                HashSet<int> merged = new HashSet<int>();
                for (int row = 0; row < Board.GetLength(1); row++)
                {
                    if (Board[col, row] != 0)
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
                            if (Board[col, curr - 1] == 0)
                            {
                                //move up by swapping values
                                Board[col, curr - 1] = Board[col, curr];
                                Board[col, curr] = 0;
                                curr--;
                                moveOccured = true;
                            }
                            //if the above spot is the same value and not merged
                            else if (Board[col, curr - 1] == Board[col, curr] && !merged.Contains(curr - 1))
                            {
                                //move up by merging values
                                Board[col, curr - 1] += Board[col, curr];
                                Board[col, curr] = 0;

                                Score += Board[col, curr - 1];

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
                Board = copy.Clone() as int[,];
            }

            return moveOccured;
        }

        bool Down(bool reset)
        {
            int[,] copy = Board.Clone() as int[,];
            bool moveOccured = false;
            for (int col = 0; col < Board.GetLength(0); col++)
            {
                HashSet<int> merged = new HashSet<int>();
                for (int row = Board.GetLength(1) - 1; row >= 0; row--)
                {
                    if (Board[col, row] != 0)
                    {
                        int curr = row;
                        while (curr < Board.GetLength(1) - 1)
                        {
                            if (merged.Contains(curr))
                            {
                                break;
                            }
                            if (Board[col, curr + 1] == 0)
                            {
                                Board[col, curr + 1] = Board[col, curr];
                                Board[col, curr] = 0;
                                curr++;
                                moveOccured = true;
                            }
                            else if (Board[col, curr + 1] == Board[col, curr] && !merged.Contains(curr + 1))
                            {
                                Board[col, curr + 1] += Board[col, curr];
                                Board[col, curr] = 0;
                                Score += Board[col, curr + 1];
                                merged.Add(curr + 1);
                                curr++;
                                moveOccured = true;
                            }
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
                Board = copy.Clone() as int[,];
            }

            return moveOccured;
        }

        bool Left(bool reset)
        {
            int[,] copy = Board.Clone() as int[,];
            bool moveOccured = false;
            for (int row = 0; row < Board.GetLength(1); row++)
            {
                HashSet<int> merged = new HashSet<int>();
                for (int col = 0; col < Board.GetLength(0); col++)
                {
                    if (Board[col, row] != 0)
                    {
                        int curr = col;
                        while (curr > 0)
                        {
                            if (merged.Contains(curr))
                            {
                                break;
                            }
                            if (Board[curr - 1, row] == 0)
                            {
                                Board[curr - 1, row] = Board[curr, row];
                                Board[curr, row] = 0;
                                curr--;
                                moveOccured = true;
                            }
                            else if (Board[curr - 1, row] == Board[curr, row] && !merged.Contains(curr - 1))
                            {
                                Board[curr - 1, row] += Board[curr, row];
                                Board[curr, row] = 0;

                                Score += Board[curr - 1, row];
                                merged.Add(curr - 1);
                                curr--;
                                moveOccured = true;
                            }
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
                Board = copy.Clone() as int[,];
            }

            return moveOccured;
        }

        bool Right(bool reset)
        {
            int[,] copy = Board.Clone() as int[,];
            bool moveOccured = false;
            for (int row = 0; row < Board.GetLength(1); row++)
            {
                HashSet<int> merged = new HashSet<int>();
                for (int col = Board.GetLength(0) - 1; col >= 0; col--)
                {
                    if (Board[col, row] != 0)
                    {
                        int curr = col;
                        while (curr < Board.GetLength(0) - 1)
                        {
                            if (merged.Contains(curr))
                            {
                                break;
                            }

                            if (Board[curr + 1, row] == 0)
                            {
                                Board[curr + 1, row] = Board[curr, row];
                                Board[curr, row] = 0;
                                curr++;
                                moveOccured = true;
                            }
                            else if (Board[curr + 1, row] == Board[curr, row] && !merged.Contains(curr + 1))
                            {
                                Board[curr + 1, row] += Board[curr, row];
                                Board[curr, row] = 0;
                                Score += Board[curr + 1, row];
                                merged.Add(curr + 1);
                                curr++;
                                moveOccured = true;
                            }
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
                Board = copy.Clone() as int[,];
            }

            return moveOccured;
        }
    }
}
