using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Npgsql;

namespace Events
{
    public class NpgsqlExternalEntityRepository<TEntity> : NpgsqlRepository<TEntity, string>, IExternalEntityRepository<TEntity> where TEntity : class, IExternalEntity
    {
        public NpgsqlExternalEntityRepository(string connectionString, string schema) : base(connectionString, schema) {}
        public async Task<TEntity> GetByExternalId(Network network, string externalId)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var result = await connection.QueryAsync<TEntity>($"SELECT * FROM {TableName} WHERE network={(int)network} and external_id='{externalId}'");
                return result.FirstOrDefault();
            }
        }
    }
}