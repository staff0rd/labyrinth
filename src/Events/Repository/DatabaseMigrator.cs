using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DbUp;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Events
{
    public class DatabaseMigrator
    {
        readonly string _connectionString;
        readonly ILogger<DatabaseMigrator> _logger;

        public DatabaseMigrator(ILogger<DatabaseMigrator> logger, string connectionString)
        {
            _logger = logger;
            _connectionString = connectionString;
        }
        
        public async Task Migrate()
        {
            EnsureDatabase.For.PostgresqlDatabase(_connectionString);

            using (var connection = new NpgsqlConnection(_connectionString)) {
                new [] { "hangfire", "labyrinth" }
                    .ToList()
                    .ForEach(db => {
                        var exists = connection.ExecuteScalar<bool>($"SELECT true FROM pg_database WHERE datname = '{db}'");
                        if (!exists) {
                            connection.Execute($"CREATE DATABASE {db}");
                        }
                    });
            }
            

            var upgrader = DeployChanges.To.PostgresqlDatabase(_connectionString)
                .WithScriptsEmbeddedInAssembly(this.GetType().Assembly)
                .LogToConsole()
                .Build();

            var result = upgrader.PerformUpgrade();

            if (result.Successful)
            {
                _logger.LogInformation("Database upgrade successful");
            } else {
                _logger.LogError("Database upgrade unsuccessful");
            }
            
            await Task.Delay(0);
        }
    }
}