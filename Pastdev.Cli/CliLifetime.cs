using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Pastdev.Cli
{
    public class CliLifetime : IHostLifetime
    {
        private ILogger<CliLifetime> logger;

        public CliLifetime(ILogger<CliLifetime> logger, IApplicationLifetime applicationLifetime,
            IServiceProvider services, ICliService cliService)
        {
            this.logger = logger;
            this.ApplicationLifetime = applicationLifetime;
            this.CliService = cliService;
        }

        private IApplicationLifetime ApplicationLifetime { get; }

        private ICliService CliService { get; }

        public int ExitCode { get; private set; } = 0;

        private IServiceProvider Services { get; }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task WaitForStartAsync(CancellationToken cancellationToken)
        {
            ApplicationLifetime.ApplicationStarted.Register(async () =>
            {
                ExitCode = await CliService.RunAsync(cancellationToken).ConfigureAwait(false);
                ApplicationLifetime.StopApplication();
            });
            return Task.CompletedTask;
        }
    }
}
