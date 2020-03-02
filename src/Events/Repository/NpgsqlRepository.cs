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
        protected readonly NpgsqlConnectionFactory _connectionFactory;

        protected string TableName(string userName) => $"user_{userName}.{typeof(TEntity).Name.ToLower()}s";

        public NpgsqlRepository(NpgsqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<TEntity> GetById(string userName, string id)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var result = await connection.QueryAsync<TEntity>($"SELECT * FROM {TableName(userName)} WHERE id={id}");
                return result.FirstOrDefault();
            }
        }
    }
}