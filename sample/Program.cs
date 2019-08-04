using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Backblaze.Sample
{
    class Program
    {
        // Note: Run with Control + F5 to keep the console window open
        static async Task Main(string[] args)
        {
            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            try
            {
                var services = new ServiceCollection();

                services.AddLogging(builder =>
                {
                    builder.AddDebug();
                })
               .Configure<LoggerFilterOptions>(options =>
                    options.MinLevel = LogLevel.Debug
                );

                services.AddBackblazeAgent(options =>
                {
                    options.KeyId = "[key_id]";
                    options.ApplicationKey = "[application_key]";
                });
                services.AddSingleton<Application>();
                var serviceProvider = services.BuildServiceProvider();
                await serviceProvider.GetService<Application>().MainAsync(args, cts.Token);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}