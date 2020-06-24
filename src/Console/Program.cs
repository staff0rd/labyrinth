using Microsoft.Extensions.DependencyInjection;
using McMaster.Extensions.CommandLineUtils;
using Events;
using MediatR;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.ComponentModel.DataAnnotations;

namespace Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try {
                await CreateHostBuilder().RunCommandLineApplicationAsync(args, (app) => {
                    if (args.Length == 0)
                        app.ShowHelp();
                    app.Command<SourceCommand>(null, null);
                    app.Command<AccountCommand>(null, null);
                    app.Command<MigrateDatabaseCommand>(null, null);
                    app.Command<BackfillCommand>(null, null);
                    app.Command<ProcessCommand>(null, null);
                    app.Command<TeamsCommand>(null, null);
                });
            } catch (Exception e)
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine(e.Message);
                System.Console.ResetColor();
            }
        }

        [Required]
        [Option("-u|--username", CommandOptionType.SingleValue, Description = "User name")]
        public string Username { get; set; }
        
        [Required]
        [Option("-p|--password", CommandOptionType.SingleValue, Description = "Password")]
        public string Password { get; set; }

        public static IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    var connectionString = hostContext.Configuration.GetConnectionString("EventsConnection");
                    services
                        .AddSingleton<KeyRepository>()
                        .AddSingleton<EventRepository>()
                        .AddSingleton<RestEventManager>()
                        .AddTransient<IProgress, ConsoleProgress>()
                        .AddSingleton<SourceRepository>()
                        .AddMediatR(typeof(ChangePasswordCommand).Assembly)
                        .AddSingleton<DatabaseMigrator>()
                        .AddSingleton<NpgsqlConnectionFactory>((services) => {
                            return new NpgsqlConnectionFactory(connectionString);
                        })
                        .AddSingleton<Store>();
                });
        }
    }
}