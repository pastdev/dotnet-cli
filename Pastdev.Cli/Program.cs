namespace Pastdev.Cli
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Greets a person.
    /// </summary>
    public class Program
    {
        /// <summary/><value>The person to greet.</value>
        [Option(Description = "The subject")]
        public string Subject { get; }

        /// <summary/><value>The number of times to write the message.</value>
        [Option(ShortName = "n")]
        public int Count { get; }

        /// <summary>
        /// The main method.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        /// <returns>A task for an int.</returns>
        public static async Task<int> Main(string[] args)
        {
            return await new HostBuilder()
                .ConfigureLogging((context, builder) =>
                {
                    builder.AddConsole();
                })
                .RunCliAsync<Program>(args)
                .ConfigureAwait(false);
        }

        private int OnExecute()
        {
            var subject = this.Subject ?? "world";

            for (var i = 0; i < this.Count; i++)
            {
                Thread.Sleep(2000);
                Console.WriteLine($"Hello {subject}!");
            }

            return 3;
        }
    }
}
