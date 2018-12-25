namespace Pastdev.Cli.Internal
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.Extensions.Logging;

    /// <inheritdoc/>
    internal class CliService<T> : ICliService
        where T : class
    {
        private ILogger logger;
        private CommandLineApplication application;
        private CliArgs args;

        /// <summary>
        /// Initializes a new instance of the <see cref="CliService{T}"/> class.
        /// </summary>
        /// <param name="logger">A logger.</param>
        /// <param name="args">The command line arguments.</param>
        /// <param name="serviceProvider">The DI service provider.</param>
        public CliService(ILogger<CliService<T>> logger, CliArgs args, IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.args = args;

            logger.LogDebug(
                "Constructing CommandLineApplication<{type}> with args [{args}]",
                typeof(T).FullName,
                string.Join(",", args.Value));
            this.application = new CommandLineApplication<Program>();
            this.application.Conventions
                .UseDefaultConventions()
                .UseConstructorInjection(serviceProvider);
        }

        /// <inheritdoc/>
        public Task<int> RunAsync(CancellationToken cancellationToken)
        {
            this.logger.LogDebug("Running");
            return Task.Run(() => this.application.Execute(this.args.Value));
        }
    }
}
