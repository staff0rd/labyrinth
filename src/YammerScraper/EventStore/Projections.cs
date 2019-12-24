using System;
using System.Net;
using System.Threading.Tasks;
using EventStore.ClientAPI.Exceptions;
using EventStore.ClientAPI.Projections;
using EventStore.ClientAPI.SystemData;
using Microsoft.Extensions.Logging;

namespace YammerScraper
{
    public class Projections
    {
        public enum Exists {
            False,
            Outdated,
            True,
        }
        private readonly ProjectionsManager _manager;
        private readonly ILogger _logger;
        private readonly UserCredentials _credentials;

        public Projections(ILogger logger, UserCredentials credentials) {
            _logger = logger;
            _credentials = credentials;
            var endpoint = (EndPoint)new IPEndPoint(IPAddress.Loopback, 2113);
            _manager = new ProjectionsManager(new EventStoreLogger(logger), endpoint, TimeSpan.FromSeconds(5), "http");
        }

        public async Task CreateOrUpdate(string name, string query)
        {
            var exists = await ProjectionExists(name, query);

            switch (exists) {
                case Exists.False: {
                    await _manager.CreateContinuousAsync(name, query, _credentials);
                    _logger.LogInformation("Query '{name}' created", name);
                    break;
                }
                case Exists.Outdated: {
                    await _manager.UpdateQueryAsync(name, query, _credentials);
                    _logger.LogInformation("Query '{name}' updated", name);
                    break;
                }
                default: break;
            }
        }

        public async Task<Exists> ProjectionExists(string name, string query)
        {
            try
            {
                var existing = await _manager.GetQueryAsync(name, _credentials);
                if (existing == query) {
                    return Exists.True;
                }
                return Exists.Outdated;
            }
            catch (ProjectionCommandFailedException ex)
            {
                if (ex.HttpStatusCode == 404)
                    return Exists.False;
                else throw;
            }
        }

        internal async Task<string> GetState(string name, string category)
        {
            return await _manager.GetPartitionStateAsync(name, category, _credentials);
        }
    }
}