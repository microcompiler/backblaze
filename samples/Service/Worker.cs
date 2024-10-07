using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Bytewizer.Backblaze.Client;

namespace Backblaze.Service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        public readonly IStorageClient _client;

        public Worker(IStorageClient client, ILogger<Worker> logger)
        {
            _client = client;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var filePath in Directory.GetFiles(@"c:\backblaze"))
                {
                    using var stream = File.OpenRead(filePath);
                        var results = await _client.UploadAsync("ce6194ce5c3f2f746ce20d1b", new FileInfo(filePath).Name, stream);
                }

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(50000, stoppingToken);
            }
        }
    }
}
