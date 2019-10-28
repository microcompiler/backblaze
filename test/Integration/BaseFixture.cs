using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

using Bytewizer.Backblaze.Client;
using Bytewizer.Backblaze.Models;
using Bytewizer.Backblaze.Extensions;
using Bytewizer.Backblaze.Enumerables;

using Xunit;

using System.IO.Abstractions.TestingHelpers;

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
        public MockFileSystem FileSystem { get; }

        public BaseFixture(StorageClientFixture fixture)
        {
            Services = fixture.Services;
            Config = fixture.Config;
            Logger = fixture.Logger;
            Options = fixture.Options;
            Storage = fixture.Storage;
            FileSystem = fixture.FileSystem;

            BucketId = fixture.bucketId;
        }

        public string BucketName = StorageClientFixture.BucketName;
        public string BucketId;
        public string KeyName = StorageClientFixture.KeyName;
        public string KeyId;
    }
}
