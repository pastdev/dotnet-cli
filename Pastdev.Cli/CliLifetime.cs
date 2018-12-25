namespace Pastdev.Cli
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Waits from completion of the <see cref="CommandLineApplication"/> and initiates shutdown.
    /// </summary>
    public class CliLifetime : IHostLifetime
    {
        private readonly ILogger<CliLifetime> logger;
        private readonly IApplicationLifetime applicationLifetime;
        private readonly ICliService cliService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CliLifetime"/> class.
        /// </summary>
        /// <param name="logger">A logger instance.</param>
        /// <param name="applicationLifetime">The lifetime of the application.</param>
        /// <param name="cliService">The CliService instance.</param>
        public CliLifetime(ILogger<CliLifetime> logger, IApplicationLifetime applicationLifetime, ICliService cliService)
        {
            this.logger = logger;
            this.applicationLifetime = applicationLifetime;
            this.cliService = cliService;
        }

        /// <summary/><value>The exit code returned by the command line application.</value>
        public int ExitCode { get; private set; } = 0;

        /// <inheritdoc/>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Registers an <c>ApplicationStarted</c> hook that runs the <see cref="ICliService"/>.
        /// This ensures the container and all hosted services are started before the <see cref="CommandLineApplication"/> is run.
        /// After the <c>ICliService</c> completes, the <c>ExitCode</c> is recorded and the application is stopped.
        /// </summary>
        /// <param name="cancellationToken">Used to indicate when stop should no longer be graceful.</param>
        /// <returns>A task.</returns>
        /// <seealso cref="IHostLifetime.WaitForStartAsync(CancellationToken)"/>
        public Task WaitForStartAsync(CancellationToken cancellationToken)
        {
            this.applicationLifetime.ApplicationStarted.Register(async () =>
            {
                this.ExitCode = await this.cliService.RunAsync(cancellationToken).ConfigureAwait(false);
                this.applicationLifetime.StopApplication();
            });
            return Task.CompletedTask;
        }
    }
}
