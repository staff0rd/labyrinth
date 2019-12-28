using System;
using System.Threading.Tasks;
using Browser.LinkedIn;
using Events;
using Events.LinkedIn;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace Robot
{
    public class LinkedInAutomation {
        private readonly ILogger _logger;
        private readonly EventStoreManager _events;
        private readonly Queries _queries;
        private readonly string _username;

        private readonly string _password;

        private string _streamName = Queries.StreamName;

        public LinkedInAutomation(ILogger logger, string username, string password) {
            _logger = logger;
            _events = new EventStoreManager(logger, _streamName);
            _queries = new Queries(_events);
            _username = username;
            _password = password;
        }
        
        public async Task Automate() {
            await _events.CreateOrUpdateProjection($"{_streamName}{nameof(_queries.UserById)}", _queries.UserById());
            await _events.CreateOrUpdateProjection($"{_streamName}{nameof(_queries.AllUsers)}", _queries.AllUsers());

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
                var connections = driver.FindElementsByClassName("mn-connection-card");
                var totalConnections = connections.Count;
                for (int i = 0; i < connections.Count; i++)
                {
                    var connection = connections[i];
                    var card = new ConnectionCard(connection);
                    driver.ExecuteScript("arguments[0].scrollIntoView()", connection);

                    User user = User.Create(card.Name, card.MugshotUrl, card.ProfileUrl, card.Occupation, card.ConnectedDate);
                    await _events.Sync(user, "User", await _queries.GetUser(user.Id), true);
                    connections = driver.FindElementsByClassName("mn-connection-card");
                }
            }
            catch (Exception e) {
                _logger.LogError(e, "Failed");
            }
            finally {
                driver?.Quit();
            }
        }
    }
}