using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Bytewizer.Backblaze.Client;
using Bytewizer.Backblaze.Models;

using System.IO.Abstractions.TestingHelpers;
using System.IO;

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
        private static readonly object _lock = new object();

        /// <summary>
        /// The default test bucket id to run test methods.
        /// </summary>
        public string bucketId;

        #endregion

        public StorageClientFixture()
        {
            lock (_lock)
            {
                var services = new ServiceCollection();

                var config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("settings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

                services.AddLogging(builder =>
                {
                    builder.AddConfiguration(config.GetSection("Logging"));
                    builder.AddConsole();
                    builder.AddDebug();
                });

                services.AddMemoryCache();
                services.AddBackblazeAgent(config.GetSection("Agent"));

                var serviceProvider = services.BuildServiceProvider();

                Config = serviceProvider.GetService<IConfiguration>();
                Logger = serviceProvider.GetService<ILogger<StorageClientFixture>>();
                Options = serviceProvider.GetService<IClientOptions>();
                Storage = serviceProvider.GetService<IStorageClient>();

                Storage.Connect();

                SeedStorage().GetAwaiter().GetResult();
            }
        }

        private async Task SeedStorage()
        {
            // Check for test bucket
            var testBucket = await Storage.Buckets.FindByNameAsync(BucketName);

            // If test bucket doesn't exist try to create
            if (testBucket == null)
            {
                // Create bucket
                var createResults = await Storage.Buckets.CreateAsync(BucketName, BucketType.AllPrivate);
                createResults.EnsureSuccessStatusCode();
                //Logger.LogDebug($"Bucket '{BucketName}' created during storage seed initialization.");

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
        }

        public void Dispose()
        {
            Storage?.Dispose();
        }
    }
}
