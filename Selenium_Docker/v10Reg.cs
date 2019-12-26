using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Support.UI;
using System.Drawing.Imaging;

namespace Selenium_Docker
{
    /// <summary>
    /// Summary description for MySeleniumTests
    /// </summary>
    [TestClass]
    public class v10Reg
    {
        private TestContext testContextInstance;
        private IWebDriver driver;
        private string appURL;

        public v10Reg()
        {
        }

        [TestMethod]
        [TestCategory("Chrome")]
        public void Login()
        {
            driver.Navigate().GoToUrl(appURL + "/index.aspx");
            driver.FindElement(By.XPath("//input[@id='username']")).SendKeys("Vnetadmin");
            driver.FindElement(By.XPath("//input[@id='password']")).SendKeys("vnetadmin");
            driver.FindElement(By.XPath("//input[@class='btn btn-default btn-lg btn-block']")).Click();
            Assert.IsTrue(driver.Title.Contains("Radial: Store Order Manager"), "Verified title of the page");
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            driver.FindElement(By.XPath("//i[@class='ss-icon ss-navigatedown ss-black-tie-bold reskin-dropdown-arrow']")).Click();
            driver.FindElement(By.XPath("//a[@id='reskin-logout']")).Click();
        }

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        [TestInitialize()]
        public void SetupTest()
        {
            appURL = "http://prio-snregmantest.vndev.nonprd.gsi.local";

            string browser = "Chrome";
            switch (browser)
            {
                case "Chrome":
                    driver = new ChromeDriver();
                    break;
                case "Firefox":
                    driver = new FirefoxDriver();
                    break;
                case "IE":
                    driver = new InternetExplorerDriver();
                    break;
                default:
                    driver = new ChromeDriver();
                    break;
            }

        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
            driver.Quit();
        }
    }
}