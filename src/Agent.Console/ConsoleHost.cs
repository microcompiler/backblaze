using System;
using System.IO;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bytewizer.Backblaze.Agent.Console
{
    /// <summary>
    /// Provides convenience methods for creating instances of <see cref="IConsoleHost"/> and <see cref="IConsoleHostBuilder"/> with pre-configured defaults.
    /// </summary>
    internal class ConsoleHost : IConsoleHost
    {
        public IServiceProvider Services { get; }

        public static IConsoleHostBuilder CreateDefaultBuilder() =>
            CreateDefaultBuilder(args: null);

        public static IConsoleHostBuilder CreateDefaultBuilder(string[] args)
        {
            var builder = new ConsoleHostBuilder();

            builder.ConfigureHost(config =>
            {
                // Add settings file
                config.SetBasePath(Directory.GetCurrentDirectory());
                config.AddJsonFile("settings.json", optional: true, reloadOnChange: true);

                // Add enviroment variables
                config.AddEnvironmentVariables(prefix: "BACKBLAZE_");
                if (args != null)
                {
                    config.AddCommandLine(args);
                }
            })
            .ConfigureLogging((context, logging) =>
            {
                // Add logging
                logging.AddConfiguration(context.Configuration.GetSection("Logging"));
                logging.AddConsole();
                logging.AddDebug();
            })
            .ConfigureServices((context, services) =>
            {
                // Add memory cache
                services.AddMemoryCache();

                // Add backblaze agent
                services.AddBackblazeAgent(context.Configuration.GetSection("Agent"));
            });

            return builder;
        }

        public void Run()
        {
        }
    }
}
