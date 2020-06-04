using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;

namespace Events
{
    public class TeamsBackfillCommandHandler : IRequestHandler<TeamsBackfillCommand, Result>
    {
        private readonly string[] _tokenUrls = new [] {
            "graph.microsoft.com",
        };
        private readonly RestEventManager _rest;
        private readonly ILogger<TeamsBackfillCommandHandler> _logger;
        private readonly EventRepository _events;
        private readonly Store _store;
        private readonly IMediator _mediator;
        private readonly IProgress _progress;
        public static string GetImageDirectory(Guid sourceId) {
            return Path.Combine(System.IO.Directory.GetCurrentDirectory(), "images", sourceId.ToString());
        }
        public TeamsBackfillCommandHandler(ILogger<TeamsBackfillCommandHandler> logger, RestEventManager rest, Store store,
            EventRepository events, IProgress progress, IMediator mediator) {
            _rest = rest;
            _logger = logger;
            _progress = progress;
            _store = store;
            _events = events;
            _mediator = mediator;
        }

        public async Task<Result> Handle(TeamsBackfillCommand request, CancellationToken cancellationToken)
        {
            if (!_store.IsHydrated)
            {
                _logger.LogInformation("Hydrating store...");
                await _mediator.Send(new HydrateCommand { LabyrinthUsername = request.LabyrinthUsername, LabyrinthPassword = request.LabyrinthPassword });
            }
                
            var credential = new Credential(request.LabyrinthUsername, request.LabyrinthPassword);
            var client = GetGraphClient(request.Token);
            var chats = await client
                .Me
                .Chats
                .Request()
                .GetAsync();
            
            await _rest.SaveResponse(credential, request.SourceId, null, TeamsRequestTypes.Chats, null, chats.ToJson());
            var i = 0;

            foreach (var chat in chats)
            {
                await ProcessChat(chat, client, request, credential);
                i++;
                await _progress.Set(i, chats.Count);
            }

            return Result.Ok();
        }

        private async Task ProcessChat(Chat chat, GraphServiceClient client, TeamsBackfillCommand request, Credential credential)
        {
                int messageCount = 0;
                var tasks = new List<Task>();
                try {
                    var messages = await client.Me.Chats[chat.Id].Messages.Request(new [] { new QueryOption("$top", "50")}).GetAsync();
                    do
                    {
                        tasks.Add(ProcessMessages(request, credential, chat, messages));
                        messageCount += messages.Count;
                        _logger.LogInformation($"Chat messages queued for processing: {messageCount}");
                        messages = messages?.NextPageRequest != null ? await messages.NextPageRequest.GetAsync() : null;
                    } while (messages != null && messages.Count > 0);
                } catch (ServiceException e)
                {
                    if (e.StatusCode == HttpStatusCode.Forbidden) {
                        _logger.LogError($"Access to {chat.Id} was Forbidden");
                        return;
                    }
                }
                _logger.LogInformation("Waiting on processing tasks...");
                Task.WaitAll(tasks.ToArray());
                _logger.LogInformation("Chat processed");
        }

        private async Task ProcessMessages(TeamsBackfillCommand request, Credential credential, Chat chat, IChatMessagesCollectionPage messages)
        {
            foreach (var message in messages)
            {
                var images = new ImageProcessor().GetImages(message, Network.Teams);
                foreach (var image in images)
                {
                    try
                    {
                        if (_store.GetImage(request.SourceId, image.FromEntityId, image.Url) == null)
                        {
                            try
                            {
                                await _rest.DownloadImage(credential, request.SourceId, image.Id, image.Url, request.Token, GetImageDirectory(request.SourceId), _tokenUrls);
                            }
                            catch (Exception e)
                            {
                                _logger.LogError(e.Message);
                                var url = image.Url;
                                var hostedContentIds = Regex.Matches(url, @"hostedContents\/(.+)\/\$value").GetGroupMatches();
                                if (hostedContentIds.Length == 1)
                                {
                                    var hostedContent = hostedContentIds.First();
                                    var id = ExtractUrlFromHostedContent(url, hostedContent);
                                    if (string.IsNullOrEmpty(id.Url))
                                    {
                                        _logger.LogWarning($"hostedContents had no url: {Base64Decode(hostedContent)}");
                                    }
                                    await _rest.DownloadImage(credential, request.SourceId, image.Id, id.Url, request.Token, GetImageDirectory(request.SourceId), _tokenUrls);
                                }
                                else
                                {
                                    _logger.LogError("Unable to find hostedContent");
                                    throw;
                                }
                            }
                            image.Created = message.CreatedDateTime.Value;
                            await _events.Add(credential, request.SourceId, image.FromEntityId, "ImageCreated", image.ToJson(), message.CreatedDateTime.Value.ToUnixTimeMilliseconds());
                            _store.Add(request.SourceId, image);
                            ReplaceMessageContent(message, image);
                        }
                        else
                            ReplaceMessageContent(message, image);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Could not download image");
                    }
                }
            }
            await _rest.SaveResponse(credential, request.SourceId, null, TeamsRequestTypes.ChatMessages, new TeamsMessageData { Id = chat.Id, Topic = chat.Topic }, messages.ToJson());
        }

        private HostedContentId ExtractUrlFromHostedContent(string existingUrl, string hostedContentIds)
        {
            var decoded = Base64Decode(hostedContentIds);
            var split = decoded.Split(',');
            var id = split[0].Split('=')[1];
            var type = split[1].Split('=')[1];
            var url = split[2].Split('=')[1];
            return new HostedContentId
            {
                Type = int.Parse(type),
                Url = url,
                Id = id,
            };
        }

        public string Base64Decode(string base64Encoded)
        {
            byte[] data = System.Convert.FromBase64String(base64Encoded);
            return System.Text.ASCIIEncoding.ASCII.GetString(data);
        }

        private static void ReplaceMessageContent(ChatMessage message, Image image)
        {
            message.Body.Content = message.Body.Content.Replace(image.Url, ImageProcessor.ImagePath(image));
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