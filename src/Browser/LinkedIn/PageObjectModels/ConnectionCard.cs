using System;
using OpenQA.Selenium;

namespace Browser.LinkedIn
{
    public class ConnectionCard {
        private readonly IWebElement _div;

        public string MugshotUrl => _div.FindElement(By.ClassName("presence-entity__image")).GetAttribute("src");

        public string ProfileUrl => _div.FindElement(By.ClassName("mn-connection-card__picture")).GetAttribute("href");

        public string Name => _div.FindElement(By.ClassName("mn-connection-card__name")).Text;

        public string Occupation => _div.FindElement(By.ClassName("mn-connection-card__occupation")).Text;

        public string Connected => _div.FindElement(By.ClassName("time-badge")).Text;

        public DateTimeOffset ConnectedDate => new RelativeTimeConverter().Convert(Connected);

        public ConnectionCard(IWebElement div) {
            _div = div;
        }
    }
}