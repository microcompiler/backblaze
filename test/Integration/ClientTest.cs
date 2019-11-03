using System;
using System.Threading.Tasks;
using System.Security.Authentication;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

using Bytewizer.Backblaze.Client;

using Xunit;
using Bytewizer.Backblaze.Models;

namespace Backblaze.Tests.Integration
{
    public class ClientTest : IClassFixture<StorageClientFixture>
    {
        private BackblazeClient Client;

        private static readonly string _keyName = $"{Guid.NewGuid().ToString()}";

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
        public async Task Agent_DependencyInjection()
        {
            var services = new ServiceCollection();

            services.AddLogging(builder =>
            {
                builder.AddFilter("Bytewizer.Backblaze", LogLevel.Trace);
                builder.AddDebug();
            });

            services.AddBackblazeAgent(options =>
            {
                options.KeyId = Options.KeyId;
                options.ApplicationKey = Options.ApplicationKey;
            });

            services.AddMemoryCache();

            var serviceProvider = services.BuildServiceProvider();

            var logger = serviceProvider.GetService<ILogger<StorageService>>();
            var client = serviceProvider.GetService<IStorageClient>();
            
            var results = await client.Buckets.FindByNameAsync(BucketName, TimeSpan.FromSeconds(60));

            Assert.NotNull(logger);
            Assert.NotNull(client);
            Assert.Equal(BucketName, results.BucketName);
        }

        [Fact]
        public async Task Client_InitializeWithAppKey()
        {
            Client = BackblazeClient.Initialize(Options.KeyId, Options.ApplicationKey);

            var request = new CreateKeyRequest(Client.AccountId, _keyName, Capabilities.ReadOnly());
            var results = await Client.Keys.CreateAsync(request);
            results.EnsureSuccessStatusCode();

            var options = results.Response;
            var client = BackblazeClient.Initialize(options.ApplicationKeyId, options.ApplicationKey);

            var findResults = await client.Buckets.FindByNameAsync(BucketName);
            Assert.Equal(BucketName, findResults.BucketName);

            var deleteResults = await Client.Keys.DeleteAsync(options.ApplicationKeyId);
            Assert.Equal(typeof(DeleteKeyResponse), deleteResults.Response.GetType());
            Assert.Equal(Capabilities.ReadOnly(), deleteResults.Response.Capabilities);
        }

        [Fact]
        public async Task Client_Initialize()
        {
            Client = BackblazeClient.Initialize(Options.KeyId, Options.ApplicationKey);

            var results = await Client.Buckets.FindByNameAsync(BucketName);
            Assert.Equal(BucketName, results.BucketName);
        }

        [Fact]
        public async Task Client_Authentication()
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
        public async Task Client_Default()
        {
            using (var client = new BackblazeClient())
            {
                client.Connect(Options.KeyId, Options.ApplicationKey);

                var results = await client.Buckets.FindByNameAsync(BucketName);
                Assert.Equal(BucketName, results.BucketName);
            };
        }

        [Fact]
        public async Task Client_WithLoggerAndCache()
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
            }
        }
    }
}
