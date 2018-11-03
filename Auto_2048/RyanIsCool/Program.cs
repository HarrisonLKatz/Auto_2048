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
                double[] inputs = new double[16];
                IWebElement tiles = driver.FindElement(By.ClassName("tile-container"));
                System.Collections.ObjectModel.ReadOnlyCollection<IWebElement> children = tiles.FindElements(By.XPath(".//*"));
                foreach (IWebElement child in children)
                {
                    //try here? error occurs occasionally
                    string className = child.GetAttribute("class");
                    if (className != "tile-inner")
                    {
                        //tile-#
                        //tile-position-#-#
                        //fill inputs array
                    }
                }

                //calculate move
                double[] outputs = new double[4];
                //use neural net

                //perform move
                int pick = Array.IndexOf(outputs, outputs.Max());
                switch (pick)
                {
                    case 0:
                        builder.SendKeys("w").Perform();
                        break;
                    case 1:
                        builder.SendKeys("a").Perform();
                        break;
                    case 2:
                        builder.SendKeys("s").Perform();
                        break;
                    case 3:
                        builder.SendKeys("d").Perform();
                        break;
                }

                //check game lost
                try
                {
                    driver.FindElement(By.ClassName("game-message"));
                }
                catch
                {
                    //calculate fitness (largest number created)
                }

            }
        }
    }
}
