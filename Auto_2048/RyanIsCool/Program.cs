using FeedForwardNeuralNetwork;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System;
using System.IO;
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

            string json = File.ReadAllText("bestNet.json");
            Network net = JsonConvert.DeserializeObject<Network>(json);
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
                        int tileValue = int.Parse(blocks[1].Substring(5));

                        int y = int.Parse(blocks[2].Substring(14, 1)) - 1;
                        int x = int.Parse(blocks[2].Substring(16, 1)) - 1;
                        int index = y * 4 + x;
                        //tile-position-#-# 2dposition and translate to 1d array, if the new value is bigger replace, otherwise skip
                        if (inputs[index] < tileValue)
                        {
                            inputs[index] = tileValue;
                        }


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
