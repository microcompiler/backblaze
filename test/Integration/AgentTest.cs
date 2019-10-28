using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

using Bytewizer.Backblaze.Client;

using Xunit;

namespace Backblaze.Tests.Integration
{
    public class AgentTest : IClassFixture<StorageClientFixture>
    {
        public AgentTest(StorageClientFixture fixture)
        {
            BucketName = StorageClientFixture.BucketName;
            KeyName = StorageClientFixture.KeyName;

            Services = fixture.Services;
            Configuration = fixture.Config;
            Logger = fixture.Logger;
            Options = fixture.Options;
            Storage = fixture.Storage;
        }


        public string BucketName;
        public string KeyName;

        public IServiceProvider Services { get; }
        public IConfiguration Configuration { get; }
        public ILogger Logger { get; }
        public IClientOptions Options { get; }
        public IStorageClient Storage { get; }

        //[Fact]
        //public async Task Agent_Example()
        //{
        //    var testBucket = await Storage.Buckets.FindByNameAsync(BucketName);
        //    Logger.LogCritical(testBucket.BucketName);
        //    await Task.CompletedTask;
        //}
    }
}
