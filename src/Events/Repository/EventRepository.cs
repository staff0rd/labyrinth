using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using JsonDiffPatchDotNet;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Events
{
    public class EventRepository : NpgsqlRepository<Event, int>
    {
        public EventRepository(string connectionString, string schema) : base(connectionString, schema)
        {
        }

        public async Task<List<DateTime>> GetLastUpdated(Network network, string eventName, string category, int limit)
        {
            using (var connection = new NpgsqlConnection(_connectionString)) 
            {
                var result = await connection.QueryAsync<DateTime>($"SELECT inserted_at FROM {TableName} WHERE network={(int)network} AND event_name='{eventName}' AND body ->> 'category' = '{category}' ORDER BY inserted_at DESC LIMIT {limit}");
                return result.ToList();
            }
        }

        public async Task Add(Network network, Guid entityId, string eventName, string body)
        {
            var ev = body.ToEvent(network, entityId, eventName);

            using (var connection = new NpgsqlConnection(_connectionString)) 
            {
                var query = $"INSERT INTO {_schema}.events (network, entity_id, event_name, body) VALUES ({(int)network}, '{entityId}', '{eventName}', CAST(@json as json))";
                var parameters = new DynamicParameters();
                parameters.Add("json", body);
                await connection.ExecuteAsync(query, parameters);
            }
        }

        public async Task ReadForward(Network network, Func<Event[], Task> eventProcessor)
        {
            Event[] currentSlice;
            int nextPage = 0;
            const int PAGE_SIZE = 200;
            do
            {
                var result = await Paginate(network, nextPage, PAGE_SIZE);
                currentSlice = result.Rows.ToArray();

                nextPage++;

                await eventProcessor(currentSlice);
                    
            } while (currentSlice.Length == PAGE_SIZE);
        }

        public async Task Sync<T>(Network network, T payload, T existing, ILogger logger, bool update = true) where T: IEntity<Guid> 
        {
            if (existing != null)
            {
                if (update) {
                    var jdp = new JsonDiffPatch();
                    var output = jdp.Diff(existing.ToJson(), payload.ToJson());
                    if (output != null)
                    {
                        var eventName = $"{payload.GetType().Name}Updated";
                        await Add(network, payload.Id, eventName, payload.ToJson());
                        logger.LogInformation("Raised {eventName} in {network}", eventName, network);
                    }
                }
            }
            else
            {
                var eventName = $"{payload.GetType().Name}Created";
                await Add(network, payload.Id, eventName, payload.ToJson());
                logger.LogInformation("Raised {eventName} in {network}", eventName, network);
            }
        }
    }
}