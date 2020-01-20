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
        private readonly IDictionary<string, RateLimit> _limits;
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        private readonly string _connectionString;
        private readonly EventRepository _events;
        private readonly string _schema;

        public async Task Migrate()
        {
            EnsureDatabase.For.PostgresqlDatabase(_connectionString);

            var upgrader = DeployChanges.To.PostgresqlDatabase(_connectionString)
                .WithScriptsEmbeddedInAssembly(this.GetType().Assembly)
                .LogToConsole()
                .Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                _logger.LogInformation("Database upgrade successful");
            } else {
                _logger.LogError("Database upgrade unsuccessful");
            }
            
            await Task.Delay(0);
        }

        public RestEventManager(Microsoft.Extensions.Logging.ILogger logger, string connectionString, string schema)
        {
            _logger = logger;
            _connectionString = connectionString;
            _schema = schema;
            _events = new EventRepository(_connectionString, _schema);
        }

        public RestEventManager(Microsoft.Extensions.Logging.ILogger logger, string connectionString, string schema, ReadOnlyDictionary<string, RateLimit> limits) : this(logger, connectionString, schema)
        {
            _limits = limits;
        }

        public async Task<T> Get<T>(Network network, Request<T> request, object queryString, string token)
        {
            var oldest = await _events.GetLastUpdated(network, "RestApiRequest", request.Category, request.RateLimit.RequestCount);
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
                var url = request.Endpoint
                    .WithOAuthBearerToken(token);
                
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
                await _events.Add(network, Guid.NewGuid().ToString(), "RestApiRequest", new RestApiRequest
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