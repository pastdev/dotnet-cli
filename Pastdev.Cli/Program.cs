using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace Ross.Database.Migrate.Postgres
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureLogging((context, builder) =>
                {
                    builder.AddConsole();
                })
                .UseCommandLineApplication<Program>(args)
                .RunConsoleAsync();
        }

        [Option(Description = "The subject")]
        public string Subject { get; }

        [Option(ShortName = "n")]
        public int Count { get; }

        private void OnExecute()
        {
            var subject = Subject ?? "world";
            for (var i = 0; i < Count; i++)
            {
                Console.WriteLine($"Hello {subject}!");
            }
        }
    }
}
