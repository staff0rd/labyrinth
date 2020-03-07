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

        public async Task<List<DateTime>> GetLastUpdated(string userName, string password, Network network, string eventName, string category, int limit)
        {
            using (var connection = _connectionFactory.CreateConnection()) 
            {
                var result = await connection.QueryAsync<DateTime>($@"

                SELECT
                    inserted_at
                FROM
                    {TableName(userName)}
                JOIN
                    public.keys ON keys.name = '{userName}'
                WHERE
                    network={(int)network}
                AND
                    event_name='{eventName}'
                AND
                    cast(pgp_sym_decrypt(body, pgp_sym_decrypt(keys.key, '{password}')) as jsonb) ->> 'category' = '{category}'
                ORDER BY
                    inserted_at DESC LIMIT {limit}
                    
                ");

                return result.ToList();
            }
        }

        public async Task Add(string userName, string password, Network network, string entityId, string eventName, string body)
        {
            var ev = body.ToEvent(network, entityId, eventName);

            using (var connection = _connectionFactory.CreateConnection()) 
            {
                var query = $@"
                
                INSERT INTO
                    {TableName(userName)} (network, entity_id, event_name, body)
                SELECT
                    {(int)network},
                    '{entityId}',
                    '{eventName}',
                    pgp_sym_encrypt(@json, pgp_sym_decrypt(keys.key, '{password}'))
                FROM
                    public.keys
                WHERE
                    keys.name = '{userName}'
                ";

                var parameters = new DynamicParameters();
                parameters.Add("json", body);
                await connection.ExecuteAsync(query, parameters);
            }
        }

        public async Task ReadForward(string userName, string password, Network network, Func<Event[], Task<int>> eventProcessor)
        {
            Event[] currentSlice;
            int lastId = 0;
            const int PAGE_SIZE = 200;
            do
            {
                var result = await Paginate(userName, password, network, lastId, PAGE_SIZE);
                currentSlice = result.Rows.ToArray();


                lastId = await eventProcessor(currentSlice);
                    
            } while (currentSlice.Length == PAGE_SIZE);
        }

        private string GetEventFilter(string[] eventTypes)
        {
            if (eventTypes == null || !eventTypes.Any())
                return "";

            var events = string.Join(",",eventTypes.Select(e => $"'{e}'"));
            return $"AND event_name IN ({events})";
        }

        public async Task<Paginated<Event>> Paginate(string userName, string password, Network network, int lastId = 0, int pageSize = 200, string orderBy = "id ASC",
        string[] eventTypes = null)
        {
            var limit = pageSize;

            using (var connection = _connectionFactory.CreateConnection())
            {
                var rowsQuery = $@"
                
                SELECT
                    id,
                    entity_id,
                    network,
                    event_name,
                    pgp_sym_decrypt(body, pgp_sym_decrypt(keys.key, '{password}')) as body,
                    inserted_at
                FROM
                    {TableName(userName)}
                JOIN
                    public.keys ON keys.name = '{userName}'
                WHERE
                    network={(int)network}
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

        public int GetCount(string userName, Network network, string eventType)
        {
            return GetCount(userName, network, new [] { eventType });
        }

        public int GetCount(string userName, Network network, string[] eventTypes)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var query = $"SELECT COUNT(*) FROM {TableName(userName)} WHERE network={(int)network} {GetEventFilter(eventTypes)}";
                return connection.ExecuteScalar<int>(query);
            }
        }

        public Task Sync<T>(string userName, string password, Network network, T payload, T existing, ILogger logger) where T: IEntity<string> 
        {
            return Sync(userName, password, network, payload, existing, logger, new string[] {});
        }

        public async Task Sync<T>(string userName, string password, Network network, T payload, T existing, ILogger logger, IEnumerable<string> ignoreNulls) where T: IEntity<string> 
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
                        await Add(userName, password, network, payload.Id, eventName, payload.ToJson());
                        logger.LogInformation("Raised {eventName} in {network}", eventName, network);
                    }
                }
            }
            else
            {
                var eventName = $"{payload.GetType().Name}Created";
                await Add(userName, password, network, payload.Id, eventName, payload.ToJson());
                logger.LogInformation("Raised {eventName} in {network}", eventName, network);
            }
        }
    }
}