using System.Threading.Tasks;
using DbUp;
using Microsoft.Extensions.Logging;

namespace Events
{
    public class DatabaseMigrator
    {
        readonly string _connectionString;
        readonly ILogger _logger;

        public DatabaseMigrator(ILogger logger, string connectionString)
        {
            _logger = logger;
            _connectionString = connectionString;
        }
        
        public async Task Migrate()
        {
            EnsureDatabase.For.PostgresqlDatabase(_connectionString);

            var upgrader = DeployChanges.To.PostgresqlDatabase(_connectionString)
                .WithScriptsEmbeddedInAssembly(this.GetType().Assembly)
                .LogToConsole()
                .Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                _logger.LogInformation("Database upgrade successful");
            } else {
                _logger.LogError("Database upgrade unsuccessful");
            }
            
            await Task.Delay(0);
        }
    }
}