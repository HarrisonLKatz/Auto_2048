using FeedForwardNeuralNetwork;
using System;
using System.Linq;

namespace TrainerGpu
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

        public void Play(Random random)
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
