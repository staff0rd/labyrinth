using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rest.Yammer;

namespace Events
{
    public class YammerStore
    {
        private Dictionary<string, Message> _messages;
        public ReadOnlyDictionary<string, Message> Messages => new ReadOnlyDictionary<string, Message>(_messages);
        private readonly EventRepository _events;
        private Dictionary<string, User> _users;
        public ReadOnlyDictionary<string, User> Users => new ReadOnlyDictionary<string, User>(_users);
        private readonly ILogger _logger;

        public YammerStore(EventRepository events, ILogger logger) {
            _events = events;
            _logger = logger;
        }

        public Events.Yammer.Overview GetOverview() {
            return new Yammer.Overview
            {
                Messages = _messages.Count,
                Users = _users.Count
            };
        }

        public async Task Hydrate()
        {
            var sw = Stopwatch.StartNew();
           
            var messages = new Dictionary<string, Message>();
            await _events.ReadForward(Network.Yammer, (events) => { 
                events
                    .Where(p => p.EventName == "MessageCreated")
                    .Select(p => JsonConvert.DeserializeObject<Message>(p.Body))
                    .ToList()
                    .ForEach(message => {
                        if (!messages.ContainsKey(message.Id))
                            messages.Add(message.Id, message);
                    });
                return Task.CompletedTask;
            });
            _messages = messages;

            var users = new Dictionary<string, User>();
            await _events.ReadForward(Network.Yammer, (events) => {
                events
                    .Where(p => p.EventName == "UserCreated")
                    .Select(p => JsonConvert.DeserializeObject<User>(p.Body))
                    .ToList()
                    .ForEach(user => {
                        if (!users.ContainsKey(user.Id))
                            users.Add(user.Id, user);
                        
                    });
                return Task.CompletedTask;
            });
            _users = users;
            _logger.LogInformation($"It look {sw.Elapsed} to read network {Network.Yammer}, {_users.Count} users, {_messages.Count} messages");
        }

        public void Add(User user)
        {
            _users.Add(user.Id.ToString(), user);
            _logger.LogInformation($"Added user {user.Id} to {Network.Yammer}");
        }

        public void Add(Message message)
        {
            _messages.Add(message.Id.ToString(), message);
            _logger.LogInformation($"Added message {message.Id} to {Network.Yammer}");
        }
    }
}