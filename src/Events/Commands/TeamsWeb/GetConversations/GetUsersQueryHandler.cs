using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;
using Flurl.Http.Configuration;
using MediatR;

namespace Events.TeamsWeb
{
    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, Result<FetchShortProfile>>
    {
        const string FETCH_USERS_URL = "https://teams.microsoft.com/api/mt/apac/beta/users/fetchShortProfile?enableGuest=true";
        public async Task<Result<FetchShortProfile>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var userIds = string.Join(",",request.UserIds.Select(p => $"\"{p}\""));
            var payload = new StringContent($"[{userIds}]");

            var response = await new FlurlRequest(FETCH_USERS_URL)
                .WithOAuthBearerToken(request.Token)
                .WithHeader("Content-Type", "application/json;charset=UTF-8")
                .PostAsync(payload)
                    
                    //"[\"8:orgid:19e29285-1fcb-44bf-883c-39338b5dcff9\"]"))
                .ReceiveJson<FetchShortProfile>();

            return new Result<FetchShortProfile>(response);
        }
    }
}