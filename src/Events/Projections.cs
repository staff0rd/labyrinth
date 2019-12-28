using System;
using System.Net;
using System.Threading.Tasks;
using EventStore.ClientAPI.Exceptions;
using EventStore.ClientAPI.Projections;
using EventStore.ClientAPI.SystemData;
using Microsoft.Extensions.Logging;

namespace Events
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

        public async Task CreateOrUpdate(string projectionName, string query)
        {
            var exists = await ProjectionExists(projectionName, query);

            switch (exists) {
                case Exists.False: {
                    await _manager.CreateContinuousAsync(projectionName, query, _credentials);
                    _logger.LogInformation("Query '{name}' created", projectionName);
                    break;
                }
                case Exists.Outdated: {
                    await _manager.UpdateQueryAsync(projectionName, query, _credentials);
                    _logger.LogInformation("Query '{name}' updated", projectionName);
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

        internal Task<string> GetProjection(string projectionName) {
            return _manager.GetStateAsync(projectionName, _credentials);
        } 

        internal Task<string> GetProjection(string projectionName, string partition)
        {
            return _manager.GetPartitionStateAsync(projectionName, partition, _credentials);
        }
    }
}