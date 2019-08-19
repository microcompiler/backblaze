using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
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
                ConfigureServices(services);
                var serviceProvider = services.BuildServiceProvider(); 
                var logger = serviceProvider.GetService<ILogger<Program>>();

                logger.LogDebug("Woo Hooo");

                await serviceProvider.GetService<Storage>().MainAsync(args, cts.Token);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }

        internal static void ConfigureServices(IServiceCollection services)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("settings.json", optional: true, reloadOnChange: true)
                .Build();

            services.AddLogging(builder =>
            {
                builder.AddConfiguration(config.GetSection("Logging"));
                builder.AddDebug();
                //builder.AddConsole();
            });
            
            services.AddBackblazeAgent(config.GetSection("Agent"));

            services.AddSingleton<Storage>();
        }
    }
}