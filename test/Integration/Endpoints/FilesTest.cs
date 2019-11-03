using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using System.IO.Abstractions.TestingHelpers;

using Bytewizer.Backblaze.Client;
using Bytewizer.Backblaze.Models;
using Bytewizer.Backblaze.Extensions;
using Bytewizer.Backblaze.Enumerables;

using Xunit;
using Bytewizer.Backblaze.Command;

namespace Backblaze.Tests.Integration
{
    public class FilesTest : BaseFixture
    {
        public FilesTest(StorageClientFixture fixture) 
            : base(fixture)
        { }

        [Fact, TestPriority(1)]
        public async Task UploadAsync()
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
        public async Task FileNameEnumerable()
        {
            var request = new ListFileNamesRequest(BucketId);
            var enumerable = await Storage.Files.GetEnumerableAsync(request);

            Assert.Equal(typeof(FileNameEnumerable), enumerable.GetType());
            Assert.True(enumerable.ToList().Count() >= 1, "The actual count was not greater than one");
        }

        [Fact, TestPriority(2)]
        public async Task FileVersionEnumerable()
        {
            var request = new ListFileVersionRequest(BucketId);
            var enumerable = await Storage.Files.GetEnumerableAsync(request);

            Assert.Equal(typeof(FileVersionEnumerable), enumerable.GetType());
            Assert.True(enumerable.ToList().Count() >= 1, "The actual count was not greater than one");
        }

        [Fact, TestPriority(2)]
        public async Task ListNamesAsync()
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
        public async Task ListVersionsAsync()
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

            Assert.Equal(4, response.Count());
        }

        [Fact, TestPriority(3)]
        public async Task ListFileInfoAsync()
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
        public async Task DownloadAsync()
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
        public async Task CopyAsync()
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

            int progressEventCounter = 0;
            long lastBytesTransferred = 0;
            double lastProgress = 0;
            var progress = new NaiveProgress<ICopyProgress>(x =>
            {
                progressEventCounter++;
                //Assert.True(x.BytesTransferred > lastBytesTransferred);
                lastBytesTransferred = x.BytesTransferred;
                lastProgress = x.PercentComplete;
            });
            
            foreach (var file in files)
            {
                if (!fileSystem.Directory.Exists(Path.GetDirectoryName(file)))
                {
                    fileSystem.Directory.CreateDirectory(Path.GetDirectoryName(file));
                }

                using (var content = fileSystem.File.Create(file))
                {
                    var request = new DownloadFileByNameRequest(BucketName, file);
                    var results = await Storage.DownloadAsync(request, content, progress,CancellationToken.None);
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

            Assert.True(progressEventCounter > 0);
            Assert.Equal(1, lastProgress);
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

        //[Fact, TestPriority(100)]
        //public async Task Files_DeleteAllAsync()
        //{
        //    var response = new List<UploadFileResponse>();
        //    var files = FileSystem.Directory.GetFiles(@"c:\", "*.*", SearchOption.AllDirectories);

        //    foreach (var file in files)
        //    {
        //        using (var content = FileSystem.File.OpenRead(file))
        //        {
        //            var request = new UploadFileByBucketIdRequest(BucketId, file)
        //            {
        //                LastModified = FileSystem.File.GetLastWriteTime(file)
        //            };
        //            var results = await Storage.UploadAsync(request, content, null, CancellationToken.None);
        //            if (results.IsSuccessStatusCode)
        //            {
        //                var fileSha1 = FileSystem.File.OpenRead(file).ToSha1();
        //                if (!fileSha1.Equals(results.Response.ContentSha1))
        //                    throw new InvalidOperationException();

        //                response.Add(results.Response);
        //            }
        //        }
        //    }

        //    Assert.Equal(files.Count(), response.Count());

        //    var deletedRequest = new ListFileVersionRequest(BucketId);
        //    var deletedFiles = await Storage.Files.DeleteAllAsync(deletedRequest);
            
        //    Assert.Equal(files.Count(), deletedFiles.Count());
        //}
    }
}
