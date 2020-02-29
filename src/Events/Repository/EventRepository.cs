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
        readonly string _password;
        public EventRepository(string connectionString, string schema, string password) : base(connectionString, schema)
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            _password = password;
        }

        public async Task<List<DateTime>> GetLastUpdated(Network network, string eventName, string category, int limit)
        {
            using (var connection = new NpgsqlConnection(_connectionString)) 
            {
                var result = await connection.QueryAsync<DateTime>($@"

                SELECT
                    inserted_at
                FROM
                    {TableName}
                JOIN
                    public.keys ON keys.name = '{_userName}'
                WHERE
                    network={(int)network}
                AND
                    event_name='{eventName}'
                AND
                    cast(pgp_sym_decrypt(body, pgp_sym_decrypt(keys.key, '{_password}')) as jsonb) ->> 'category' = '{category}'
                ORDER BY
                    inserted_at DESC LIMIT {limit}
                    
                ");

                return result.ToList();
            }
        }

        public async Task Add(Network network, string entityId, string eventName, string body)
        {
            var ev = body.ToEvent(network, entityId, eventName);

            using (var connection = new NpgsqlConnection(_connectionString)) 
            {
                var query = $@"
                
                INSERT INTO
                    {TableName} (network, entity_id, event_name, body)
                SELECT
                    {(int)network},
                    '{entityId}',
                    '{eventName}',
                    pgp_sym_encrypt(@json, pgp_sym_decrypt(keys.key, '{_password}'))
                FROM
                    public.keys
                WHERE
                    keys.name = '{_userName}'
                ";

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

        public async Task<Paginated<Event>> Paginate(Network network, int page = 0, int pageSize = 200, string orderBy = "inserted_at ASC")
        {
            var offset = pageSize * page;
            var limit = pageSize;
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var rowsQuery = $@"
                
                SELECT
                    id,
                    entity_id,
                    network,
                    event_name,
                    pgp_sym_decrypt(body, pgp_sym_decrypt(keys.key, '{_password}')) as body,
                    inserted_at
                FROM
                    {TableName}
                JOIN
                    public.keys ON keys.name = '{_userName}'
                WHERE
                    network={(int)network}
                ORDER BY
                    {orderBy}
                LIMIT
                    {limit}
                OFFSET
                    {offset}
                    
                ";
                var totalRowsQuery = $"SELECT COUNT(*) FROM {TableName} WHERE network={(int)network}";
                return new Paginated<Event>
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalRows = await connection.ExecuteAsync(totalRowsQuery),
                    Rows = await connection.QueryAsync<Event>(rowsQuery),
                };
            }
        }

        public Task Sync<T>(Network network, T payload, T existing, ILogger logger) where T: IEntity<string> 
        {
            return Sync(network, payload, existing, logger, new string[] {});
        }

        public async Task Sync<T>(Network network, T payload, T existing, ILogger logger, IEnumerable<string> ignoreNulls) where T: IEntity<string> 
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