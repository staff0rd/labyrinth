using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;
using Flurl.Http.Configuration;
using MediatR;

namespace Events.TeamsWeb
{
    public class GetChatsQueryHandler : IRequestHandler<GetChatsQuery, Result<ChatOverview[]>>
    {
        private readonly IMediator _mediator;

        public GetChatsQueryHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        const string OVERVIEW_URL = "https://teams.microsoft.com/api/csa/api/v1/teams/users/me?isPrefetch=false&enableMembershipSummary=true";
        public async Task<Result<ChatOverview[]>> Handle(GetChatsQuery request, CancellationToken cancellationToken)
        {
            FlurlHttp.Configure(settings => settings.JsonSerializer = new NewtonsoftJsonSerializer(Converter.Settings));
            var response = await new FlurlRequest(OVERVIEW_URL)
                .WithOAuthBearerToken(request.Token)
                .GetAsync()
                .ReceiveJson<Overview>();

            var chats = response
                .Chats
                .Where(p => p.ChatType == ChatType.Chat);

            var members = chats
                .SelectMany(p => p.Members)
                .Select(p => p.Mri)
                .Distinct()
                .ToArray();

            var me = chats.SelectMany(p => p.Members)
                .GroupBy(g => g.Mri)
                .OrderByDescending(g => g.Count())
                .First().Key;

            var users = await _mediator.Send(new GetUsersQuery { Token = request.Token, UserIds = members });
            
            var conversations = chats
                .OrderByDescending(p => p.LastMessage.ComposeTime)
                .Select(p => new ChatOverview {
                    Author = GetAuthor(p, me, users),
                    Time = p.LastMessage?.ComposeTime?.ToLocalTime(),
                    LastMessage = Truncate(p.LastMessage?.Content?.Replace("\n", "").Replace("\r", ""))
                })
                .ToArray();

            return new Result<ChatOverview[]>(conversations);
        }

        private string GetAuthor(Chat chat, string me, Result<FetchShortProfile> users)
        {
            if (!string.IsNullOrEmpty(chat.Title))
                return chat.Title;
            
            var notMe = chat.Members
                .Where(p => p.Mri != me)
                .Select(p => {
                    var user = users.Response.Value.First(u => u.Mri == p.Mri);
                    return new {
                        user.GivenName,
                        user.DisplayName
                    };
                });
                

            if (notMe.Count() == 1)
                return notMe.First().DisplayName;
            
            else return string.Join(" & ", notMe.Select(p => p.GivenName));
        }

        const int TRUNCATE = 50;
        

        private string Truncate(string value)
        {
            if (value != null && value.Length > TRUNCATE)
                return $"{value.Substring(0, TRUNCATE)}…";
            return value;
        }
    }
}