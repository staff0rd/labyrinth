using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Npgsql;

namespace Events
{
    public class NpgsqlExternalEntityRepository<TEntity> : NpgsqlRepository<TEntity, Guid>, IExternalEntityRepository<TEntity> where TEntity : class, IExternalEntity
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

    public class NpgsqlRepository<TEntity, TId> : IRepository<TEntity, TId> where TEntity : class, IEntity<TId>
    {
        protected readonly string _connectionString;
        protected readonly string _schema;

        protected string TableName => $"{_schema}.{typeof(TEntity).Name.ToLower()}s";

        public NpgsqlRepository(string connectionString, string schema)
        {
            _connectionString = connectionString;
            _schema = schema;
        }

        public async Task<TEntity> GetById(Guid id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var result = await connection.QueryAsync<TEntity>($"SELECT * FROM {TableName} WHERE id={id}");
                return result.FirstOrDefault();
            }
        }

        public async Task<Paginated<TEntity>> Paginate(Network network, int page = 0, int pageSize = 200)
        {
            var offset = pageSize * page;
            var limit = pageSize;
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                return new Paginated<TEntity>
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalRows = await connection.ExecuteAsync($"SELECT COUNT(*) FROM {TableName} WHERE network={(int)network}"),
                    Rows = await connection.QueryAsync<TEntity>($"SELECT * FROM {TableName} WHERE network={(int)network} ORDER BY inserted_at DESC LIMIT {limit} OFFSET {offset}"),
                };
            }
        }
    }
}