using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using JsonDiffPatchDotNet;
using KellermanSoftware.CompareNetObjects;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Events
{
    public class EventRepository : NpgsqlRepository<Event, int>
    {
        private ILogger<EventRepository> _logger;

        public EventRepository(NpgsqlConnectionFactory connectionFactory, ILogger<EventRepository> logger) : base(connectionFactory)
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            _logger = logger;
        }

        public async Task<List<DateTime>> GetLastInserted(Credential creds, Guid sourceId, string eventName, string category, int limit)
        {
            using (var connection = _connectionFactory.CreateConnection()) 
            {
                var result = await connection.QueryAsync<DateTime>($@"

                SELECT
                    inserted_at
                FROM
                    {TableName(creds.Username)}
                JOIN
                    public.keys ON keys.name = '{creds.Username}'
                WHERE
                    source_id='{sourceId}'
                AND
                    event_name='{eventName}'
                AND
                    cast(pgp_sym_decrypt(body, pgp_sym_decrypt(keys.key, '{creds.Password}')) as jsonb) ->> 'category' = '{category}'
                ORDER BY
                    inserted_at DESC
                LIMIT
                    {limit}    
                ");

                return result.ToList();
            }
        }

        internal async Task<EventCount[]> GetEventTypes(string username, Guid sourceId)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var query = $"select count(*), event_name FROM {TableName(username)} WHERE source_id='{sourceId}' group by event_name";
                var result = await connection.QueryAsync<EventCount>(query);
                return result.ToArray();
            }
        }

        internal async Task Delete(string userName, string password, Guid sourceId, string[] events)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var query = $"SELECT COUNT(*) FROM {TableName(userName)} WHERE source_id='{sourceId}' {GetEventFilter(events)}";
                await connection.ExecuteAsync(query);
            }
        }

        public async Task Add(Credential creds, Guid sourceId, string entityId, string eventName, string body)
        {
            var ev = body.ToEvent(sourceId, entityId, eventName);

            using (var connection = _connectionFactory.CreateConnection()) 
            {
                var query = $@"
                
                INSERT INTO
                    {TableName(creds.Username)} (source_id, entity_id, event_name, timestamp, body)
                SELECT
                    '{sourceId}',
                    '{entityId}',
                    '{eventName}',
                    {DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()},
                    pgp_sym_encrypt(@json, pgp_sym_decrypt(keys.key, '{creds.Password}'))
                FROM
                    public.keys
                WHERE
                    keys.name = '{creds.Username}'
                ";

                var parameters = new DynamicParameters();
                parameters.Add("json", body);
                await connection.ExecuteAsync(query, parameters);
            }
        }

        public async Task ReadForward(Credential credential, Guid sourceId, int totalEvents, Func<Event[], int, Task<int>> eventProcessor)
        {
            Event[] currentSlice;
            int lastId = 0;
            const int PAGE_SIZE = 200;
            do
            {
                var result = await Paginate(credential, sourceId, lastId, PAGE_SIZE);
                currentSlice = result.Rows.ToArray();


                lastId = await eventProcessor(currentSlice, totalEvents);
                    
            } while (currentSlice.Length == PAGE_SIZE);
        }

        private string GetEventFilter(string[] eventTypes)
        {
            if (eventTypes == null || !eventTypes.Any())
                return "";

            var events = string.Join(",",eventTypes.Select(e => $"'{e}'"));
            return $"AND event_name IN ({events})";
        }

        public async Task<Paginated<Event>> Paginate(Credential credential, Guid sourceId, int lastId = 0, int pageSize = 200, string orderBy = "id ASC",
        string[] eventTypes = null)
        {
            var limit = pageSize;
            var userName = credential.Username;
            var password = credential.Password;

            using (var connection = _connectionFactory.CreateConnection())
            {
                var rowsQuery = $@"
                
                SELECT
                    id,
                    entity_id,
                    source_id,
                    event_name,
                    pgp_sym_decrypt(body, pgp_sym_decrypt(keys.key, '{password}')) as body,
                    inserted_at,
                    timestamp
                FROM
                    {TableName(userName)}
                JOIN
                    public.keys ON keys.name = '{userName}'
                WHERE
                    source_id='{sourceId}'
                AND
                    id > {lastId}
                {GetEventFilter(eventTypes)}
                ORDER BY
                    {orderBy}
                LIMIT
                    {limit}
                ";
                return new Paginated<Event>
                {
                    PageSize = pageSize,
                    Rows = await connection.QueryAsync<Event>(rowsQuery),
                };
            }
        }

        public Task<int> GetCount(string userName, Guid sourceId, string eventType)
        {
            return GetCount(userName, sourceId, new [] { eventType });
        }

        public async Task<int> GetCount(string userName, Guid sourceId, string[] eventTypes)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var query = $"SELECT COUNT(*) FROM {TableName(userName)} WHERE source_id='{sourceId}' {GetEventFilter(eventTypes)}";
                var result = await connection.ExecuteScalarAsync<int>(query);
                return result;
            }
        }

        public Task Sync<T>(Credential creds, Guid sourceId, T payload, T existing) where T: IEntity<string> 
        {
            return Sync(creds, sourceId, payload, existing, new string[] {});
        }

        public async Task Sync<T>(Credential creds, Guid sourceId, T payload, T existing, IEnumerable<string> ignoreNulls) where T: IEntity<string> 
        {
            if (existing != null)
            {
                CompareLogic compareLogic = new CompareLogic();
                compareLogic.Config.MaxDifferences = 20;
                var result = compareLogic.Compare(existing, payload);
                if (!result.AreEqual)
                {
                    var importantDifferences = result.Differences.Count(p => !ignoreNulls.Contains(p.PropertyName) || p.Object2 != null);
                    if (importantDifferences > 0) {
                        var eventName = $"{payload.GetType().Name}Updated";
                        await Add(creds, sourceId, payload.Id, eventName, payload.ToJson());
                        _logger.LogInformation("Raised {eventName} in {sourceId}", eventName, sourceId);
                    }
                }
            }
            else
            {
                var eventName = $"{payload.GetType().Name}Created";
                await Add(creds, sourceId, payload.Id, eventName, payload.ToJson());
                _logger.LogInformation("Raised {eventName} in {sourceId}", eventName, sourceId);
            }
        }
    }
}