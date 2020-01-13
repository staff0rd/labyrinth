using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Npgsql;

namespace Events
{
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

        public async Task<Paginated<TEntity>> Paginate(Network network, int page = 0, int pageSize = 200, string orderBy = "inserted_at ASC")
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
                    Rows = await connection.QueryAsync<TEntity>($"SELECT * FROM {TableName} WHERE network={(int)network} ORDER BY {orderBy} LIMIT {limit} OFFSET {offset}"),
                };
            }
        }
    }
}