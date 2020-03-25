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
using System.IO;

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

        public async Task DownloadImage(Credential creds, Guid sourceId, Image image, string token, string downloadPath)
        {
            if (!Directory.Exists(downloadPath))
                Directory.CreateDirectory(downloadPath);
                
            var url = image.Url.WithOAuthBearerToken(token);
            
            var response = await url.GetBytesAsync();

            File.WriteAllBytes(Path.Combine(downloadPath, image.Id.ToString()), response);
        }

        public async Task<T> Get<T>(Credential creds, Guid sourceId, Request<T> request, object queryString, string token)
        {
            var oldest = await _events.GetLastInserted(creds, sourceId, "RestApiRequest", request.Category, request.RateLimit.RequestCount);
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
                await SaveResponse(creds, sourceId, request.Endpoint, request.Category, queryString, response);
            }
        }
        public async Task SaveResponse(Credential creds, Guid sourceId, string url, string category, object queryString, string response)
        {
            await _events.Add(creds, sourceId, Guid.NewGuid().ToString(), "RestApiRequest", new RestApiRequest
            {
                Uri = url,
                Category = category,
                Data = queryString.ToJson(),
                Method = "GET",
                Response =  response,
            }.ToJson(), DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
        }
    }
}