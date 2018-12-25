namespace Pastdev.Cli
{
    using System.Linq;
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Pastdev.Cli.Internal;

    /// <summary>
    /// Extension methods for <see cref="IHostBuilder"/> support.
    /// </summary>
    /// <seealso href="https://github.com/natemcmaster/CommandLineUtils/issues/134">host support</seealso>
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// Runs <c>T</c> as a <see cref="CommandLineApplication"/> with the supplied <c>args</c>.
        /// This method should be the primary approach taken for command line applications.
        /// </summary>
        /// <typeparam name="T">The command line application implementation.</typeparam>
        /// <param name="hostBuilder">This instance.</param>
        /// <param name="args">The command line arguments.</param>
        /// <returns>A task whose result is the exit code of the application.</returns>
        public static async Task<int> RunCliAsync<T>(this IHostBuilder hostBuilder, string[] args)
            where T : class
        {
            using (var host = hostBuilder.UseCliLifetime().UseCli<T>(args).Build())
            {
                await host.StartAsync().ConfigureAwait(false);
                await host.WaitForShutdownAsync().ConfigureAwait(false);
                return ((CliLifetime)host.Services.GetService<IHostLifetime>()).ExitCode;
            }
        }

        /// <summary>
        /// Sets the <see cref="IHostLifetime"/> to one that handles running a <see cref="CommandLineApplication"/>.
        /// It will ensure the <see cref="ICliService"/> will be run to completion, the exit code gathered, and the hosted application stopped.
        /// <para>
        /// This method is not intended to be used directly.  Instead, you should use <see cref="RunCliAsync{T}(IHostBuilder, string[])"/>.
        /// </para>
        /// </summary>
        /// <param name="hostBuilder">This instance.</param>
        /// <returns>This instance for chaining.</returns>
        public static IHostBuilder UseCliLifetime(this IHostBuilder hostBuilder)
            => hostBuilder.ConfigureServices((context, services) => services.AddSingleton<IHostLifetime, CliLifetime>());

        /// <summary>
        /// Sets the <see cref="ICliService"/> to use <c>T</c> for its <see cref="CommandLineApplication"/>.
        /// <para>
        /// This method is not intended to be used directly.
        /// Instead, you should use <see cref="RunCliAsync{T}(IHostBuilder, string[])"/>.
        /// </para>
        /// </summary>
        /// <typeparam name="T">The command line application implementation.</typeparam>
        /// <param name="hostBuilder">This instance.</param>
        /// <param name="args">The command line arguments.</param>
        /// <returns>This instance for chaining.</returns>
        public static IHostBuilder UseCli<T>(this IHostBuilder hostBuilder, string[] args)
            where T : class
            => hostBuilder.ConfigureServices(
                    (context, services) =>
                    {
                        services.AddSingleton<CliArgs>(new CliArgs { Value = args });
                        services.AddSingleton<ICliService, CliService<T>>();
                    });
    }
}
