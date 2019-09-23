using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bytewizer.Backblaze.Agent.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateConsoleHostBuilder(args).Build().Run();
        }

        public static IConsoleHostBuilder CreateConsoleHostBuilder(string[] args) =>
            ConsoleHost.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
            });
    }
}
