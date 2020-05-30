using Microsoft.Extensions.DependencyInjection;
using McMaster.Extensions.CommandLineUtils;
using Console;
using Events;
using MediatR;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

namespace Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await CreateHostBuilder().RunCommandLineApplicationAsync(args, (app) => {
                new RegisterCommands(typeof(ChangePasswordCommand).Assembly).Apply(app);
                if (args.Length == 0)
                    app.ShowHelp();
            });
        }

        public static IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    var connectionString = hostContext.Configuration.GetConnectionString("EventsConnection");
                    services
                        .AddSingleton<KeyRepository>()
                        .AddMediatR(typeof(ChangePasswordCommand).Assembly)
                        .AddSingleton<DatabaseMigrator>()
                        .AddSingleton<NpgsqlConnectionFactory>((services) => {
                            return new NpgsqlConnectionFactory(connectionString);
                        });
                });
        }
    }
}