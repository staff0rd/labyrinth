using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace Events
{
    public class LinkedInBackfillCommandHandler : IRequestHandler<LinkedInBackfillCommand> {
        const string RECORD_XHR_BLOBS = @"
            const oldXHROpen = window.XMLHttpRequest.prototype.open;
            window.lresponses = [];
            window.XMLHttpRequest.prototype.open = function(method, url, async, user, password) {
                this.addEventListener('load', function() {
                    if (this.responseType === 'blob') {
                        const myReader = new FileReader();
                        myReader.onload = function(event){
                            window.lresponses.push({
                                url,
                                json: myReader.result
                            });
                        };
                        myReader.readAsText(this.response);
                    }
                });
                return oldXHROpen.apply(this, arguments);
            }
        ";
        private readonly ILogger<LinkedInBackfillCommandHandler> _logger;
        private readonly EventRepository _events;
        private readonly CredentialCache _credentials;

        public LinkedInBackfillCommandHandler(ILogger<LinkedInBackfillCommandHandler> logger, EventRepository events, 
        CredentialCache credentials) {
            _logger = logger;
            _events = events;
            _credentials = credentials;
        }
        
        public async Task<Unit> Handle(LinkedInBackfillCommand request, CancellationToken cancellationToken)
        {
            var credential = _credentials.Get(request.SourceId, request.Username);

            await Browse(credential, request.SourceId);

            return Unit.Value;
        }
        
        public async Task Browse(Credential credential, Guid sourceId) {
            var driver = new ChromeDriver(new ChromeOptions { PageLoadStrategy = PageLoadStrategy.Eager });
            WebDriverWait _wait = new WebDriverWait(driver, new TimeSpan(0, 0, 10));
            try {
                driver.Navigate().GoToUrl("https://www.linkedin.com/login");
                driver.ExecuteScript("console.log('hi!')");
                _wait.Until(d => d.FindElement(By.Id("username")));
                driver.FindElement(By.Id("username")).SendKeys(credential.ExternalIdentifier);
                var passwordTextfield = driver.FindElement(By.Id("password"));
                passwordTextfield.SendKeys(credential.ExternalSecret);
                var button = driver.FindElement(By.CssSelector("button[aria-label='Sign in']"));
                button.Click();
                await Task.Delay(1000);
                driver.Navigate().GoToUrl("https://www.linkedin.com/mynetwork/invite-connect/connections/");
                driver.ExecuteScript(RECORD_XHR_BLOBS);

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

                    var received = new Events.User {AvatarUrl = card.MugshotUrl, Description = card.Occupation, Id = card.ProfileUrl.AsId<User>(Network.LinkedIn), KnownSince = 
                    card.ConnectedDate, Name = card.Name, Network = Network.LinkedIn };
                    connections = driver.FindElementsByClassName("mn-connection-card");
                    var responses = driver.ExecuteScript("return window.lresponses;");
                    driver.ExecuteScript("window.lresponses.clear();");
                    await SaveResponses(credential, sourceId, responses);
                }

            }
            catch (Exception e) {
                _logger.LogError(e, "Failed");
            }
            finally {
                driver?.Quit();
            }
        }
    
        public async Task SaveResponses(Credential credential, Guid sourceId, object responses) {
            var jsonArray = JArray.FromObject(responses);
            foreach (dynamic item in jsonArray) {
                var payload = new JsonPayload { Url = item.url, Json = item.json };
                _logger.LogInformation($"Saving {payload.Url}");
                var json = JsonConvert.SerializeObject(payload);
                await _events.Add(credential, sourceId, Guid.NewGuid().ToString(), "JsonPayload", json);
            }
        }
    }
}