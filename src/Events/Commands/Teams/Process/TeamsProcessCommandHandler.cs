using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Events.LinkedIn;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rest;

namespace Events
{
    public class TeamsProcessCommandHandler : IRequestHandler<TeamsProcessCommand>
    {
        private readonly ILogger<TeamsProcessCommandHandler> _logger;
        private readonly CredentialCache _credentials;
        private readonly EventRepository _events;
        private readonly IProgress _progress;
        private readonly Store _store;
        private readonly RestEventManager _rest;

        public TeamsProcessCommandHandler(
            ILogger<TeamsProcessCommandHandler> logger, CredentialCache credentials, Store store,
            EventRepository events, IProgress progress, RestEventManager rest)
        {
            _logger = logger;
            _credentials = credentials;
            _store = store;
            _events = events;
            _progress = progress;
            _rest = rest;
        }

        public async Task<Unit> Handle(TeamsProcessCommand request, CancellationToken cancellationToken)
        {
            
            if (!_store.IsHydrated)
                throw new Exception("Store must be hydrated first");

            var creds = _credentials.Get(request.SourceId, request.Username);

            var count = await _events.GetCount(creds.Username, request.SourceId, "RestApiRequest");

            var currentCount = 0;
                
            await _events.ReadForward(creds, request.SourceId, count, async (events, totalEvents) => { 
                var requests = events
                    .Where(p => p.EventName == "RestApiRequest")
                    .ToList();
                foreach (var apiRequest in requests)
                {
                    currentCount++;
                    _progress.Set(currentCount, totalEvents);

                    var payload = JsonConvert.DeserializeObject<RestApiRequest>(apiRequest.Body);

                    if (payload.Category == TeamsRequestTypes.Chats) {
                        await Process(creds, request.SourceId, JsonConvert.DeserializeObject<IUserChatsCollectionPage>(payload.Response));
                    } else if (payload.Category == TeamsRequestTypes.ChatMessages) {
                        dynamic data = JObject.Parse(payload.Data);
                        await ProcessMessages(creds, request.SourceId, data.id, JsonConvert.DeserializeObject<IChatMessagesCollectionPage>(payload.Response), creds.ExternalSecret);
                    } else
                        throw new NotImplementedException(payload.Category);
                }
                return events.Last().Id;
            });

            _logger.LogInformation("Completed processing");
            return Unit.Value;
        }

        private async Task Process(Credential creds, Guid sourceId, IUserChatsCollectionPage userChatsCollectionPage)
        {
            foreach (var chat in userChatsCollectionPage)
            {
                var received = Events.Topic.From(chat);
                var existing = _store.GetTopic(sourceId, received.Id);
                if (existing == null)
                {
                    _store.Add(sourceId, received);
                } 
                await _events.Sync(creds, sourceId, received, existing, received.LastUpdated.ToUnixTimeMilliseconds());
            }
        }
        private async Task Process(Credential creds, Guid sourceId, Identity user)
        {
            if (!_store.IsHydrated)
                throw new Exception("Store must be hydrated first");

            var received = Events.User.From(user, sourceId);
            var existing = _store.GetUser(sourceId, received.Id);
            if (existing == null)
            {
                _store.Add(sourceId, received);
            } 
            await _events.Sync(creds, sourceId, received, existing, Math.Min(received.KnownSince.ToUnixTimeMilliseconds(), received.KnownSince.ToUnixTimeMilliseconds()));
        }

        private async Task ProcessImages(Credential creds, Guid sourceId, ChatMessage message, string token)
        {
            var images = new ImageProcessor().GetImages(message, Network.Teams);
            foreach (var image in images)
            {
                if (_store.GetImage(sourceId, image.FromEntityId, image.Url) == null)
                {
                    await _events.Add(creds, sourceId, image.FromEntityId, "ImageCreated", image.ToJson(), message.CreatedDateTime.Value.ToUnixTimeMilliseconds());
                    _store.Add(sourceId, image);
                }
            }
        }

        private async Task ProcessMessages(Credential creds, Guid sourceId, string topicId, IChatMessagesCollectionPage chatMessagesCollectionPage, string token)
        {
            foreach (var message in chatMessagesCollectionPage)
            {
                await Process(creds, sourceId, message.From.User);
                var received = Events.Message.From(message, sourceId, topicId);
                await ProcessImages(creds, sourceId, message, token);
                var existing = _store.GetMessage(sourceId, received.Id);
                if (existing == null)
                {
                    _store.Add(sourceId, received);
                } 
                await _events.Sync(creds, sourceId, received, existing, message.LastModifiedDateTime?.ToUnixTimeMilliseconds() ?? message.CreatedDateTime.Value.ToUnixTimeMilliseconds());
            }
        }
    }
}