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

            //net: 16 input, 4 output

            while (true)
            {
                //get board state
                IWebElement tiles = driver.FindElement(By.ClassName("tile-container"));
                var children = tiles.FindElements(By.XPath(".//*"));

                double[] inputs = new double[16];
                foreach (IWebElement child in children)
                {
                    string className = child.GetAttribute("class");
                    if (className != "tile-inner")
                    {
                        //tile-#
                        //tile-position-#-# 2d position and translate to 1d array, if the new value is bigger replace, otherwise skip
                    }
                }

                ////calculate move
                //double[] outputs = new double[4];
                ////use neural net

                ////perform move
                //int pick = Array.IndexOf(outputs, outputs.Max());
                //switch (pick)
                //{
                //    case 0:
                //        builder.SendKeys("w");
                //        break;
                //    case 1:
                //        builder.SendKeys("a");
                //        break;
                //    case 2:
                //        builder.SendKeys("s");
                //        break;
                //    case 3:
                //        builder.SendKeys("d");  
                //        break;
                //}
                //builder.Build().Perform();


                driver.FindElement(By.ClassName("game-message"));
            }
        }
    }
}
