namespace Pastdev.Cli
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// A DI container for storing command line arguments.
    /// </summary>
    /// <seealso cref="HostBuilderExtensions.UseCli{T}(Microsoft.Extensions.Hosting.IHostBuilder, string[])"/>
    public class CliArgs
    {
        /// <summary/><value>The command line arguments.</value>
        public string[] Value { get; set; }
    }
}
