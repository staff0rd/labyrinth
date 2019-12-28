using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace Robot.LinkedIn
{
    public class LinkedInAutomation {
        private readonly ILogger _logger;
        private readonly EventStoreManager _events;
        private readonly string _username;

        private readonly string _password;

        private string _streamName = "LinkedIn";

        public LinkedInAutomation(ILogger logger, string username, string password) {
            _logger = logger;
            _events = new EventStoreManager(logger, _streamName);
            _username = username;
            _password = password;
        }
        
        public async Task Automate() {
            await Task.Delay(0);
            var driver = new ChromeDriver();
            WebDriverWait _wait = new WebDriverWait(driver, new TimeSpan(0, 0, 10));
            try {
                driver.Navigate().GoToUrl("https://www.linkedin.com/login");
                _wait.Until(d => d.FindElement(By.Id("username")));
                driver.FindElement(By.Id("username")).SendKeys(_username);
                var passwordTextfield = driver.FindElement(By.Id("password"));
                passwordTextfield.SendKeys(_password);
                var button = driver.FindElement(By.CssSelector("button[aria-label='Sign in']"));
                button.Click();
                await Task.Delay(1000);
                driver.Navigate().GoToUrl("https://www.linkedin.com/mynetwork/invite-connect/connections/");
                _wait.Until(d => d.FindElement(By.CssSelector("[aria-label='Sorting options for connections list']")));
                var dropdown = driver.FindElement(By.CssSelector("[aria-label='Sorting options for connections list']"));
                dropdown.Click();
                driver.FindElement(By.CssSelector("[data-control-name='sort_by_recently_added']")).Click();
                
                _wait.Until(ExpectedConditions.ElementExists(By.CssSelector("button[aria-label^='Send message']")));
                int s = int.Parse(driver.ExecuteScript("return document.getElementsByClassName('mn-connection-card').length").ToString());
                Console.WriteLine(s);
                s = (int)driver.ExecuteScript("return document.getElementsByClassName('mn-cssdonnection-card').length");
                Console.WriteLine(s);
            }
            catch (Exception e) {
                _logger.LogError(e, "Failed");
            }
            finally {
                driver?.Quit();
            }
            // await _events.CreateOrUpdateProjection($"{_streamName}Users", Queries.Users(_streamName));
            //await _events.Sync(user, "User", await GetUser(user.Id));

        }
        
        // internal async Task<User> GetUser(long id)
        // {
        //     var result = await _events.GetPartitionState($"{_streamName}Users", id.ToString());

        //     if (result == "")
        //         return null;

        //     return User.FromJson(result.ToString());
        // }
    }
}