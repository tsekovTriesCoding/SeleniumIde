using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;

[TestFixture]
public class TC01IfUserIsInvalidTryAgainTest
{
    private IWebDriver driver;
    private string tempUserDataDir;
    public IDictionary<string, object> Vars { get; private set; }
    private IJavaScriptExecutor js;

    [SetUp]
    public void SetUp()
    {
        try
        {
            // Create isolated user-data-dir to avoid session errors
            tempUserDataDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempUserDataDir);

            var options = new ChromeOptions();
            options.AddArgument("--headless=new");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--window-size=1920x1080");
            options.AddArgument($"--user-data-dir={tempUserDataDir}");

            driver = new ChromeDriver(options);
            js = (IJavaScriptExecutor)driver;
            Vars = new Dictionary<string, object>();
        }
        catch (Exception ex)
        {
            Assert.Fail($"Setup failed: {ex.Message}");
        }
    }

    [TearDown]
    protected void TearDown()
    {
        try
        {
            if (driver != null)
            {
                driver.Quit();
                driver.Dispose();
                driver = null;
            }

            if (!string.IsNullOrEmpty(tempUserDataDir) && Directory.Exists(tempUserDataDir))
            {
                Directory.Delete(tempUserDataDir, true);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Teardown encountered an error: {ex.Message}");
        }
    }

    [Test]
    public void TC01IfUserIsInvalidTryAgain()
    {
        driver.Navigate().GoToUrl("https://www.saucedemo.com/");
        driver.Manage().Window.Size = new System.Drawing.Size(1552, 832);

        driver.FindElement(By.CssSelector("*[data-test=\"username\"]")).SendKeys("user123");
        driver.FindElement(By.CssSelector("*[data-test=\"password\"]")).SendKeys("secret_sauce");
        driver.FindElement(By.CssSelector("*[data-test=\"login-button\"]")).Click();

        Vars["errorMessage"] = driver.FindElement(By.CssSelector("*[data-test=\"error\"]")).Text;
        if ((Boolean)js.ExecuteScript("return (arguments[0] === 'Epic sadface: Username and password do not match any user in this service')", Vars["errorMessage"]))
        {
            Console.WriteLine("Wrong username");
            var usernameInput = driver.FindElement(By.CssSelector("*[data-test=\"username\"]"));
            usernameInput.Clear();
            usernameInput.SendKeys("standard_user");

            driver.FindElement(By.CssSelector("*[data-test=\"login-button\"]")).Click();
            Assert.That(driver.FindElement(By.CssSelector("*[data-test=\"title\"]")).Text, Is.EqualTo("Products"));
            Console.WriteLine("Successful login");
        }
    }
}
