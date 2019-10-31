using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Collections.Generic;

using System.IO.Abstractions.TestingHelpers;

using Bytewizer.Backblaze.Client;
using Bytewizer.Backblaze.Models;
using Bytewizer.Backblaze.Extensions;
using Bytewizer.Backblaze.Enumerables;

using Xunit;
using System.Diagnostics;

namespace Backblaze.Tests.Integration
{
    public class EndpointTest : BaseFixture
    {
        private static readonly string _bucketName = $"{Guid.NewGuid().ToString()}";
        private static string _bucketId;

        private static readonly string _keyName = $"{Guid.NewGuid().ToString()}";
        private static string _keyId;

        public EndpointTest(StorageClientFixture fixture) 
            : base(fixture)
        { }

        [Fact, TestPriority(1)]
        public async Task Buckets_CreateAsync()
        {
            // Create bucket
            var request = new CreateBucketRequest(Storage.AccountId, _bucketName, BucketType.AllPrivate)
            {
                BucketInfo = new BucketInfo()
                {
                    { "key1", "value1" },
                    { "key2", "value2" }
                },
                CacheControl = new CacheControlHeaderValue()
                {
                    MaxAge = TimeSpan.FromSeconds(120)
                },
                LifecycleRules = new LifecycleRules()
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
                },
                CorsRules = new CorsRules
                { new CorsRule(
                     "downloadFromAnyOrigin",
                     new List<string> { "https" },
                     new List<string> { "b2_download_file_by_id" , "b2_download_file_by_name" },
                     3600)
                    {
                        AllowedHeaders = new List<string> { "range" },
                        ExposeHeaders = new List<string> { "x-bz-content-sha1" },
                    }
                }
            };

            var results = await Storage.Buckets.CreateAsync(request);
            results.EnsureSuccessStatusCode();

            _bucketId = results.Response.BucketId;

            Assert.Equal(typeof(CreateBucketResponse), results.Response.GetType());
            Assert.Equal(_bucketName, results.Response.BucketName);
            Assert.Equal(request.BucketInfo.Count, results.Response.BucketInfo.Count);
            Assert.Equal(request.LifecycleRules.Count, results.Response.LifecycleRules.Count);
            Assert.Equal(request.CorsRules.Count, results.Response.CorsRules.Count);

            Assert.True(results.Response.CorsRules.Equals(request.CorsRules));
            Assert.Equal(request.CorsRules.ToList(), results.Response.CorsRules.ToList());

            Assert.True(results.Response.LifecycleRules.Equals(request.LifecycleRules));
            Assert.Equal(request.LifecycleRules.ToList(), results.Response.LifecycleRules.ToList());
        }

        [Fact, TestPriority(2)]
        public async Task Buckets_FindByIdAsync()
        {
            var results = await Storage.Buckets.FindByIdAsync(_bucketId);

            Assert.Equal(typeof(BucketItem), results.GetType());
            Assert.Equal(_bucketName, results.BucketName);
        }

        [Fact, TestPriority(2)]
        public async Task Buckets_FindByNameAsync()
        {
            var results = await Storage.Buckets.FindByNameAsync(_bucketName);

            Assert.Equal(typeof(BucketItem), results.GetType());
            Assert.Equal(_bucketId, results.BucketId);
        }

        [Fact, TestPriority(2)]
        public async Task Buckets_ListAsync()
        {
            var results = await Storage.Buckets.ListAsync();
            results.EnsureSuccessStatusCode();

            Assert.Equal(typeof(ListBucketsResponse), results.Response.GetType());
            Assert.True(results.Response.Buckets.Count >= 1, "The actual count was not greater than one");
        }

        [Fact, TestPriority(2)]
        public async Task Buckets_GetAsync()
        {
            var results = await Storage.Buckets.GetAsync();

            Assert.Equal(typeof(List<BucketItem>), results.GetType());
            Assert.True(results.ToList().Count() >= 1, "The actual count was not greater than one");
        }

        [Fact, TestPriority(2)]
        public async Task Buckets_UpdateAsync()
        {
            var results = await Storage.Buckets.UpdateAsync(_bucketId, BucketType.AllPrivate);

            Assert.Equal(typeof(UpdateBucketResponse), results.Response.GetType());
            Assert.Equal(_bucketId, results.Response.BucketId);
            Assert.Equal(BucketType.AllPrivate, results.Response.BucketType);
        }

        [Fact, TestPriority(3)]
        public async Task Buckets_DeleteAsync()
        {
            var results = await Storage.Buckets.DeleteAsync(_bucketId);

            Assert.Equal(typeof(DeleteBucketResponse), results.Response.GetType());
            Assert.Equal(_bucketId, results.Response.BucketId);
            Assert.Equal(BucketType.AllPrivate, results.Response.BucketType);
        }

        [Fact, TestPriority(1)]
        public async Task Keys_CreateAsync()
        {
            var request = new CreateKeyRequest(Storage.AccountId, _keyName, Capabilities.ReadOnly())
            {
                ValidDurationInSeconds = (ulong)DateTime.Now.AddDays(5).Second
            };
            var results = await Storage.Keys.CreateAsync(request);

            _keyId = results.Response.ApplicationKeyId;

            Assert.Equal(typeof(CreateKeyResponse), results.Response.GetType());
            //Assert.Equal(DateTime.Now.AddDays(5).Date, results.Response.ExpirationTimestamp.Date);
            Assert.Equal(Capabilities.ReadOnly(), results.Response.Capabilities);
        }

        [Fact, TestPriority(2)]
        public async Task Keys_FindByNameAsync()
        {
            var results = await Storage.Keys.FindByNameAsync(_keyName);

            Assert.Equal(typeof(KeyItem), results.GetType());
            Assert.Equal(_keyId, results.ApplicationKeyId);
            Assert.Equal(Capabilities.ReadOnly(), results.Capabilities);
        }

        [Fact, TestPriority(2)]
        public async Task Keys_FindByIdAsync()
        {
            var results = await Storage.Keys.FindByIdAsync(_keyId);

            Assert.Equal(typeof(KeyItem), results.GetType());
            Assert.Equal(_keyName, results.KeyName);
            Assert.Equal(Capabilities.ReadOnly(), results.Capabilities);
        }

        [Fact, TestPriority(2)]
        public async Task Keys_Enumerable()
        {
            var request = new ListKeysRequest(Storage.AccountId);
            var enumerable = await Storage.Keys.GetEnumerableAsync(request);

            Assert.Equal(typeof(KeyEnumerable), enumerable.GetType());
            Assert.True(enumerable.ToList().Count() >= 1, "The actual count was not greater than one");
        }

        [Fact, TestPriority(2)]
        public async Task Keys_ListAsync()
        {
            var results = await Storage.Keys.ListAsync();

            Assert.Equal(typeof(ListKeysResponse), results.Response.GetType());
            Assert.True(results.Response.Keys.Count >= 1, "The actual count was not greater than one");
        }

        [Fact, TestPriority(2)]
        public async Task Keys_GetAsync()
        {
            var results = await Storage.Keys.GetAsync();

            Assert.Equal(typeof(List<KeyItem>), results.GetType());
            Assert.True(results.ToList().Count() >= 1, "The actual count was not greater than one");
        }

        [Fact, TestPriority(3)]
        public async Task Keys_DeleteAsync()
        {
            var results = await Storage.Keys.DeleteAsync(_keyId);

            Assert.Equal(typeof(DeleteKeyResponse), results.Response.GetType());
            Assert.Equal(Capabilities.ReadOnly(), results.Response.Capabilities);
        }

        [Fact, TestPriority(1)]
        public async Task Files_UploadAsync()
        {
            var response = new List<UploadFileResponse>();
            var files = FileSystem.Directory.GetFiles(@"c:\", "*.*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                using (var content = FileSystem.File.OpenRead(file))
                {
                    var request = new UploadFileByBucketIdRequest(BucketId, file)
                    {
                        LastModified = FileSystem.File.GetLastWriteTime(file)
                    };
                    var results = await Storage.UploadAsync(request, content, null, CancellationToken.None);
                    if (results.IsSuccessStatusCode)
                    {
                        var fileSha1 = FileSystem.File.OpenRead(file).ToSha1();
                        if (!fileSha1.Equals(results.Response.ContentSha1))
                            throw new InvalidOperationException();

                        response.Add(results.Response);
                    }
                }
            }

            Assert.Equal(files.Count(), response.Count());
        }

        [Fact, TestPriority(2)]
        public async Task Files_FileNameEnumerable()
        {
            var request = new ListFileNamesRequest(BucketId);
            var enumerable = await Storage.Files.GetEnumerableAsync(request);

            Assert.Equal(typeof(FileNameEnumerable), enumerable.GetType());
            Assert.True(enumerable.ToList().Count() >= 1, "The actual count was not greater than one");
        }

        [Fact, TestPriority(2)]
        public async Task Files_FileVersionEnumerable()
        {
            var request = new ListFileVersionRequest(BucketId);
            var enumerable = await Storage.Files.GetEnumerableAsync(request);

            Assert.Equal(typeof(FileVersionEnumerable), enumerable.GetType());
            Assert.True(enumerable.ToList().Count() >= 1, "The actual count was not greater than one");
        }

        [Fact, TestPriority(2)]
        public async Task Files_ListNamesAsync()
        {
            var response = new List<ListFileNamesResponse>();

            var results = await Storage.Files.ListNamesAsync(BucketId);
            foreach (var fileList in results.Response.Files)
            {
                if (results.IsSuccessStatusCode)
                {
                    var fileSha1 = FileSystem.File.OpenRead(fileList.FileName).ToSha1();
                    if (!fileSha1.Equals(fileList.ContentSha1))
                        throw new InvalidOperationException();

                    response.Add(results.Response);
                }
            }

            Assert.Equal(3, response.Count());
        }

        [Fact, TestPriority(2)]
        public async Task Files_ListVersionsAsync()
        {
            var response = new List<ListFileVersionResponse>();

            var results = await Storage.Files.ListVersionsAsync(BucketId);
            foreach (var fileList in results.Response.Files)
            {
                if (results.IsSuccessStatusCode)
                {
                    response.Add(results.Response);
                }
            }

            Assert.Equal(3, response.Count());
        }

        [Fact, TestPriority(3)]
        public async Task Files_ListFileInfoAsync()
        {
            var response = new List<GetFileInfoResponse>();

            var files = await Storage.Files.ListNamesAsync(BucketId);
            files.EnsureSuccessStatusCode();

            foreach (var file in files.Response.Files)
            {
                var results = await Storage.Files.GetInfoAsync(file.FileId);
                if (results.IsSuccessStatusCode)
                {
                    if (!file.ContentLength.Equals(results.Response.ContentLength))
                        throw new InvalidOperationException();

                    if (!file.ContentSha1.Equals(results.Response.ContentSha1))
                        throw new InvalidOperationException();

                    if (!file.ContentType.Equals(results.Response.ContentType))
                        throw new InvalidOperationException();

                    if (!file.FileInfo.Equals(results.Response.FileInfo))
                        throw new InvalidOperationException();

                    if (!file.FileName.Equals(results.Response.FileName))
                        throw new InvalidOperationException();

                    if (!file.UploadTimestamp.Equals(results.Response.UploadTimestamp))
                        throw new InvalidOperationException();

                    response.Add(results.Response);
                }
            }

            Assert.Equal(files.Response.Files.Count(), response.Count());
        }

        [Fact, TestPriority(4)]
        public async Task Files_DownloadAsync()
        {
            var response = new List<DownloadFileResponse>();
            var fileSystem = new MockFileSystem();
            var files = FileSystem.Directory.GetFiles(@"c:\", "*.*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                if (!fileSystem.Directory.Exists(Path.GetDirectoryName(file)))
                {
                    fileSystem.Directory.CreateDirectory(Path.GetDirectoryName(file));
                }

                using (var content = fileSystem.File.Create(file))
                {
                    var results = await Storage.DownloadAsync(BucketName, file, content);
                    if (results.IsSuccessStatusCode)
                    {
                        response.Add(results.Response);
                    }
                }

                var byte1 = FileSystem.File.ReadAllBytes(file);
                var byte2 = fileSystem.File.ReadAllBytes(file);

                if (!byte1.SequenceEqual(byte2))
                    throw new InvalidOperationException();
            }

            Assert.Equal(files.Count(), response.Count());
        }

        [Fact, TestPriority(5)]
        public async Task Files_CopyAsync()
        {
            var response = new List<CopyFileResponse>();

            var files = await Storage.Files.ListNamesAsync(BucketId);
            files.EnsureSuccessStatusCode();

            foreach (var file in files.Response.Files)
            {
                var path = Path.Combine(Path.GetDirectoryName(file.FileName), $"{Path.GetFileNameWithoutExtension(file.FileName)}-copy.bin");
                var results = await Storage.Files.CopyAsync(file.FileId, path);
                if (results.IsSuccessStatusCode)
                {
                    response.Add(results.Response);
                }
            }

            Assert.Equal(files.Response.Files.Count(), response.Count());
        }

        [Fact, TestPriority(10)]
        public async Task LargeFiles_UploadAsync()
        {
            var response = new List<UploadFileResponse>();
            var files = LargeFileSystem.Directory.GetFiles(@"c:\", "*.*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                using (var content = LargeFileSystem.File.OpenRead(file))
                {
                    var request = new UploadFileByBucketIdRequest(BucketId, file);
                    var results = await Storage.UploadAsync(request, content, null, CancellationToken.None);
                    if (results.IsSuccessStatusCode)
                    {
                        var fileSha1 = LargeFileSystem.File.OpenRead(file).ToSha1();
                        if (!fileSha1.Equals(results.Response.ContentSha1))
                            throw new InvalidOperationException();

                        response.Add(results.Response);
                    }
                }
            }

            Assert.Equal(files.Count(), response.Count());
        }

        [Fact, TestPriority(11)]
        public async Task LargeFiles_DownloadAsync()
        {
            var response = new List<DownloadFileResponse>();
            var fileSystem = new MockFileSystem();
            var files = LargeFileSystem.Directory.GetFiles(@"c:\", "*.*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                if (!fileSystem.Directory.Exists(Path.GetDirectoryName(file)))
                {
                    fileSystem.Directory.CreateDirectory(Path.GetDirectoryName(file));
                }

                using (var content = fileSystem.File.Create(file))
                {
                    var results = await Storage.DownloadAsync(BucketName, file, content);
                    if (results.IsSuccessStatusCode)
                    {
                        response.Add(results.Response);
                    }
                }

                var byte1 = LargeFileSystem.File.ReadAllBytes(file);
                var byte2 = fileSystem.File.ReadAllBytes(file);

                if (!byte1.SequenceEqual(byte2))
                    throw new InvalidOperationException();
            }

            Assert.Equal(files.Count(), response.Count());
        }

        [Fact, TestPriority(12)]
        public async Task LargeFiles_DownloadAsync_ById()
        {
            var response = new List<DownloadFileResponse>();
            var fileSystem = new MockFileSystem();

            var fileResults = await Storage.Files.ListNamesAsync(BucketId);
            fileResults.EnsureSuccessStatusCode();

            var files = fileResults.Response.Files.Where(x => x.FileName == "c:/six-megabyte.bin");

            foreach (var file in files)
            {
                if (!fileSystem.Directory.Exists(Path.GetDirectoryName(file.FileName)))
                {
                    fileSystem.Directory.CreateDirectory(Path.GetDirectoryName(file.FileName));
                }

                using (var content = fileSystem.File.Create(file.FileName))
                {
                    var results = await Storage.DownloadByIdAsync(file.FileId, content);
                    if (results.IsSuccessStatusCode)
                    {
                        response.Add(results.Response);
                    }
                }

                var byte1 = LargeFileSystem.File.ReadAllBytes(file.FileName);
                var byte2 = fileSystem.File.ReadAllBytes(file.FileName);

                if (!byte1.SequenceEqual(byte2))
                    throw new InvalidOperationException();
            }

            Assert.Equal(files.Count(), response.Count());
        }

        [Fact, TestPriority(100)]
        public async Task Files_DeleteAsync()
        {
            var response = new List<DeleteFileVersionResponse>();

            var files = await Storage.Files.ListNamesAsync(BucketId);
            files.EnsureSuccessStatusCode();

            foreach (var file in files.Response.Files)
            {
                var results = await Storage.Files.DeleteAsync(file.FileId, file.FileName);
                if (results.IsSuccessStatusCode)
                {
                    response.Add(results.Response);
                }
            }

            Assert.Equal(files.Response.Files.Count(), response.Count());
        }
    }
}
