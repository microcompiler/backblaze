using System;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

using Backblaze.Tests.Integration;

using Bytewizer.Backblaze.Client;

using Xunit;

[assembly: TestFramework("Backblaze.Tests.Integration.XunitExtensions.XunitTestFrameworkWithAssemblyFixture", "Backblaze.Tests.Integration")]
[assembly: AssemblyFixture(typeof(StorageClientFixture))]

namespace Backblaze.Tests.Integration
{
    [TestCaseOrderer("Backblaze.Tests.Integration.PriorityOrderer", "Backblaze.Tests.Integration")]
    public abstract class BaseFixture : IClassFixture<StorageClientFixture>
    {
        public IServiceProvider Services { get; }
        public IConfiguration Config { get; }
        public ILogger Logger { get; }
        public IClientOptions Options { get; }
        public IStorageClient Storage { get; }

        public static readonly string ContentType = "application/octet-stream";
        public static readonly string BucketName = StorageClientFixture.BucketName;
        public static readonly string KeyName = StorageClientFixture.KeyName;
        
        public string BucketId;
        public string KeyId;

        protected BaseFixture(StorageClientFixture fixture)
        {
            Services = fixture.Services;
            Config = fixture.Config;
            Logger = fixture.Logger;
            Options = fixture.Options;
            Storage = fixture.Storage;
            BucketId = fixture.bucketId;
        }
    }
}
