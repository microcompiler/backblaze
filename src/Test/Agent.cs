using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Bytewizer.Backblaze;
using Bytewizer.Backblaze.Storage;
using Bytewizer.Backblaze.Models;
using System.Security.Cryptography;
using System.Threading;
using System.IO.Compression;
using System.Diagnostics;

namespace Backblaze
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
            var logger = serviceProvider.GetService<ILogger<Agent>>();

            logger.LogDebug("Woo Hooo");

            // Run application
            _storage = serviceProvider.GetService<Storage>();

            // Set account id
            _accountId = _storage.Agent.AccountId;

            // Initialize test bucket
            InitializeAsync().GetAwaiter().GetResult();

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

            // Add memory cache
            services.AddMemoryCache();

            // Add services
            services.AddBackblazeAgent(config.GetSection("Agent"));

            services.AddSingleton<Storage>();
        }

        private async Task InitializeAsync()
        {
            // Get test bucket
            var testBucket = await _storage.Agent.Buckets.FindByNameAsync(BucketName);

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
                var capabilities = new Capabilities { Capability.ListBuckets, Capability.ListFiles, Capability.ReadFiles, Capability.ShareFiles, Capability.WriteFiles, Capability.DeleteFiles };
                var keyResults = await _storage.Agent.Keys.CreateAsync(KeyName, capabilities);
            }
            else
            {
                // Set bucket id
                _bucketId = testBucket.BucketId;

                // Set file id
                var request = new ListFileNamesRequest(_bucketId);
                var file = await _storage.Agent.Files.FirstAsync(request, x => x.FileName == SmallStreamName);
                _fileId = file.FileId;
            }

            //Assert.IsNotNull(_bucketId);
            //Assert.IsNotNull(_fileId);
        }

        #endregion

        [TestMethod]
        public void Services_Initialized()
        {
            Assert.IsNotNull(_storage);
        }

        [TestMethod]
        public async Task Copy_File()
        {

            var results = await _storage.Agent.Files.CopyAsync(_fileId, "copyfile.bin");
            Assert.AreEqual(typeof(CopyFileResponse), results.Response.GetType());
            Assert.AreEqual(SmallStreamSize, results.Response.ContentLength, "The file size did not match");
            Assert.AreEqual("copyfile.bin", results.Response.FileName, "The file name did not match");

            var range = new System.Net.Http.Headers.RangeHeaderValue(1, 1000);
            Debug.WriteLine(range.ToString());

            var request = new CopyFileRequest(_fileId, "copyfile2.bin");
            request.Range = range;
            var results2 = await _storage.Agent.Files.CopyAsync(request);
        }

        [TestMethod]
        public async Task Bucket_Find_By_Id()
        {
            var results = await _storage.Agent.Buckets.FindByIdAsync(_bucketId);
            Assert.IsNotNull(results);
        }

        [TestMethod]
        public async Task Bucket_Find_By_Name()
        {
            var results = await _storage.Agent.Buckets.FindByNameAsync(BucketName);
            Assert.IsNotNull(results);
        }

        [TestMethod]
        public async Task Copy_Directory()
        {
            var results = await _storage.Agent.Directories.CopyToAsync(_bucketId, @"C:\SourceFolder", "*.*", SearchOption.AllDirectories);
        }


        //[TestMethod]
        //public async Task Copy_Part()
        //{

            //}

            //[TestMethod]
            //public async Task Parallel_Uploads()
            //{
            //    var source = new DirectoryInfo("C:/TestSrc");
            //    var files = source.EnumerateFiles("*.*", SearchOption.AllDirectories);

            //    await _storage.Agent.Directories.CopyToAsync(files, _bucketId);

            //}

            //[TestMethod]
            //public async Task TestCase5()
            //{
            //    foreach (var filepath in Directory.GetFiles(@"c:\my\directory"))
            //    {
            //        using (var stream = File.OpenRead(filepath))
            //            await _storage.Agent.UploadAsync(new UploadFileByBucketIdRequest(_bucketId, new System.IO.FileInfo(filepath).Name), stream);
            //    }
            //}

            //[TestMethod]
            //public async Task Parallel_Downloads()
            //{
            //    await _storage.Agent.Directories.CopyFromAsync(_bucketId, "C:/TestSrc");
            //}

            //[TestMethod]
            //public async Task Tester()
            //{
            //    await _storage.Agent.Directories.Tester();
            //}

            //[TestMethod]
            //public async Task Parallel_Downloads()
            //{
            //    var parallelTasks = new List<Task>();

            //    var results = await _storage.Agent.Files.GetNamesAsync(_bucketId, "C:/TestSrc", null, null, 10000);

            //    foreach (var file in results.Response.Files)
            //    {
            //        parallelTasks.Add(Task.Run(async () =>
            //        {
            //            await _storage.Agent.Files.DownloadAsync(BucketName, file.FileName, file.FileName, null);
            //        }));
            //    }
            //    await Task.WhenAll(parallelTasks);
            //}

        [TestMethod]
        public async Task Get_First_Bucket()
        {
            var buckets = await _storage.Agent.Buckets.GetAsync();
            var bucketName = buckets.ToList().First(x => x.BucketName == BucketName);
            Assert.AreEqual(BucketName, bucketName.BucketName);

            //bucket = await _storage.Agent.Buckets.ListAllAsync();
            //Assert.AreEqual(BucketName, bucket.BucketName);
        }

        [TestMethod]
        public async Task Get_First_File()
        {
            var request = new ListFileNamesRequest(_bucketId);
            var file = await _storage.Agent.Files.FirstAsync(request, x => x.FileName == SmallStreamName); 

            Assert.AreEqual(SmallStreamName, file.FileName);
        }

        //[TestMethod]
        //public async Task Delete_All_Files_In_Bucket()
        //{
        //    //var request = new ListFileVersionRequest(_bucketId);
        //    //var files = await _storage.Agent.Files.DeleteAllAsync(request);

        //    //Assert.AreEqual(SmallStreamName, file.FileName);
        //}

        [TestMethod]
        public async Task Upload_And_Download_Stream()
        {
            // Upload stream
            var uploadResults = await _storage.Agent.UploadAsync(_bucketId, "c:/folder/file.bin", SmallStream);
            Assert.AreEqual(typeof(UploadFileResponse), uploadResults.Response.GetType());

            // Download stream
            var download = new MemoryStream();
            var downloadResults = await _storage.Agent.DownloadByIdAsync(_fileId, download);
            Assert.AreEqual(typeof(DownloadFileResponse), downloadResults.Response.GetType());
            Assert.AreEqual(SmallStream.Length, download.Length);
            //Assert.AreEqual(SmallStream.ToSha1(), download.ToSha1());
        }

        //[TestMethod]
        //public async Task Upload_Encrypted_Stream()
        //{
            //using (var aes = new AesCryptoServiceProvider())
            //{
            //    ICryptoTransform encryptor = aes.CreateEncryptor();
            //    ICryptoTransform decryptor = aes.CreateDecryptor();

            //    var outputFileStream = new MemoryStream();
            //    var inputFileStream = new FileStream("c:/copyto/tester.json", FileMode.Open, FileAccess.Read);
            //    var cryptoStream = new CryptoStream(outputFileStream, encryptor, CryptoStreamMode.Write);
            //    var gZipStream = new GZipStream(cryptoStream, CompressionMode.Compress);

            //    inputFileStream.CopyTo(gZipStream);

            //    var uploadResults = await _storage.Agent.UploadAsync(_bucketId, "c:/copyto/tester.bin", cryptoStream);


            //    //using (var inputFileStream = new FileStream("c:/copyto/tester.bin", FileMode.Open, FileAccess.Read))
            //    //{
            //    //    using (var outputFileStream = new FileStream("c:/copyto/tester-decrypted.json", FileMode.Create, FileAccess.Write))
            //    //    {
            //    //        DecryptThenDecompress(inputFileStream, outputFileStream, decryptor);
            //    //    }
            //    //}
            //}
        //}

        private static void CompressThenEncrypt(Stream inputFileStream, Stream outputFileStream, ICryptoTransform encryptor)
        {
            using (var cryptoStream = new CryptoStream(outputFileStream, encryptor, CryptoStreamMode.Write))
            using (var gZipStream = new GZipStream(cryptoStream, CompressionMode.Compress))
            {
                inputFileStream.CopyTo(gZipStream);
            }
        }

        private static void DecryptThenDecompress(Stream inputFileStream, Stream outputFileStream, ICryptoTransform decryptor)
        {
            using (var cryptoStream = new CryptoStream(inputFileStream, decryptor, CryptoStreamMode.Read))
            {
                using (var gZipStream = new GZipStream(cryptoStream, CompressionMode.Decompress))
                {
                    gZipStream.CopyTo(outputFileStream);
                }
            }
        }

        [TestMethod]
        public async Task Create_And_Delete_Key()
        {
            // Create key
            var capabilities = new Capabilities { Capability.ListBuckets, Capability.ListBuckets, Capability.ListFiles, Capability.ReadFiles, Capability.ShareFiles, Capability.WriteFiles, Capability.DeleteFiles };
            var keyName = $"{Guid.NewGuid().ToString()}";
            var createResults = await _storage.Agent.Keys.CreateAsync(keyName, capabilities);
            Assert.AreEqual(typeof(CreateKeyResponse), createResults.Response.GetType());
            Assert.AreEqual(keyName, createResults.Response.KeyName);
            // Assert.AreEqual(capabilities, createResults.Response.Capabilities);

            Assert.IsTrue(createResults.Response.Capabilities.Equals(capabilities));
            CollectionAssert.AreEqual(createResults.Response.Capabilities.ToList(), capabilities.ToList());

            // Delete key
            var deleteResults = await _storage.Agent.Keys.DeleteAsync(createResults.Response.ApplicationKeyId);
            Assert.AreEqual(typeof(DeleteKeyResponse), deleteResults.Response.GetType());
            Assert.AreEqual(keyName, createResults.Response.KeyName);
        }

        [TestMethod]
        public async Task List_Keys()
        {
            var results = await _storage.Agent.Keys.ListAsync();
            Assert.AreEqual(typeof(ListKeysResponse), results.Response.GetType());
            Assert.IsTrue(results.Response.Keys.Count >= 1, "The actual count was not greater than one");

            var filelist2 = await _storage.Agent.Keys.GetAsync(new ListKeysRequest(_accountId) { MaxKeyCount = 5 }, TimeSpan.Zero);

            foreach (var file in filelist2)
            {
                Debug.WriteLine(file.KeyName);
            }
        }

        [TestMethod]
        public async Task List_Parts()
        {
            var results = await _storage.Agent.Parts.GetAsync(new ListPartsRequest(_fileId), TimeSpan.FromSeconds(60));
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
                { new CorsRule(
                     "downloadFromAnyOrigin",
                     new List<string> { "https" },
                     new List<string> { "b2_download_file_by_id" , "b2_download_file_by_name" },
                     3600 )
                    {
                        AllowedHeaders = new List<string> {"range" },
                        ExposeHeaders = new List<string> {"x-bz-content-sha1" },               
                    }
                };
            

            var createResults = await _storage.Agent.Buckets.CreateAsync(request);

            Assert.AreEqual(typeof(CreateBucketResponse), createResults.Response.GetType());
            Assert.AreEqual(bucketName, createResults.Response.BucketName);
            Assert.AreEqual(request.BucketInfo.Count, createResults.Response.BucketInfo.Count);
            Assert.AreEqual(request.LifecycleRules.Count, createResults.Response.LifecycleRules.Count);
            Assert.AreEqual(request.CorsRules.Count, createResults.Response.CorsRules.Count);

            Assert.IsTrue(createResults.Response.CorsRules.Equals(request.CorsRules));
            CollectionAssert.AreEqual(request.CorsRules.ToList(), createResults.Response.CorsRules.ToList());

            Assert.IsTrue(createResults.Response.LifecycleRules.Equals(request.LifecycleRules));
            CollectionAssert.AreEqual(request.LifecycleRules.ToList(), createResults.Response.LifecycleRules.ToList());

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
        public async Task FileNames_Iterator()
        {
            var request = new ListFileNamesRequest(_bucketId) { MaxFileCount = 10 };
            var filelist = await _storage.Agent.Files.GetAsync(request, TimeSpan.FromSeconds(10));

            Debug.WriteLine("First Run");
            foreach (var file in filelist)
            {
                //Debug.WriteLine(file.FileName);
            }

            var filelist2 = await _storage.Agent.Files.GetAsync(request, TimeSpan.FromSeconds(10));
            Debug.WriteLine("Second Run");
            foreach (var file in filelist2)
            {
                //Debug.WriteLine(file.FileName);
            }

            
            

        }

        [TestMethod]
        public async Task FileVersions_Iterator()
        {
            var request = new ListFileVersionRequest(_bucketId);
            var filelist = await _storage.Agent.Files.GetAsync(request, TimeSpan.FromSeconds(10));

            foreach (var file in filelist)
            {
                //Debug.WriteLine(file.FileName);
            }

            var filelist2 = await _storage.Agent.Files.GetAsync(request, TimeSpan.FromSeconds(10));

            foreach (var file in filelist2)
            {
                //Debug.WriteLine(file.FileName);
            }
        }

        [TestMethod]
        public async Task UnfinishedLargeFiles_Iterator()
        {
            var request = new ListUnfinishedLargeFilesRequest(_bucketId);
            var filelist = await _storage.Agent.Files.GetAsync(request, TimeSpan.FromSeconds(10));

            foreach (var file in filelist)
            {
                Debug.WriteLine(file.FileName);
            }

            var filelist2 = await _storage.Agent.Files.GetAsync(request, TimeSpan.FromSeconds(10));

            foreach (var file in filelist2)
            {
                Debug.WriteLine(file.FileName);
            }
        }

        [TestMethod]
        public async Task Copy_Tester()
        {
            //var results = await _storage.Agent.Directories.CopyToAsync(_bucketId, @"C:\Python27", "*.*", SearchOption.AllDirectories);


        }

        [TestMethod]
        public async Task List_Buckets()
        {
            var results = await _storage.Agent.Buckets.ListAsync();
            Assert.AreEqual(typeof(ListBucketsResponse), results.Response.GetType());
            Assert.IsTrue(results.Response.Buckets.Count >= 1, "The actual count was not greater than one");
        }

        [TestMethod]
        public async Task Upload_Hide_And_Delete_File()
        {
            var streamName = "c:/test/hidefile.bin";

            // Upload test file
            var fileResults = await _storage.Agent.UploadAsync(_bucketId, streamName, SmallStream);
            Assert.AreEqual(typeof(UploadFileResponse), fileResults.Response.GetType());
            Assert.AreEqual(SmallStreamSize, fileResults.Response.ContentLength, "The file size did not match");
            Assert.AreEqual(streamName, fileResults.Response.FileName, "The file name did not match");

            // Check before hideing
            var resultsCheck1 = await _storage.Agent.Files.ListNamesAsync(_bucketId);

            // Hide test file
            var hideResults = await _storage.Agent.Files.HideAsync(_bucketId, streamName);
            Assert.AreEqual(typeof(HideFileResponse), hideResults.Response.GetType());

            // Check it's hidden
            var resultsCheck2 = await _storage.Agent.Files.ListNamesAsync(_bucketId);
            Assert.IsTrue(resultsCheck1.Response.Files.Count > resultsCheck2.Response.Files.Count);

            // Delete test file
            var deleteResutls = await _storage.Agent.Files.DeleteAsync(fileResults.Response.FileId, streamName);
            Assert.AreEqual(typeof(DeleteFileVersionResponse), deleteResutls.Response.GetType());
            Assert.AreEqual(streamName, deleteResutls.Response.FileName, "The file name did not match");
        }

        [TestMethod]
        public async Task List_File_Names()
        {
            var results = await _storage.Agent.Files.ListNamesAsync(_bucketId);
            Assert.AreEqual(typeof(ListFileNamesResponse), results.Response.GetType());
            Assert.IsTrue(results.Response.Files.Count >= 1, "The actual count was not greater than one");
        }

        [TestMethod]
        public async Task List_File_Versions()
        {
            var results = await _storage.Agent.Files.ListVersionsAsync(_bucketId);
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
