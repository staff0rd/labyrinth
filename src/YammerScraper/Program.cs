using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using EventStore.ClientAPI.Exceptions;
using McMaster.Extensions.CommandLineUtils;
using McMaster.Extensions.CommandLineUtils.Validation;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;

namespace YammerScraper
{
    class Program
    {
        static int Main(string[] args)
        {
            var config = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}")
                .WriteTo.Trace();
            
            Log.Logger = config.CreateLogger();
            
            var app = new CommandLineApplication();

            app.HelpOption();

            var token = app
                .Option("-t|--token <TOKEN>", "Yammer authorization token", CommandOptionType.SingleValue)
                .IsRequired();

            var yammerUrl = app
                .Option("-u|--url <URL>", "Yammer url", CommandOptionType.SingleValue)
                .IsRequired();

            yammerUrl.Validators.Add(new MustBeUriValidator());

            app.OnExecuteAsync(async (cancel) =>
            {
                var logger = new SerilogLoggerProvider(Log.Logger).CreateLogger(nameof(Program));
                
                var scraper = new Scraper(new Uri(yammerUrl.Value()), token.Value(), logger);
                
                await scraper.Automate();
                
                return 0;
            });

            try
            {
                Log.Information("Welcome to YammerScraper!");

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
