using System;
using System.IO;
using System.Reflection;
//using Xunit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Chrome;

namespace WebUI.SeleniumTests
{
    [TestClass]
    public class IndexTests : IDisposable
    {
        const string baseUrl = "http://territory.bellevuemandarin.org";

        readonly ChromeDriver driver;
        static TestContext _testContext;

        public IndexTests()
        {
            var workingDirectory = Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location);

            var options = new ChromeOptions();
            options.AddArguments("headless");

            driver = new ChromeDriver(
                workingDirectory, 
                options, 
                TimeSpan.FromSeconds(60));
        }

        public TestContext TestContext { get; set; }

        [ClassInitialize]
        public static void SetupTests(TestContext testContext)
        {
            _testContext = testContext;
        }

        [TestInitialize]
        public void SetUp()
        {
            TestContext = _testContext;
        }

        public void Dispose()
        {
            driver.Close();
            driver.Dispose();
        }

        //[Fact]
        [TestMethod]
        public void Index_Welcome_Present()
        {
            driver.Navigate().GoToUrl(baseUrl);

            IWebElement element = driver.FindElement(By.TagName("h1"));
            Assert.AreEqual("Welcome", element.Text);
        }

        [TestMethod]
        public void Index_MenuLogin_Present()
        {
            driver.Navigate().GoToUrl(baseUrl);

            IWebElement element = driver.FindElement(
                By.XPath("//a[@href='/Identity/Account/Login']"));

            Assert.AreEqual("Login", element.Text);
        }

        [TestMethod]
        public void Index_LoginPasswordMenu_Present()
        {
            driver.Navigate().GoToUrl(baseUrl + "/Identity/Account/Login");

            IWebElement element = driver.FindElement(
                By.XPath("//a[@href='LoginPassword']"));

            Assert.AreEqual("Account & Password", element.Text);
        }

        [TestMethod]
        public void Index_AccountPassword_CheckTitle()
        {
            driver.Navigate().GoToUrl(baseUrl + "/Identity/Account/LoginPassword");

            IWebElement element = driver.FindElement(
                By.XPath("//h3"));

            Assert.AreEqual("Use an Account and Password", element.Text);

            Console.WriteLine("Password: " + System.Environment.GetEnvironmentVariable("PATH"));
        }

        [TestMethod]
        public void Index_LoginWithPassword_ReturnsError()
        {
            driver.Navigate().GoToUrl(baseUrl + "/Identity/Account/LoginPassword");

            var un = driver.FindElement(
                By.XPath("//input[@name='Input.Email']"));
            
            un.SendKeys("user@domain.test");
            
            var pw = driver.FindElement(
                By.XPath("//input[@name='Input.Password']"));
            
            pw.SendKeys("wrong password");

            pw.Submit();

            IWebElement element = driver.FindElement(
                By.XPath("//div[@id='validation-summary']/ul/li"));

            Assert.AreEqual("Invalid login attempt.", element.Text);
        }

        // [TestMethod]
        // public void Index_LoginCredentials_AllCaps_Available()
        // {
        //     string userName = System.Environment.GetEnvironmentVariable("LOGINUSERNAME");
        //     string password = System.Environment.GetEnvironmentVariable("LOGINPASSWORD");

        //     Assert.IsNotNull(userName);
        //     Assert.IsNotNull(password);
        // }

        // [TestMethod]
        // public void Index_LoginCredentials_PrefixedAllCaps_Available()
        // {
        //     string userName = System.Environment.GetEnvironmentVariable("KEYVAULT_LOGINUSERNAME");
        //     string password = System.Environment.GetEnvironmentVariable("KEYVAULT_LOGINPASSWORD");

        //     Assert.IsNotNull(userName);
        //     Assert.IsNotNull(password);
        // }

        // [TestMethod]
        // public void Index_LoginCredentialUserName_Available()
        // {
        //     string userName = System.Environment.GetEnvironmentVariable("LoginUserName");

        //     Assert.IsNotNull(userName);
        // }

        // [TestMethod]
        // public void Index_LoginCredentialPassword_Available()
        // {
        //     string password = System.Environment.GetEnvironmentVariable("LoginPassword");

        //     Assert.IsNotNull(password);
        // }

        // [TestMethod]
        // public void Index_LoginWithPassword_CheckTitle()
        // {
        //     string userName = System.Environment.GetEnvironmentVariable("LoginUserName");
        //     string password = System.Environment.GetEnvironmentVariable("LoginPassword");

        //     driver.Navigate().GoToUrl(baseUrl + "/Identity/Account/LoginPassword");

        //     var un = driver.FindElement(
        //         By.XPath("//input[@name='Input.Email']"));
            
        //     un.SendKeys(userName);
            
        //     var pw = driver.FindElement(
        //         By.XPath("//input[@name='Input.Password']"));
            
        //     pw.SendKeys(password);

        //     pw.Submit();

        //     IWebElement element = driver.FindElement(
        //         By.XPath("//h4"));

        //     Assert.AreEqual("Your Territories", element.Text);
        // }
    }
}
