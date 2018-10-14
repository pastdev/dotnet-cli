using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pastdev.Cli.Internal;

namespace Pastdev.Cli
{
    /// <summary>
    /// A placeholder until
    /// <see href="https://github.com/natemcmaster/CommandLineUtils/issues/134">host support</see>
    /// is added to cli itself.
    /// </summary>
    public static class HostBuilderExtensions
    {
        public static async Task<int> RunCliAsync<T>(this IHostBuilder hostBuilder, string[] args) where T : class
        {
            using (var host = hostBuilder.UseCliLifetime().UseCli<T>(args).Build()) 
            {
                await host.StartAsync();
                await host.WaitForShutdownAsync();
                return ((CliLifetime)host.Services.GetService<IHostLifetime>()).ExitCode;
            }
        }

        public static IHostBuilder UseCliLifetime(this IHostBuilder hostBuilder)
            => hostBuilder.ConfigureServices((context, services) => services.AddSingleton<IHostLifetime, CliLifetime>());

        public static IHostBuilder UseCli<T>(this IHostBuilder hostBuilder, string[] args) where T : class
            => hostBuilder.ConfigureServices(
                    (context, services) =>
                    {
                        services.AddSingleton<CliArgs>(new CliArgs{Value = args});
                        services.AddSingleton<ICliService, CliService<T>>();
                    });
    }
}
