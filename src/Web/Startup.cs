using System;
using System.Diagnostics;
using Events;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using Hangfire;
using Hangfire.PostgreSql;
using MediatR;
using Hangfire.Console;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Diagnostics;

namespace Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews().AddNewtonsoftJson(options => {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            });

            services.AddSignalR();

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });

            services.AddHangfire(config => {
		        config.UsePostgreSqlStorage(Configuration.GetConnectionString("HangfireConnection"));
                config.UseConsole();
                config.UseSerializerSettings(new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
            });

            services.AddSingleton<CredentialCache>();
            services.AddScoped<EventRepository>();
            services.AddScoped<SourceRepository>();
            services.AddScoped<RestEventManager>();

            services.AddSingleton<NpgsqlConnectionFactory>((services) => {
                return new NpgsqlConnectionFactory(Configuration.GetConnectionString("EventsConnection"));
            });;

            services.AddScoped<KeyRepository>();
            services.AddSingleton<DatabaseMigrator>();
            services.AddMediatR(GetType().Assembly);
            services.AddMediatR(typeof(Store).Assembly);

            services.AddSingleton<TaskHubSender>();
            services.AddSingleton<JobActivator, InjectContextJobActivator>();
            services.AddTransient<IProgress, HangfireSignalRProgress>();

            services.AddSingleton<Store>(provider => {
                var logger = provider.GetRequiredService<ILogger<Store>>();
                var progress = provider.GetRequiredService<IProgress>();
                var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();
                try
                {
                    using (var scope = scopeFactory.CreateScope())
                    {
                        var events = scope.ServiceProvider.GetRequiredService<EventRepository>();
                        var store = new Store(events, logger, progress);
                        return store;
                    }
                } 
                catch (Exception e)
                {
                    logger.LogError(e, $"Failed to create {nameof(Store)}");
                    throw;
                }
            });
        }

        public void ConfigureExceptions(IApplicationBuilder app)
        {
            app.UseExceptionHandler(errorApp =>
        {
                errorApp.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "text/plain";

                    var exceptionHandlerPathFeature = 
                        context.Features.Get<IExceptionHandlerPathFeature>();

                    // if (exceptionHandlerPathFeature?.Error is FileNotFoundException)
                    // {
                    //     await context.Response.WriteAsync("File error thrown!<br><br>\r\n");
                    // }

                    await context.Response.WriteAsync(exceptionHandlerPathFeature?.Error?.Message);
                });
            });
        }
        

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory,
            DatabaseMigrator migrator)
        {
            loggerFactory.AddProvider(new HangfireConsoleLoggerProvider());

            if (env.IsDevelopment())
            {
                ConfigureExceptions(app);
            }
            else
            {
                ConfigureExceptions(app);
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            migrator.Migrate().Wait();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseHangfireServer();
            app.UseHangfireDashboard();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                endpoints.MapHub<TaskHub>("/taskHub");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:3000");
                }
            });
        }
    }
}
