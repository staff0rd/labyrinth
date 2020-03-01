using System.Data.Common;
using Npgsql;

namespace Events {
    public class NpgsqlConnectionFactory
    {
        private readonly string _connectionString;

        public NpgsqlConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }
        public DbConnection CreateConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }
    }
}