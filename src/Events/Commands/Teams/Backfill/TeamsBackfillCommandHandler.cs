using System;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;

namespace Events
{
    public class TeamsMessageData
    {
        public string Id { get; set; }
        public string Topic { get; set; }

    }
    public class TeamsRequestTypes {
        public const string Chats = "Chats";
        public const string ChatMessages = "Chat-Messages";
    }

    public class TeamsBackfillCommandHandler : IRequestHandler<TeamsBackfillCommand>
    {
        private readonly RestEventManager _rest;
        private readonly ILogger<TeamsBackfillCommandHandler> _logger;
        private readonly CredentialCache _credentials;
        private readonly EventRepository _events;
        private readonly Store _store;
        private readonly IProgress _progress;
        public static string GetImageDirectory(Guid sourceId) {
            return Path.Combine(System.IO.Directory.GetCurrentDirectory(), "images", sourceId.ToString());
        }

        public TeamsBackfillCommandHandler(ILogger<TeamsBackfillCommandHandler> logger, RestEventManager rest, Store store,
            EventRepository events, CredentialCache credentials, IProgress progress) {
            _rest = rest;
            _logger = logger;
            _credentials = credentials;
            _progress = progress;
            _store = store;
            _events = events;
        }

        public async Task<Unit> Handle(TeamsBackfillCommand request, CancellationToken cancellationToken)
        {
            if (!_store.IsHydrated)
                throw new Exception("Store must be hydrated first");
                
            var credential = _credentials.Get(request.SourceId, request.LabyrinthUsername);
            var client = GetGraphClient(credential.ExternalSecret);
            var chats = await client
                .Me
                .Chats
                .Request()
                .GetAsync();
            
            await _rest.SaveResponse(credential, request.SourceId, null, TeamsRequestTypes.Chats, null, chats.ToJson());
            
            int i = 0;
            foreach (var chat in chats)
            {
                i++;
                _progress.Set(i, chats.Count);
                try {
                    var messages = await client.Me.Chats[chat.Id].Messages.Request(new [] { new QueryOption("$top", "50")}).GetAsync();
                    do 
                    {   
                        foreach (var message in messages) {
                            var images = new ImageProcessor().GetImages(message, Network.Teams);
                            foreach (var image in images)
                            {
                                await _rest.DownloadImage(credential, request.SourceId, image, credential.ExternalSecret, GetImageDirectory(request.SourceId));
                                image.Created = message.CreatedDateTime.Value;
                                if (_store.GetImage(request.SourceId, image.FromEntityId, image.Url) == null)
                                {
                                    await _events.Add(credential, request.SourceId, image.FromEntityId, "ImageCreated", image.ToJson(), message.CreatedDateTime.Value.ToUnixTimeMilliseconds());
                                    _store.Add(request.SourceId, image);
                                }
                                message.Body.Content = message.Body.Content.Replace(image.Url, ImageProcessor.ImagePath(image));
                            }
                            
                                
                            
                        }
                        await _rest.SaveResponse(credential, request.SourceId, null, TeamsRequestTypes.ChatMessages, new TeamsMessageData { Id=chat.Id, Topic=chat.Topic }, messages.ToJson());
                        messages = messages?.NextPageRequest != null ? await messages.NextPageRequest.GetAsync() : null;
                    } while (messages != null && messages.Count > 0);
                } catch (ServiceException e)
                {
                    if (e.StatusCode == HttpStatusCode.Forbidden) {
                        _logger.LogError($"Access to {chat.Id} was Forbidden");
                        continue;
                    }
                }
            }
            return Unit.Value;
        }

        public GraphServiceClient GetGraphClient(string accessToken) {
            return new GraphServiceClient(new DelegateAuthenticationProvider((requestMessage) => {
                requestMessage
                    .Headers
                    .Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                return Task.FromResult(0);
            }));
        }
    }
}