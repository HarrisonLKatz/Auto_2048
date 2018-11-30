using System;
using System.Collections.Generic;
using System.Text;

namespace Trainer
{
    public class Board
    {
        public int Score;
        public bool GameOver;
        public int[,] Values;
        Random random;

        public Board(Random gen)
        {
            GameOver = false;
            Values = new int[4, 4];
            random = gen;
            AddRandomTile();
            AddRandomTile();
        }

        public void AddRandomTile()
        {
            //find all open spots
            List<(int, int)> openSpots = new List<(int, int)>(16);
            for (int i = 0; i < Values.GetLength(0); i++)
            {
                for (int j = 0; j < Values.GetLength(1); j++)
                {
                    if (Values[i, j] == 0)
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
            Values[x, y] = random.NextDouble() < 0.9 ? 2 : 4;
        }

        public bool Up(bool reset) // 0 top, length bot
        {
            int[,] copy = Values.Clone() as int[,];


            bool moveOccured = false;
            //for every column
            for (int col = 0; col < Values.GetLength(0); col++)
            {
                HashSet<int> merged = new HashSet<int>();
                for (int row = 0; row < Values.GetLength(1); row++)
                {
                    if (Values[col, row] != 0)
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
                            if (Values[col, curr - 1] == 0)
                            {
                                //move up by swapping values
                                Values[col, curr - 1] = Values[col, curr];
                                Values[col, curr] = 0;
                                curr--;
                                moveOccured = true;
                            }
                            //if the above spot is the same value and not merged
                            else if (Values[col, curr - 1] == Values[col, curr] && !merged.Contains(curr - 1))
                            {
                                //move up by merging values
                                Values[col, curr - 1] += Values[col, curr];
                                Values[col, curr] = 0;

                                Score += Values[col, curr - 1];

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
                Values = copy.Clone() as int[,];
            }

            return moveOccured;
        }

        public bool Down(bool reset)
        {
            int[,] copy = Values.Clone() as int[,];
            bool moveOccured = false;
            for (int col = 0; col < Values.GetLength(0); col++)
            {
                HashSet<int> merged = new HashSet<int>();
                for (int row = Values.GetLength(1) - 1; row >= 0; row--)
                {
                    if (Values[col, row] != 0)
                    {
                        int curr = row;
                        while (curr < Values.GetLength(1) - 1)
                        {
                            if (merged.Contains(curr))
                            {
                                break;
                            }
                            if (Values[col, curr + 1] == 0)
                            {
                                Values[col, curr + 1] = Values[col, curr];
                                Values[col, curr] = 0;
                                curr++;
                                moveOccured = true;
                            }
                            else if (Values[col, curr + 1] == Values[col, curr] && !merged.Contains(curr + 1))
                            {
                                Values[col, curr + 1] += Values[col, curr];
                                Values[col, curr] = 0;
                                Score += Values[col, curr + 1];
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
                Values = copy.Clone() as int[,];
            }

            return moveOccured;
        }

        public bool Left(bool reset)
        {
            int[,] copy = Values.Clone() as int[,];
            bool moveOccured = false;
            for (int row = 0; row < Values.GetLength(1); row++)
            {
                HashSet<int> merged = new HashSet<int>();
                for (int col = 0; col < Values.GetLength(0); col++)
                {
                    if (Values[col, row] != 0)
                    {
                        int curr = col;
                        while (curr > 0)
                        {
                            if (merged.Contains(curr))
                            {
                                break;
                            }
                            if (Values[curr - 1, row] == 0)
                            {
                                Values[curr - 1, row] = Values[curr, row];
                                Values[curr, row] = 0;
                                curr--;
                                moveOccured = true;
                            }
                            else if (Values[curr - 1, row] == Values[curr, row] && !merged.Contains(curr - 1))
                            {
                                Values[curr - 1, row] += Values[curr, row];
                                Values[curr, row] = 0;

                                Score += Values[curr - 1, row];
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
                Values = copy.Clone() as int[,];
            }

            return moveOccured;
        }

        public bool Right(bool reset)
        {
            int[,] copy = Values.Clone() as int[,];
            bool moveOccured = false;
            for (int row = 0; row < Values.GetLength(1); row++)
            {
                HashSet<int> merged = new HashSet<int>();
                for (int col = Values.GetLength(0) - 1; col >= 0; col--)
                {
                    if (Values[col, row] != 0)
                    {
                        int curr = col;
                        while (curr < Values.GetLength(0) - 1)
                        {
                            if (merged.Contains(curr))
                            {
                                break;
                            }

                            if (Values[curr + 1, row] == 0)
                            {
                                Values[curr + 1, row] = Values[curr, row];
                                Values[curr, row] = 0;
                                curr++;
                                moveOccured = true;
                            }
                            else if (Values[curr + 1, row] == Values[curr, row] && !merged.Contains(curr + 1))
                            {
                                Values[curr + 1, row] += Values[curr, row];
                                Values[curr, row] = 0;
                                Score += Values[curr + 1, row];
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
                Values = copy.Clone() as int[,];
            }

            return moveOccured;
        }
    }
}
