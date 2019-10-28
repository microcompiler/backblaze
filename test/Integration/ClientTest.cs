using System;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Bytewizer.Backblaze.Client;

using Xunit;

using Microsoft.Extensions.Caching.Memory;

namespace Backblaze.Tests.Integration
{
    public class ClientTest : IClassFixture<StorageClientFixture>
    {
        private readonly static BackblazeClient Client = 
            BackblazeClient.Initialize("[key_id]", "[application_key]");

        public ClientTest(StorageClientFixture fixture)
        {
            Options = fixture.Options;
            BucketId = fixture.bucketId;
        }
        public IClientOptions Options { get; }

        public string BucketName = StorageClientFixture.BucketName;
        public string KeyName = StorageClientFixture.KeyName;
        public string BucketId;

        [Fact]
        public async Task BackblazeAgent_Default()
        {
            using (var client = new BackblazeClient())
            {
                client.Connect(Options.KeyId, Options.ApplicationKey);

                var results = await client.Buckets.FindByNameAsync(BucketName);
                Assert.Equal(BucketName, results.BucketName);
            };
        }

        [Fact]
        public async Task BackblazeAgent_Initialize()
        {
            using (var client = BackblazeClient.Initialize(Options.KeyId, Options.ApplicationKey))
            {
                client.Connect();

                var results = await client.Buckets.FindByNameAsync(BucketName);
                Assert.Equal(BucketName, results.BucketName);
            };
        }

        [Fact]
        public async Task InitializeAgent_WithLoggerAndCache()
        {
            var options = Options;

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("Bytewizer.Backblaze", LogLevel.Trace)
                    .AddDebug();
            });

            var cache = new MemoryCache(new MemoryCacheOptions());

            using (var client = new BackblazeClient(options, loggerFactory, cache))
            {
                client.Connect();

                var results = await client.Buckets.FindByNameAsync(BucketName);
                Assert.Equal(BucketName, results.BucketName);
            };
        }
    }
}
