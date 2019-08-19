using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Bytewizer.Backblaze.Agent;
using Bytewizer.Backblaze.Models;


namespace Backblaze.Test
{
    [TestClass]
    public class Agent
    {
        #region Constants

        /// <summary>
        /// The key identifier used to log in to the Backblaze B2 Cloud Storage service.
        /// </summary>
        public const string KeyId = "e14ecff4c2db";

        /// <summary>
        /// The secret part of the key used to log in to the Backblaze B2 Cloud Storage service.
        /// </summary>
        public const string ApplicationKey = "0007eb0f509d3f8d7b40f8594b10ea501dd48303e8";

        /// <summary>
        /// The default test bucket created to run test methods.
        /// </summary>
        public const string BucketName = "e6b1db7e-9749-4686-testbucket";

        /// <summary>
        /// The default test key created to run test methods.
        /// </summary>
        public const string KeyName = "7eb0f509-dd4830-testkey";

        /// <summary>
        /// The small memory stream size.
        /// </summary>
        public const int SmallStreamSize = (int)(1 * MB);

        /// <summary>
        /// A small memory stream use to test upload and download methods. 
        /// </summary>
        public MemoryStream SmallStream =
            new MemoryStream(Enumerable.Range(0, SmallStreamSize).Select(i => (byte)i).ToArray());

        /// <summary>
        /// A file name associated with the small memory stream. 
        /// </summary>
        public const string SmallStreamName = "c:/test/smallfile.bin";

        /// <summary>
        /// Kilobyte
        /// </summary>
        public const long KB = 0x400;

        /// <summary>
        /// Megabyte
        /// </summary>
        public const long MB = 0x100000;

        /// <summary>
        /// Gigabyte
        /// </summary>
        public const long GB = 0x40000000;

        #endregion

        #region Private Fields

        /// <summary>
        /// Client for making B2 server requests.
        /// </summary>
        public readonly Storage _storage;

        /// <summary>
        /// The default test bucket id to run test methods.
        /// </summary>
        public string _bucketId;

        /// <summary>
        /// The default test file id to run test methods.
        /// </summary>
        public string _fileId;

        /// <summary>
        /// The default identifier for the account.
        /// </summary>
        public string _accountId;

        #endregion

        #region Lifetime

        public Agent()
        {
            // Create service collection
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            // Create service provider
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // Run application
            _storage = serviceProvider.GetService<Storage>();

            // Set account id
            _accountId = _storage.Agent.AccountId;

            // Initialize test bucket
            InitializeBucketAsync().GetAwaiter().GetResult();
        }

        private async Task InitializeBucketAsync()
        {
            // Get test bucket
            var bucketList = await _storage.Agent.Buckets.GetAsync();
            var testBucket = bucketList.Response.Buckets.Find(x => x.BucketName == BucketName);

            // If test bucket doesn't exist try to create
            if (testBucket == null)
            {
                // Create bucket
                var createResults = await _storage.Agent.Buckets.CreateAsync(BucketName, BucketType.AllPrivate);

                // Set bucket id
                _bucketId = createResults.Response.BucketId;

                // Upload small test file
                var fileResults = await _storage.Agent.UploadAsync(createResults.Response.BucketId, SmallStreamName, SmallStream);

                // Set file id
                _fileId = fileResults.Response.FileId;

                // Create a test key
                var capabilities = new string[] { "listBuckets", "listFiles", "readFiles", "shareFiles", "writeFiles", "deleteFiles" };
                var keyResults = await _storage.Agent.Keys.CreateAsync(capabilities, KeyName);
            }
            else
            {
                // Set bucket id
                _bucketId = testBucket.BucketId;

                // Set file id
                var fileList = await _storage.Agent.Files.GetNamesAsync(_bucketId);
                var testFile = fileList.Response.Files.Find(x => x.FileName == SmallStreamName);
                _fileId = testFile.FileId;
            }

            Assert.IsNotNull(_bucketId);
            Assert.IsNotNull(_fileId);
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("settings.json", optional: true, reloadOnChange: true)
                .Build();

            // Add logging
            services.AddLogging(builder =>
            {
                builder.AddConfiguration(config.GetSection("Logging"));
                builder.AddDebug();
            }).Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Debug);

            // Add services
            services.AddBackblazeAgent(config.GetSection("Agent"));

            services.AddSingleton<Storage>();
        }

        #endregion

        [TestMethod]
        public void Services_Initialized()
        {
            Assert.IsNotNull(_storage);
        }

        [TestMethod]
        public async Task Parallel_Uploads()
        {
            var parallelTasks = new List<Task>();
            var source = new DirectoryInfo("C:/TestSrc");
            var files = source.EnumerateFiles("*.*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                parallelTasks.Add(Task.Run(async () =>
                {
                    await _storage.Agent.Files.UploadAsync(_bucketId, file.FullName, file.FullName, null);
                }));
            }
            await Task.WhenAll(parallelTasks);
        }

        [TestMethod]
        public async Task Parallel_Downloads()
        {
            var parallelTasks = new List<Task>();

            var results = await _storage.Agent.Files.GetNamesAsync(_bucketId, "C:/TestSrc", null, null, 10000);

            foreach (var file in results.Response.Files)
            {
                parallelTasks.Add(Task.Run(async () =>
                {
                    await _storage.Agent.Files.DownloadAsync(BucketName, file.FileName, file.FileName, null);
                }));
            }
            await Task.WhenAll(parallelTasks);
        }

        [TestMethod]
        public async Task Get_First_Bucket()
        {
            var bucket = await _storage.Agent.Buckets.FirstAsync(x => x.BucketName == BucketName);
            Assert.AreEqual(BucketName, bucket.BucketName);

            bucket = await _storage.Agent.Buckets.FirstAsync();
            Assert.AreEqual(BucketName, bucket.BucketName);
        }

        [TestMethod]
        public async Task Upload_And_Download_Stream()
        {
            // Upload stream
            var uploadResults = await _storage.Agent.UploadAsync(_bucketId, SmallStreamName, SmallStream);
            Assert.AreEqual(typeof(UploadFileResponse), uploadResults.Response.GetType());

            var uploadResults2 = await _storage.Agent.UploadAsync(_bucketId, SmallStreamName, SmallStream);
            Assert.AreEqual(typeof(UploadFileResponse), uploadResults2.Response.GetType());

            var uploadResults3 = await _storage.Agent.UploadAsync(_bucketId, SmallStreamName, SmallStream);
            Assert.AreEqual(typeof(UploadFileResponse), uploadResults3.Response.GetType());

            var uploadResults4 = await _storage.Agent.UploadAsync(_bucketId, SmallStreamName, SmallStream);
            Assert.AreEqual(typeof(UploadFileResponse), uploadResults4.Response.GetType());

            var uploadResults5 = await _storage.Agent.UploadAsync(_bucketId, SmallStreamName, SmallStream);
            Assert.AreEqual(typeof(UploadFileResponse), uploadResults5.Response.GetType());

            var uploadResults6 = await _storage.Agent.UploadAsync(_bucketId, SmallStreamName, SmallStream);
            Assert.AreEqual(typeof(UploadFileResponse), uploadResults6.Response.GetType());

            // Download stream
            //var download = new MemoryStream();
            //var downloadResults = await _storage.Agent.DownloadByIdAsync(_fileId, download);
            //Assert.AreEqual(typeof(DownloadFileResponse), downloadResults.Response.GetType());
            //Assert.AreEqual(SmallStream.Length, download.Length);
            ////Assert.AreEqual(SmallStream.ToSha1(), download.ToSha1());
        }

        [TestMethod]
        public async Task Create_And_Delete_Key()
        {
            // Create key
            var capabilities = new string[] { "listBuckets", "listFiles", "readFiles", "shareFiles", "writeFiles", "deleteFiles" };
            var keyName = $"{Guid.NewGuid().ToString()}";
            var createResults = await _storage.Agent.Keys.CreateAsync(capabilities, keyName);
            Assert.AreEqual(typeof(CreateKeyResponse), createResults.Response.GetType());
            Assert.AreEqual(keyName, createResults.Response.KeyName);

            // Delete key
            var deleteResults = await _storage.Agent.Keys.DeleteAsync(createResults.Response.ApplicationKeyId);
            Assert.AreEqual(typeof(DeleteKeyResponse), deleteResults.Response.GetType());
            Assert.AreEqual(keyName, createResults.Response.KeyName);
        }

        [TestMethod]
        public async Task List_Keys()
        {
            var results = await _storage.Agent.Keys.GetAsync();
            Assert.AreEqual(typeof(ListKeysResponse), results.Response.GetType());
            Assert.IsTrue(results.Response.Keys.Count >= 1, "The actual count was not greater than one");
        }

        [TestMethod]
        public async Task Create_And_Delete_Bucket_With_Rules()
        {
            // Create bucket
            var bucketName = $"{Guid.NewGuid().ToString()}";
            var request = new CreateBucketRequest(_accountId, bucketName, BucketType.AllPrivate);
            request.BucketInfo = new BucketInfo
                {
                    { "key1", "value1" },
                    { "key2", "value2" }
                };
            request.LifecycleRules = new LifecycleRules()
                {   new LifecycleRule()
                    {
                        DaysFromHidingToDeleting = 6,
                        DaysFromUploadingToHiding = 5,
                        FileNamePrefix = "backup/",
                    },
                    new LifecycleRule()
                    {
                        DaysFromHidingToDeleting = 45,
                        DaysFromUploadingToHiding = 7,
                        FileNamePrefix = "files/",
                    },
                };
            request.CorsRules = new CorsRules
                { new CorsRule()
                    {
                        CorsRuleName = "downloadFromAnyOrigin",
                        AllowedOrigins = new List<string> { "https" },
                        AllowedHeaders = new List<string> {"range" },
                        AllowedOperations = new List<string> { "b2_download_file_by_id" , "b2_download_file_by_name" },
                        ExposeHeaders = new List<string> {"x-bz-content-sha1" },
                        MaxAgeSeconds = 3600
                    }
                };
            var createResults = await _storage.Agent.Buckets.CreateAsync(request);
            Assert.AreEqual(typeof(CreateBucketResponse), createResults.Response.GetType());
            Assert.AreEqual(bucketName, createResults.Response.BucketName);
            Assert.AreEqual(request.BucketInfo.Count, createResults.Response.BucketInfo.Count);
            Assert.AreEqual(request.LifecycleRules.Count, createResults.Response.LifecycleRules.Count);
            Assert.AreEqual(request.CorsRules.Count, createResults.Response.CorsRules.Count);

            // Delete bucket
            var deleteResults = await _storage.Agent.Buckets.DeleteAsync(createResults.Response.BucketId);
            Assert.AreEqual(typeof(DeleteBucketResponse), deleteResults.Response.GetType());
            Assert.AreEqual(bucketName, deleteResults.Response.BucketName);
        }

        [TestMethod]
        public async Task Create_Update_And_Delete_Bucket()
        {
            // Create bucket
            var bucketName = $"{Guid.NewGuid().ToString()}";
            var createResults = await _storage.Agent.Buckets.CreateAsync(bucketName, BucketType.AllPublic);
            Assert.AreEqual(typeof(CreateBucketResponse), createResults.Response.GetType());
            Assert.AreEqual(bucketName, createResults.Response.BucketName);

            // Update bucket
            var updateResults = await _storage.Agent.Buckets.UpdateAsync(createResults.Response.BucketId, BucketType.AllPrivate);
            Assert.AreEqual(typeof(UpdateBucketResponse), updateResults.Response.GetType());
            Assert.AreEqual(bucketName, updateResults.Response.BucketName);
            Assert.AreEqual(BucketType.AllPrivate, updateResults.Response.BucketType);

            // Delete bucket
            var deleteResults = await _storage.Agent.Buckets.DeleteAsync(createResults.Response.BucketId);
            Assert.AreEqual(typeof(DeleteBucketResponse), deleteResults.Response.GetType());
            Assert.AreEqual(bucketName, deleteResults.Response.BucketName);
        }

        [TestMethod]
        public async Task List_Buckets()
        {
            var results = await _storage.Agent.Buckets.GetAsync();
            Assert.AreEqual(typeof(ListBucketsResponse), results.Response.GetType());
            Assert.IsTrue(results.Response.Buckets.Count >= 1, "The actual count was not greater than one");
        }

        [TestMethod]
        public async Task Upload_Hide_And_Delete_File()
        {
            var streamName = "c:/test/hidefile.bin";

            // Upload test file
            var fileResults = await _storage.Agent.UploadAsync(_bucketId, streamName, SmallStream, null);
            Assert.AreEqual(typeof(UploadFileResponse), fileResults.Response.GetType());
            Assert.AreEqual(SmallStreamSize, fileResults.Response.ContentLength, "The file size did not match");
            Assert.AreEqual(streamName, fileResults.Response.FileName, "The file name did not match");

            // Check before hideing
            var resultsCheck1 = await _storage.Agent.Files.GetNamesAsync(_bucketId);

            // Hide test file
            var hideResults = await _storage.Agent.Files.HideAsync(_bucketId, streamName);
            Assert.AreEqual(typeof(HideFileResponse), hideResults.Response.GetType());

            // Check it's hidden
            var resultsCheck2 = await _storage.Agent.Files.GetNamesAsync(_bucketId);
            Assert.IsTrue(resultsCheck1.Response.Files.Count > resultsCheck2.Response.Files.Count);

            // Delete test file
            var deleteResutls = await _storage.Agent.Files.DeleteAsync(fileResults.Response.FileId, streamName);
            Assert.AreEqual(typeof(DeleteFileVersionResponse), deleteResutls.Response.GetType());
            Assert.AreEqual(streamName, deleteResutls.Response.FileName, "The file name did not match");
        }

        [TestMethod]
        public async Task List_File_Names()
        {
            var results = await _storage.Agent.Files.GetNamesAsync(_bucketId);
            Assert.AreEqual(typeof(ListFileNamesResponse), results.Response.GetType());
            Assert.IsTrue(results.Response.Files.Count >= 1, "The actual count was not greater than one");
        }

        [TestMethod]
        public async Task List_File_Versions()
        {
            var results = await _storage.Agent.Files.GetVersionsAsync(_bucketId);
            Assert.AreEqual(typeof(ListFileVersionResponse), results.Response.GetType());
            Assert.IsTrue(results.Response.Files.Count >= 1, "The actual count was not greater than one");
        }

        [TestMethod]
        public async Task Get_File_Info()
        {
            var results = await _storage.Agent.Files.GetInfoAsync(_fileId);
            Assert.AreEqual(typeof(GetFileInfoResponse), results.Response.GetType());
            Assert.AreEqual(SmallStreamSize, results.Response.ContentLength, "The file size did not match");
            Assert.AreEqual(SmallStreamName, results.Response.FileName, "The file name did not match");
        }
    }

    public static class AgentExtensions
    {
        public static Task ForEachAsync<T>(this IEnumerable<T> sequence, Func<T, Task> action)
        {
            return Task.WhenAll(sequence.Select(action));
        }
    }
}
