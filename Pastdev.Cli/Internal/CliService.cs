using System;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

namespace Pastdev.Cli.Internal
{
    internal class CliService<T> : ICliService where T : class
    {
        private ILogger logger;
        private CommandLineApplication application;
        private CliArgs args;

        public CliService(ILogger<CliService<T>> logger, CliArgs args,
            IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.args = args;

            logger.LogDebug("Constructing CommandLineApplication<{type}> with args [{args}]",
                typeof(T).FullName, String.Join(",", args.Value));
            application = new CommandLineApplication<Program>();
            application.Conventions
                .UseDefaultConventions()
                .UseConstructorInjection(serviceProvider);
        }

        public Task<int> RunAsync(CancellationToken cancellationToken)
        {
            logger.LogDebug("Running");
            return Task.Run(() => application.Execute(args.Value));
        }
    }
}
