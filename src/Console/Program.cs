using System;
using Microsoft.Extensions.DependencyInjection;
using McMaster.Extensions.CommandLineUtils;
using System.Collections.Generic;
using System.Linq;
using Console;
using Events;
using MediatR;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

namespace CustomServices
{
   
    [HelpOption]
    class Program
    {
        static async Task Main(string[] args)
        {
            await CreateHostBuilder()
                .RunCommandLineApplicationAsync(args, (app) => {
                    new RegisterCommands(typeof(ChangePasswordCommand).Assembly).Apply(app);
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
                        .AddSingleton<NpgsqlConnectionFactory>((services) => {
                            return new NpgsqlConnectionFactory(connectionString);
                        });
                });
        }

        private void OnExecute()
        {
            System.Console.WriteLine("Nothing");
        }
    }
}