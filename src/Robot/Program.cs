using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using EventStore.ClientAPI.Exceptions;
using McMaster.Extensions.CommandLineUtils;
using McMaster.Extensions.CommandLineUtils.Validation;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Microsoft.Extensions.Logging;

namespace Robot
{
    class Program
    {
        static int Main(string[] args)
        {
            var config = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.Trace();
            
            Log.Logger = config.CreateLogger();
            
            var logger = new SerilogLoggerProvider(Log.Logger).CreateLogger(nameof(Program));
            
            var app = new CommandLineApplication();

            app.HelpOption();

            app.Command("yammer", yammer => {
                var token = yammer
                    .Option("-t|--token <TOKEN>", "Yammer authorization token", CommandOptionType.SingleValue)
                    .IsRequired();
                
                yammer.OnExecuteAsync(async (cancel) => {
                    await Automate(logger, () => new YammerAutomation(logger, token.Value()).Automate());
                });
            });

            app.Command("linkedin", linkedIn => {
                var username = linkedIn
                    .Option("-u|--username <USERNAME>", "LinkedIn username", CommandOptionType.SingleValue)
                    .IsRequired()
                    .Accepts(a => a.EmailAddress());
                var password = linkedIn
                    .Option("-p|--password <PASSWORD>", "LinkedIn password", CommandOptionType.SingleValue)
                    .IsRequired();
                linkedIn.OnExecuteAsync(async (cancel) => {
                    await Automate(logger, () => new Robot.LinkedInAutomation(logger, username.Value(), password.Value()).Automate());
                });
            });

            try
            {
                Log.Information("Welcome to Robot!");

                return app.Execute(args);
            } catch (Exception e)
            {
                Log.Fatal(e, "Host terminated unexpectedly");
                return 1;
            } finally
            {
                Log.CloseAndFlush();
            }
        }

        static async Task<int> Automate(Microsoft.Extensions.Logging.ILogger logger, Func<Task> work) {
            try
                {
                    logger.LogInformation("Starting automation");

                    await work();
                    
                    logger.LogInformation("Automation complete");

                    return 0;
                }
                catch (Exception e)
                {
                    logger.LogError(e.Message);
                    return 1;
                }
        }
    }

    class MustBeUriValidator : IOptionValidator
    {
        public ValidationResult GetValidationResult(CommandOption option, ValidationContext context)
        {
            try
            {
                new Uri(option.Value());
                return ValidationResult.Success;
            } catch
            {
                return new ValidationResult($"The value for --{option.LongName} must be a valid URL");
            }
        }
    }
}
