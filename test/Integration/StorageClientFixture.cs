using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Bytewizer.Backblaze.Client;
using Bytewizer.Extensions.Console;
using Bytewizer.Backblaze.Models;


using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.IO;
using System.Diagnostics;
using System.Linq;

namespace Backblaze.Tests.Integration
{
    public class StorageClientFixture : IDisposable
    {
        #region Constants

        /// <summary>
        /// The default test bucket created to run test methods in.
        /// </summary>
        public static readonly string BucketName = "integraton-test-bucket-db7e";

        /// <summary>
        /// The default test key created to run test methods with.
        /// </summary>
        public static readonly string KeyName = "integration-test-key-db7e";

        #endregion

        #region Public Properties

        public IServiceProvider Services { get; }
        public IConfiguration Config { get; }
        public ILogger Logger { get; }
        public IClientOptions Options { get; }
        public IStorageClient Storage { get; }
        public MockFileSystem FileSystem { get; }
        public MockFileSystem LargeFileSystem { get; }

        #endregion

        #region Private Fields

        /// <summary>
        /// Thread synchronization object.
        /// </summary>
        private static object _lock = new object();

        /// <summary>
        /// Configuration flag.
        /// </summary>
        //private static bool _initialized;

        /// <summary>
        /// The default test bucket id to run test methods.
        /// </summary>
        public string bucketId;

        /// <summary>
        /// The default test file id to run test methods.
        /// </summary>
        public string fileId;

        /// <summary>
        /// The default identifier for the account.
        /// </summary>
        public string accountId;

        #endregion

        public StorageClientFixture()
        {
            Services = CreateConsoleBuilder(null).Build().Services;

            Config = Services.GetService<IConfiguration>();
            Logger = Services.GetService<ILogger<StorageClientFixture>>();
            Options = Services.GetService<IClientOptions>();
            Storage = Services.GetService<IStorageClient>();
            FileSystem = SeedFileSystem();
            LargeFileSystem = SeedLargeFileSystem();

            lock (_lock)
            {
                SeedStorage().GetAwaiter().GetResult();
            }
        }

        public static IApplicationBuilder CreateConsoleBuilder(string[] args) =>
        ConsoleApplication.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddMemoryCache();
                services.AddBackblazeAgent(context.Configuration.GetSection("Agent"));
            });

        private MockFileSystem SeedFileSystem()
        {
            var fileSystem = new MockFileSystem();

            fileSystem.AddFile(@"c:\root-five-bytes.bin", new MockFileData(new byte[] { 0x01, 0x34, 0x56, 0xd2, 0xd2 }));
            fileSystem.AddFile(@"c:\matrix\five-bytes.bin", new MockFileData(new byte[] { 0x02, 0x34, 0x56, 0xd2, 0xd2 }));
            fileSystem.AddFile(@"c:\shawshank\five-bytes.bin", new MockFileData(new byte[] { 0x03, 0x34, 0x56, 0xd2, 0xd2 }));

            return fileSystem;
        }

        private MockFileSystem SeedLargeFileSystem()
        {
            var content = Enumerable.Range(0, (int)(ClientOptions.MinimumCutoffSize * 1.2)).Select(i => (byte)i).ToArray();
            var largeFileSystem = new MockFileSystem();

            largeFileSystem.AddFile(@"c:\six-megabyte.bin", new MockFileData(content));

            return largeFileSystem;
        }

        private async Task SeedStorage()
        {
            //if (_initialized)
            //    return;

            //Check for test bucket
            var testBucket = await Storage.Buckets.FindByNameAsync(BucketName);

            // If test bucket doesn't exist try to create
            if (testBucket == null)
            {
                // Create bucket
                var createResults = await Storage.Buckets.CreateAsync(BucketName, BucketType.AllPrivate);
                createResults.EnsureSuccessStatusCode();
                Logger.LogDebug($"Bucket '{BucketName}' created during storage seed initialization.");

                // Set bucket id
                bucketId = createResults.Response.BucketId;
            }
            else
            {
                bucketId = testBucket.BucketId;
            }

            //Check for test key
            var testKey = await Storage.Keys.FindByNameAsync(KeyName);

            // If test key doesn't exist try to create
            if (testKey == null)
            {
                // Create a test key 
                var keyResults = await Storage.Keys.CreateAsync(KeyName, Capabilities.AllControl());
                keyResults.EnsureSuccessStatusCode();
                Logger.LogDebug($"Key '{KeyName}' created during storage seed initialization.");
            }

            Logger.LogInformation("Storage seed initialization completed");

            //_initialized = true;

        }

        public void Dispose()
        {
            Storage?.Dispose();
        }
    }
}
