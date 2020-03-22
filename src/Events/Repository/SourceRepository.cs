using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Events
{
    public class SourceRepository : NpgsqlRepository<Source, Guid>
    {
        private ILogger<SourceRepository> _logger;

        public SourceRepository(NpgsqlConnectionFactory connectionFactory, ILogger<SourceRepository> logger) : base(connectionFactory)
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            _logger = logger;
        }

        private class SourceRow
        {
            public Guid Id { get; set; }
            public string Body { get; set;}
        }

        public async Task<List<Source>> Get(Credential creds)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var query = $@"
                
                SELECT
                    id,
                    pgp_sym_decrypt(body, pgp_sym_decrypt(keys.key, '{creds.Password}')) as body
                FROM
                    {TableName(creds.Username)}
                JOIN
                    public.keys ON keys.name = '{creds.Username}'
                ";

                var sources = new List<Source>();
                var rows = await connection.QueryAsync<SourceRow>(query);
                foreach (var row in rows)
                {
                    sources.Add(JsonConvert.DeserializeObject<Source>(row.Body));
                }                
                return sources;
            }
        }

        public async Task<Source> Add(Credential creds, Guid id, Network network, string name)
        {
            var source = new Source
            { 
                Id = id,
                Name = name,
                Network = network,
            };

            using (var connection = _connectionFactory.CreateConnection()) 
            {
                var query = $@"
                
                INSERT INTO
                    {TableName(creds.Username)} (id, body)
                SELECT
                    '{source.Id}',
                    pgp_sym_encrypt(@json, pgp_sym_decrypt(keys.key, '{creds.Password}'))
                FROM
                    public.keys
                WHERE
                    keys.name = '{creds.Username}'
                ";

                var parameters = new DynamicParameters();
                var json = source.ToJson();
                parameters.Add("json", json);
                await connection.ExecuteAsync(query, parameters);
            }

            return source;
        }
    }
}