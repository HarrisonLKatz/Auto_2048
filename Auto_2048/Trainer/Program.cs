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

            while (true)
            {
                derp.Play(true);
            }
        }
    }
}
