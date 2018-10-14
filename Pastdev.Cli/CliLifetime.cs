using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Pastdev.Cli
{
    /// <summary>
    /// Waits from completion of the <see cref="CommandLineApplication"/> and
    /// initiates shutdown.
    /// </summary>
    public class CliLifetime : IHostLifetime
    {
        private readonly ILogger<CliLifetime> logger;
        private readonly IServiceProvider services;
        private readonly IApplicationLifetime applicationLifetime;
        private readonly ICliService cliService;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public CliLifetime(ILogger<CliLifetime> logger, IApplicationLifetime applicationLifetime,
            IServiceProvider services, ICliService cliService)
        {
            this.logger = logger;
            this.applicationLifetime = applicationLifetime;
            this.cliService = cliService;
        }

        /// <value>The exit code returned by the command line application</value>
        public int ExitCode { get; private set; } = 0;

        /// <inheritdoc/>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Registers an <code>ApplicationStarted</code> hook that runs the
        /// <see cref="ICliService"/>. This ensures the container and all
        /// hosted services are started before the
        /// <see cref="CommandLineApplication"/> is run.  After the
        /// <code>ICliService</code> completes, the <code>ExitCode</code> is
        /// recorded and the application is stopped.
        /// </summary>
        /// <param name="cancellationToken">Used to indicate when stop should no longer be graceful.</param>
        /// <returns></returns>
        /// <seealso cref="IHostLifetime.WaitForStartAsync(CancellationToken)"/>
        public Task WaitForStartAsync(CancellationToken cancellationToken)
        {
            applicationLifetime.ApplicationStarted.Register(async () =>
            {
                ExitCode = await cliService.RunAsync(cancellationToken).ConfigureAwait(false);
                applicationLifetime.StopApplication();
            });
            return Task.CompletedTask;
        }
    }
}
