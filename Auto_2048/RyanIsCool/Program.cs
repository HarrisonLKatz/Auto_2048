using FeedForwardNeuralNetwork;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System;
using System.Linq;

namespace RyanIsCool
{
    class Program
    {
        static void Main(string[] args)
        {
            IWebDriver driver = new ChromeDriver("./");
            driver.Navigate().GoToUrl("https://gabrielecirulli.github.io/2048/");

            // 0: up, 1: right, 2: down, 3: left
            Actions builder = new Actions(driver);

            Network net = JsonConvert.DeserializeObject<Network>("bestNet.json");
            //net: 16 input, 4 output

            while (true)
            {
                //get board state
                IWebElement tiles = driver.FindElement(By.ClassName("tile-container"));
                System.Collections.ObjectModel.ReadOnlyCollection<IWebElement> children = tiles.FindElements(By.XPath(".//*"));

                double[] inputs = new double[16];
                foreach (IWebElement child in children)
                {
                    string className = child.GetAttribute("class");
                    if (className != "tile-inner")
                    {
                        string[] blocks = className.Split(' ');
                        int tile = int.Parse(blocks[1].Substring(5));

                        int x = int.Parse(blocks[2].Substring(14, 1));
                        int y = int.Parse(blocks[2].Substring(16, 1));
                        //tile-position-#-# 2d position and translate to 1d array, if the new value is bigger replace, otherwise skip
                    }
                }


                double[] outputs = net.Compute(inputs);

                //perform move
                int pick = Array.IndexOf(outputs, outputs.Max());
                switch (pick)
                {
                    case 0: //up
                        builder.SendKeys("w");
                        break;
                    case 1: //down
                        builder.SendKeys("s");
                        break;
                    case 2: //left
                        builder.SendKeys("a");
                        break;
                    case 3: //right
                        builder.SendKeys("d");
                        break;
                }
                builder.Build().Perform();


            }
        }
    }
}
