using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Flurl.Http;
using JsonDiffPatchDotNet;
using Rest;
using System.Diagnostics;
using Npgsql;
using DbUp;
using System.Reflection;
using System.Linq;
using Dapper;

namespace Events
{
    public class RestEventManager
    {
        private readonly ILogger<RestEventManager> _logger;
        private readonly EventRepository _events;

        public RestEventManager(ILogger<RestEventManager> logger, EventRepository events)
        {
            _logger = logger;
            _events = events;
        }

        public async Task<T> Get<T>(string userName, string password, Network network, Request<T> request, object queryString, string token)
        {
            var oldest = await _events.GetLastUpdated(userName, password, network, "RestApiRequest", request.Category, request.RateLimit.RequestCount);
            if (oldest.Count() == request.RateLimit.RequestCount)
            {
                var waitUntil = oldest.Last().Add(request.RateLimit.Duration);
                if (waitUntil > DateTime.UtcNow) {
                    var delay = waitUntil.Subtract(DateTime.UtcNow);
                    _logger.LogWarning("Waiting {delay} for [{category}] {endpoint}", delay, request.Category, request.Endpoint);
                    await Task.Delay(delay);
                }
            }
            string response = null;

            try
            {
                var url = request.Endpoint.WithOAuthBearerToken(token);
                
                if (queryString != null) {
                    url.SetQueryParams(queryString);
                }
                    
                response = await url.GetStringAsync();

                return request.Transform(response);
            }
            catch (FlurlHttpException ex)
            {
                _logger.LogError(default(EventId), ex, ex.Message);
                return default(T);
            }
            finally {
                await _events.Add(userName, password, network, Guid.NewGuid().ToString(), "RestApiRequest", new RestApiRequest
                {
                    Category = request.Category,
                    Data = queryString.ToJson(),
                    Method = "GET",
                    Response =  response,
                }.ToJson());
            }
        }
    }
}