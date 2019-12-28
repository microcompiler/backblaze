using System;
using System.Threading.Tasks;
using System.Security.Authentication;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

using Bytewizer.Backblaze.Client;
using Bytewizer.Backblaze.Models;

using Xunit;

namespace Backblaze.Tests.Integration
{
    public class ClientTest : BaseFixture
    {
        private BackblazeClient Client;

        private readonly string _keyName = $"{Guid.NewGuid().ToString()}";
        private readonly string _bucketName = StorageClientFixture.BucketName;

        public ClientTest(StorageClientFixture fixture)
         : base(fixture)
        {
            Client = BackblazeClient.Initialize(Options.KeyId, Options.ApplicationKey);
        }

        //    [Fact]
        //    public async Task Agent_DependencyInjection()
        //    {
        //        var services = new ServiceCollection();

        //        services.AddLogging(builder =>
        //        {
        //            builder.AddFilter("Bytewizer.Backblaze", LogLevel.Trace);
        //            builder.AddDebug();
        //        });

        //        services.AddBackblazeAgent(options =>
        //        {
        //            options.KeyId = Options.KeyId;
        //            options.ApplicationKey = Options.ApplicationKey;
        //        });

        //        services.AddMemoryCache();

        //        var serviceProvider = services.BuildServiceProvider();

        //        var logger = serviceProvider.GetService<ILogger<StorageService>>();
        //        var client = serviceProvider.GetService<IStorageClient>();
        //        client.Connect();

        //        var results = await client.Buckets.FindByNameAsync(_bucketName, TimeSpan.FromSeconds(60));

        //        Assert.NotNull(logger);
        //        Assert.NotNull(client);
        //        Assert.Equal(_bucketName, results.BucketName);
        //    }

        //    [Fact]
        //    public async Task Client_InitializeWithAppKey()
        //    {
        //        var request = new CreateKeyRequest(Client.AccountId, _keyName, Capabilities.ReadOnly());
        //        var results = await Client.Keys.CreateAsync(request);
        //        results.EnsureSuccessStatusCode();

        //        var options = results.Response;
        //        var client = BackblazeClient.Initialize(options.ApplicationKeyId, options.ApplicationKey);

        //        var findResults = await client.Buckets.FindByNameAsync(_bucketName);
        //        Assert.Equal(_bucketName, findResults.BucketName);

        //        var deleteResults = await Client.Keys.DeleteAsync(options.ApplicationKeyId);
        //        Assert.Equal(typeof(DeleteKeyResponse), deleteResults.Response.GetType());
        //        Assert.Equal(Capabilities.ReadOnly(), deleteResults.Response.Capabilities);
        //    }

        //    [Fact]
        //    public async Task Client_Initialize()
        //    {
        //        var results = await Client.Buckets.FindByNameAsync(_bucketName);
        //        Assert.Equal(_bucketName, results.BucketName);
        //    }

        //    [Fact]
        //    public async Task Client_Authentication()
        //    {
        //        await Assert.ThrowsAsync<AuthenticationException>(async () =>
        //        {
        //            using (var client = new BackblazeClient())
        //            {
        //                await client.ConnectAsync("Bad_Key_Id", "Bad_Appkication_Key");
        //            };
        //        });
        //    }

        //    [Fact]
        //    public async Task Client_Default()
        //    {
        //        using (var client = new BackblazeClient())
        //        {
        //            client.Connect(Options.KeyId, Options.ApplicationKey);

        //            var results = await client.Buckets.FindByNameAsync(_bucketName);
        //            Assert.Equal(_bucketName, results.BucketName);
        //        };
        //    }

        //    [Fact]
        //    public async Task Client_WithLoggerAndCache()
        //    {
        //        var options = Options;

        //        var loggerFactory = LoggerFactory.Create(builder =>
        //        {
        //            builder
        //                .AddFilter("Microsoft", LogLevel.Warning)
        //                .AddFilter("System", LogLevel.Warning)
        //                .AddFilter("Bytewizer.Backblaze", LogLevel.Trace)
        //                .AddDebug();
        //        });

        //        var cache = new MemoryCache(new MemoryCacheOptions());

        //        using (var client = new BackblazeClient(options, loggerFactory, cache))
        //        {
        //            client.Connect();

        //            var results = await client.Buckets.FindByNameAsync(_bucketName);
        //            Assert.Equal(_bucketName, results.BucketName);
        //        }
        //    }
    }
}
