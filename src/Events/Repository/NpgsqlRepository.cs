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
        protected readonly string _userName;

        protected string TableName => $"user_{_userName}.{typeof(TEntity).Name.ToLower()}s";

        public NpgsqlRepository(string connectionString, string userName)
        {
            _connectionString = connectionString;
            _userName = userName;
        }

        public async Task<TEntity> GetById(string id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var result = await connection.QueryAsync<TEntity>($"SELECT * FROM {TableName} WHERE id={id}");
                return result.FirstOrDefault();
            }
        }
    }
}