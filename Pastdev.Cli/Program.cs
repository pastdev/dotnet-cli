using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace McMaster.Extensions.Hosting.CommandLine
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            return await new HostBuilder()
                .ConfigureLogging((context, builder) =>
                {
                    builder.AddConsole();
                })
                .RunCliAsync<Program>(args);
        }

        [Option(Description = "The subject")]
        public string Subject { get; }

        [Option(ShortName = "n")]
        public int Count { get; }

        private int OnExecute()
        {
            var subject = Subject ?? "world";
            for (var i = 0; i < Count; i++)
            {
                Thread.Sleep(2000);
                Console.WriteLine($"Hello {subject}!");
            }
            return 3;
        }
    }
}
