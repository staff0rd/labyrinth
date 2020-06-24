using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;
using Flurl.Http.Configuration;
using MediatR;

namespace Events.TeamsWeb
{
    public class GetOverviewQueryHandler : IRequestHandler<GetOverviewQuery, Result<Overview>>
    {
        const string OVERVIEW_URL = "https://teams.microsoft.com/api/csa/api/v1/teams/users/me?isPrefetch=false&enableMembershipSummary=true";
        public async Task<Result<Overview>> Handle(GetOverviewQuery request, CancellationToken cancellationToken)
        {
            FlurlHttp.Configure(settings => settings.JsonSerializer = new NewtonsoftJsonSerializer(Converter.Settings));
            var response = await new FlurlRequest(OVERVIEW_URL)
                .WithOAuthBearerToken(request.Token)
                .GetAsync()
                .ReceiveJson<Overview>();

            return new Result<Overview>(response);
        }
    }
}