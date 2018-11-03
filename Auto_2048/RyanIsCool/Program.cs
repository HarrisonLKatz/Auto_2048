using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System;

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

            while (true)
            {
                //get board state
                IWebElement tiles = driver.FindElement(By.ClassName("tile-container"));
                 

                //calculate move

                //perform move
                builder.SendKeys("s").Perform();
                builder.SendKeys("d").Perform();

                //check game lost

            }
        }
    }
}
