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

        private readonly string _imageDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "images");
        private readonly string _imageDirectoryNew = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "images-new");

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

                    if (payload.Category == TeamsRequestTypes.ChatMessages) {
                        var data = JsonConvert.DeserializeObject<TeamsMessageData>(payload.Data);

                        var chatMessagesCollectionPage = JsonConvert.DeserializeObject<IChatMessagesCollectionPage>(payload.Response);
                        foreach (var chatMessage in chatMessagesCollectionPage)
                        {
                            var message = Events.Message.From(chatMessage, request.SourceId, data.Id);
                            var images = _store.GetImages(request.SourceId, message.Id);
                            foreach (var image in images)
                            {
                                if (image.Url.EndsWith("$value")) {
                                    System.IO.File.Move(Path.Combine(_imageDirectory, image.Id), Path.Combine(_imageDirectoryNew, image.Id));
                                    chatMessage.Body.Content = chatMessage.Body.Content.Replace(image.Url, ImageProcessor.ImagePath(image));
                                }
                            }
                        }
                        payload.Response = chatMessagesCollectionPage.ToJson();
                    }
                    await _events.Add(creds, Guid.Parse("c1bfac18-ff7e-4f21-890d-c61a76890a2e"), Guid.NewGuid().ToString(),
                        "RestApiRequest", payload.ToJson(), DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
                }
                return events.Last().Id;
            });

            _logger.LogInformation("Completed processing");
            return Unit.Value;
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
                        await ProcessMessages(creds, request.SourceId, data.id as string, JsonConvert.DeserializeObject<IChatMessagesCollectionPage>(payload.Response), creds.ExternalSecret);
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

        private async Task ProcessImages(Credential creds, Guid sourceId, Message message, string token)
        {
            var images = new ImageProcessor().Process(message);
            foreach (var image in images)
            {
                if (_store.GetImage(sourceId, image.FromEntityId, image.Url) == null)
                {
                    await _rest.DownloadImage(creds, sourceId, image, token, _imageDirectory);
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
                await ProcessImages(creds, sourceId, received, token);
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