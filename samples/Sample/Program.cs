using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Bytewizer.Extensions.Console;

namespace Backblaze.Sample
{
    class Program
    {
        // Note: Run with Control + F5 to keep the console window open
        static async Task Main(string[] args)
        {
            try
            {
                var cts = new CancellationTokenSource();
                Console.CancelKeyPress += (s, e) =>
                {
                    e.Cancel = true;
                    cts.Cancel();
                };

                await CreateConsoleBuilder(args).Build().RunAsync();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }

        private static IApplicationBuilder CreateConsoleBuilder(string[] args) =>
           ConsoleApplication.CreateDefaultBuilder(args)
               .UseStartup<Application>()
               .ConfigureServices((context, services) =>
               {
                   services.AddMemoryCache();
                   services.AddBackblazeAgent(context.Configuration.GetSection("Agent"));
               });
    }
}