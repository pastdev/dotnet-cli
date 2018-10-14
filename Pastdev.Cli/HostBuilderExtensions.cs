using System;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Ross.Database.Migrate.Postgres
{
    /// <summary>
    /// A placeholder until
    /// <see href="https://github.com/natemcmaster/CommandLineUtils/issues/134">host support</see>
    /// is added to cli itself.
    /// </summary>
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseCommandLineApplication<T>(
            this IHostBuilder hostBuilder, string[] args)
            where T : class
        {
            hostBuilder.ConfigureServices(
                (context, services) =>
                {
                    services.Configure<ConsoleLifetimeOptions>(
                        options => options.SuppressStatusMessages = true); 
                    services.AddSingleton<CliArgs>(new CliArgs{Value = args});
                    services.AddHostedService<CliHostedService<T>>();
                });
            return hostBuilder;
        }
    }

    public class CliArgs
    {
        public string[] Value {get; set;}
    }

    public class CliHostedService<T> : IHostedService where T : class
    {
        private ILogger logger;
        private IApplicationLifetime applicationLifetime;
        private CommandLineApplication application;
        private CliArgs args;

        public CliHostedService(ILogger<CliHostedService<T>> logger, CliArgs args,
            IServiceProvider serviceProvider, IApplicationLifetime applicationLifetime)
        {
            this.logger = logger;
            this.args = args;
            this.applicationLifetime = applicationLifetime;

            application = new CommandLineApplication<Program>();
            application.Conventions
                .UseDefaultConventions()
                .UseConstructorInjection(serviceProvider);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogDebug("Starting");
            application.Execute(args.Value);
            applicationLifetime.StopApplication();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogDebug("Stopping");
            return Task.CompletedTask;
        }
    }
}
