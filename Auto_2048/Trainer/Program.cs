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
            Gamer derp = new Gamer(random);

            while (!derp.GameOver)
            {
                derp.Play(true);
            }
            Console.WriteLine("Game Over");
            while (true)
            {

            }
        }
    }
}
