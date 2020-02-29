using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using McMaster.Extensions.CommandLineUtils.Validation;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Microsoft.Extensions.Logging;
using Events;
using Npgsql;
using Dapper;

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

            app.Command("debug", debug => {
                debug.OnExecuteAsync(async (cancel) => {
                    await Automate(logger, () => new Debugger(logger).Go());
                });
            });

            app.Command("user", user =>
            {
                user.HelpOption();
                var connectionString = user
                    .Option("-c|--connection-string <CONNECTION-STRING>", "Connection string", CommandOptionType.SingleValue)
                    .IsRequired();
                AddUsernameAndPassword(user, out var username, out var password);
                
                user.Command("create", create => {
                    create.HelpOption();
                    create.OnExecuteAsync(async (cancel) => {
                        var repo = new KeyRepository(connectionString.Value(), logger);
                        await repo.Create(username.Value(), password.Value());
                    });
                });

                user.Command("change-password", changePassword =>
                {
                    changePassword.HelpOption();
                    var newPassword = changePassword
                        .Option("--new-password <NEW-PASSWORD>", "New password", CommandOptionType.SingleValue)
                        .IsRequired();
                    changePassword.OnExecuteAsync(async (cancel) =>
                    {
                        var repo = new KeyRepository(connectionString.Value(), logger);
                        var result = await repo.ChangePassword(username.Value(), password.Value(), newPassword.Value());
                        if (result)
                        {
                            Log.Information("Password change successful");
                        }
                        else
                        {
                            Log.Warning("Password change unsuccessful");
                        }
                    });
                });
                user.Command("key", getKey =>
                {
                    getKey.HelpOption();
                    getKey.OnExecuteAsync(async (cancel) =>
                    {
                        var repo = new KeyRepository(connectionString.Value(), logger);
                        var key = await repo.GetKey(username.Value(), password.Value());
                        Log.Information(key);
                    });
                });
            });

            app.Command("yammer", yammer => {
                yammer.HelpOption();
                var connectionString = yammer
                    .Option("-c|--connection-string <CONNECTION-STRING>", "Connection string", CommandOptionType.SingleValue)
                    .IsRequired();
                AddUsernameAndPassword(yammer, out var username, out var password);
                
                yammer.Command("backfill", backfill => {
                    backfill.HelpOption();
                    var token = backfill
                    .Option("-t|--token <TOKEN>", "Yammer authorization token", CommandOptionType.SingleValue)
                    .IsRequired();

                    backfill.OnExecuteAsync(async (cancel) =>
                    {
                        var events = CreateEventRepository(connectionString, username, password);
                        await Automate(logger, () => new YammerAutomation(logger, events, token.Value()).Backfill());
                    });
                });

                yammer.Command("process", process => {
                    process.HelpOption();
                    process.OnExecuteAsync(async (cancel) => {
                        var events = CreateEventRepository(connectionString, username, password);
                        await Automate(logger, () => new YammerAutomation(logger, events).Process());
                    });
                });
            });

            // app.Command("linkedin", linkedIn => {
            //     var username = linkedIn
            //         .Option("-u|--username <USERNAME>", "LinkedIn username", CommandOptionType.SingleValue)
            //         .IsRequired()
            //         .Accepts(a => a.EmailAddress());
            //     var password = linkedIn
            //         .Option("-p|--password <PASSWORD>", "LinkedIn password", CommandOptionType.SingleValue)
            //         .IsRequired();
            //     linkedIn.OnExecuteAsync(async (cancel) => {
            //         await Automate(logger, () => new Robot.LinkedInAutomation(logger, "", "", username.Value(), password.Value()).Automate());
            //     });
            // });

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

        private static EventRepository CreateEventRepository(CommandOption connectionString, CommandOption username, CommandOption password)
        {
            return new EventRepository(connectionString.Value(), username.Value(), password.Value());
        }

        private static void AddUsernameAndPassword(CommandLineApplication command, out CommandOption username, out CommandOption password)
        {
            username = command
                .Option("-u|--username <USERNAME>", "User name", CommandOptionType.SingleValue)
                .IsRequired();
            password = command
                .Option("-p|--password <PASSWORD>", "Password", CommandOptionType.SingleValue)
                .IsRequired();
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
