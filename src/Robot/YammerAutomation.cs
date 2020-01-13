using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;
using Events;
using Events.Yammer;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rest.Yammer;

namespace Robot
{
    public class YammerAutomation {
        private readonly ILogger _logger;
        private readonly EventRepository _events;
        private readonly RestEventManager _rest;
        private readonly UserRepository _users;
        private readonly string _token;

        public YammerAutomation(ILogger logger, string connectionString, string schema, string token) {
            _logger = logger;
            _events = new EventRepository(connectionString, schema);
            _rest = new RestEventManager(logger, connectionString, schema);
            _users = new UserRepository(connectionString, schema);
            _token = token;
            SqlMapperExtensions.TableNameMapper = (type) =>
            {
                return $"{schema}.{type.Name}s";
            };
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        }

        public async Task Process() {
            await _events.ReadForward(Network.Yammer, async (events) => { 
                var bodies = events
                    .Where(p => p.EventName == "RestApiRequest")
                    .Select(p => p.Body)
                    .ToList();
                foreach ( var body in bodies)
                {
                    dynamic json = JsonConvert.DeserializeObject(body);
                    
                    foreach (var reference in json.response.references) {

                        switch(reference.type.ToString()) {
                            case "user": await ProcessUser(Rest.Yammer.User.FromJson(reference.ToString()));
                            break;
                            default: 
                                _logger.LogWarning($"Unknown reference type: {reference.type}");
                            break;
                        }
                    }
                }
            });
        }

        public async Task ProcessUser(Rest.Yammer.User user)
        {
            var existing = await _users.GetByExternalId(Network.Yammer, user.Id.ToString());
            var received = Events.User.From(user);
            if (existing == null)
            {
                await _users.Add(received);
            } 
            else
            {
                received.Id = existing.Id;
            }
            await _events.Sync(Network.Yammer, received, existing, _logger, true);
        }
        
        public async Task Backfill()
        {
            // TODO
            //await _events.Migrate();
            
            long? last = null;
                do 
                {
                    var queryString = new { older_than = last };

                    var response = await _rest.Get(Network.Yammer, new MessagesSentRequest(_logger, YammerLimits.RateLimits), queryString, _token);
                    if (response != null) {
                        // foreach (var message in response.Messages)
                        // {
                        //     var existing = await new GetMessageById().Get(_events, message.Id.ToString());
                        //     await _events.Sync(_streamName, message, existing);
                        // }
                        // foreach (var user in response.References.Users) {
                            
                        //     var existing = await _users.GetByExternalId(Network.Yammer, user.Id.ToString());
                        //     await _events.Sync(Network.Yammer, user, existing, _logger, false);
                        // }
                        // foreach (var message in response.References.Messages) {
                        //     var existing = await new GetMessageById().Get(_events, message.Id.ToString());
                        //     await _events.Sync(_streamName, message, existing, false);
                        // }
                        // foreach (var group in response.References.Groups) {
                        //     var existing = await new GetGroupById().Get(_events, group.Id.ToString());
                        //     await _events.Sync(_streamName, group, existing, false);
                        // }
                        // foreach (var thread in response.References.Threads) {
                        //     var existing = await new GetThreadById().Get(_events, thread.Id.ToString());
                        //     await _events.Sync(_streamName, thread, existing, false);
                        // }


                        last = response.Messages.Last()?.Id;
                        _logger.LogInformation("Found {count} messages, last is {last}", response.Messages.Count(), last);
                    } else {
                        last = null;
                    }
                } while(last != null);
        }
    }
}