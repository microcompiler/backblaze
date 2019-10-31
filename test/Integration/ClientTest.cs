using System.Threading.Tasks;
using System.Security.Authentication;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;

using Bytewizer.Backblaze.Client;

using Xunit;

namespace Backblaze.Tests.Integration
{
    public class ClientTest : IClassFixture<StorageClientFixture>
    {
        private readonly BackblazeClient Client;

        public ClientTest(StorageClientFixture fixture)
        {
            Options = fixture.Options;
            BucketId = fixture.bucketId;

            Client = BackblazeClient.Initialize(Options.KeyId, Options.ApplicationKey);
        }
        public IClientOptions Options { get; }

        public string BucketName = StorageClientFixture.BucketName;
        public string KeyName = StorageClientFixture.KeyName;
        public string BucketId;

        [Fact]
        public async Task BackblazeClient_Initialize()
        {
            var results = await Client.Buckets.FindByNameAsync(BucketName);
            Assert.Equal(BucketName, results.BucketName);
        }

        [Fact]
        public async Task BackblazeClient_Authentication()
        {
            await Assert.ThrowsAsync<AuthenticationException>(async () =>  
            {
                using (var client = new BackblazeClient())
                {
                    await client.ConnectAsync("Bad_Key_Id", "Bad_Appkication_Key");
                };
            });
        }

        [Fact]
        public async Task BackblazeClient_Default()
        {
            using (var client = new BackblazeClient())
            {
                client.Connect(Options.KeyId, Options.ApplicationKey);

                var results = await client.Buckets.FindByNameAsync(BucketName);
                Assert.Equal(BucketName, results.BucketName);
            };
        }

        [Fact]
        public async Task BackblazeClient_WithLoggerAndCache()
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
