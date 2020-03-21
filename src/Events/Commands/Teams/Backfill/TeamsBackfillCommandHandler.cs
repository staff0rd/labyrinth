using System.Net;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;

namespace Events
{
    public class TeamsBackfillCommandHandler : IRequestHandler<TeamsBackfillCommand>
    {
        private readonly RestEventManager _rest;
        private readonly ILogger<TeamsBackfillCommandHandler> _logger;
        private readonly CredentialCache _credentials;

        private readonly IProgress _progress;

        public TeamsBackfillCommandHandler(ILogger<TeamsBackfillCommandHandler> logger, RestEventManager rest,
            CredentialCache credentials, IProgress progress) {
            _rest = rest;
            _logger = logger;
            _credentials = credentials;
            _progress = progress;
        }

        public async Task<Unit> Handle(TeamsBackfillCommand request, CancellationToken cancellationToken)
        {
            var credential = _credentials.Get(Network.Teams, request.LabyrinthUsername);
            var client = GetGraphClient(credential.ExternalSecret);
            var chats = await client
                .Me
                .Chats
                .Request()
                .GetAsync();
            
            await _rest.SaveResponse(credential.Username, credential.Password, Network.Teams, null, "Chats", null, chats.ToJson());
            
            int i = 0;
            foreach (var chat in chats)
            {
                i++;
                _progress.Set(i, chats.Count);
                try {
                    var messages = await client.Me.Chats[chat.Id].Messages.Request(new [] { new QueryOption("$top", "50")}).GetAsync();
                    do 
                    {   
                        await _rest.SaveResponse(credential.Username, credential.Password, Network.Teams, null, "Chat-Messages", new { id=chat.Id, topic=chat.Topic }, messages.ToJson());
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