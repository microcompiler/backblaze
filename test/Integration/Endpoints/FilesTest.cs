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

namespace Backblaze.Tests.Integration
{
    [Collection("Sequential")]
    public class FilesTest : BaseFixture
    {
        private static readonly MockFileSystem _fileSystem = new MockFileSystem();

        public FilesTest(StorageClientFixture fixture)
            : base(fixture)
        {
            _fileSystem.AddFile(@"c:\root-five-bytes.bin", new MockFileData(new byte[] { 0x01, 0x34, 0x56, 0xd2, 0xd2 }));
            _fileSystem.AddFile(@"c:\matrix\five-bytes.bin", new MockFileData(new byte[] { 0x02, 0x34, 0x56, 0xd2, 0xd2 }));
            _fileSystem.AddFile(@"c:\shawshank\five-bytes.bin", new MockFileData(new byte[] { 0x03, 0x34, 0x56, 0xd2, 0xd2 }));
        }

        [Fact, TestPriority(1)]
        public async Task UploadAsync()
        {
            var response = new List<UploadFileResponse>();
            var files = _fileSystem.Directory.GetFiles(@"c:\", "*.*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                using (var content = _fileSystem.File.OpenRead(file))
                {
                    var request = new UploadFileByBucketIdRequest(BucketId, file)
                    {
                        LastModified = _fileSystem.File.GetLastWriteTime(file)
                    };
                    var results = await Storage.UploadAsync(request, content, null, CancellationToken.None);
                    if (results.IsSuccessStatusCode)
                    {
                        var fileSha1 = _fileSystem.File.OpenRead(file).ToSha1();
                        if (!fileSha1.Equals(results.Response.ContentSha1))
                            throw new InvalidOperationException();

                        response.Add(results.Response);
                    }
                }
            }

            Assert.Equal(files.Count(), response.Count());
        }

        [Fact, TestPriority(2)]
        public async Task GetFileNameEnumerable()
        {
            var request = new ListFileNamesRequest(BucketId);
            var enumerable = await Storage.Files.GetEnumerableAsync(request);

            Assert.Equal(typeof(FileNameEnumerable), enumerable.GetType());
            Assert.Equal(_fileSystem.AllFiles.Count(), enumerable.ToList().Count());
        }

        [Fact, TestPriority(2)]
        public async Task GetFileVersionEnumerable()
        {
            var request = new ListFileVersionRequest(BucketId);
            var enumerable = await Storage.Files.GetEnumerableAsync(request);

            Assert.Equal(typeof(FileVersionEnumerable), enumerable.GetType());
            Assert.Equal(_fileSystem.AllFiles.Count(), enumerable.ToList().Count());
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
                    var fileSha1 = _fileSystem.File.OpenRead(fileList.FileName).ToSha1();
                    if (!fileSha1.Equals(fileList.ContentSha1))
                        throw new InvalidOperationException();

                    response.Add(results.Response);
                }
            }

            Assert.Equal(_fileSystem.AllFiles.Count(), response.Count());
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

            Assert.Equal(_fileSystem.AllFiles.Count(), response.Count());
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
                    Assert.Equal(file.ContentLength, results.Response.ContentLength);
                    Assert.Equal(file.ContentSha1, results.Response.ContentSha1);
                    Assert.Equal(file.ContentType, results.Response.ContentType);
                    Assert.Equal(file.FileInfo, results.Response.FileInfo);
                    Assert.Equal(file.FileName, results.Response.FileName);
                    Assert.Equal(file.UploadTimestamp, results.Response.UploadTimestamp);

                    response.Add(results.Response);
                }
            }

            Assert.Equal(files.Response.Files.Count(), response.Count());
        }

        [Fact, TestPriority(4)]
        public async Task GetUnfinishedEnumerable()
        {
            var results = await Storage.Parts.StartLargeFileAsync(BucketId, "unfinished-file.bin");
            results.EnsureSuccessStatusCode();

            var request = new ListUnfinishedLargeFilesRequest(BucketId);
            var enumerable = await Storage.Files.GetEnumerableAsync(request);

            Assert.Equal(typeof(UnfinishedEnumerable), enumerable.GetType());
            Assert.Single(enumerable.ToList());
        }

        [Fact, TestPriority(5)]
        public async Task DownloadAsync()
        {
            var response = new List<DownloadFileResponse>();
            var fileSystem = new MockFileSystem();
            var files = _fileSystem.Directory.GetFiles(@"c:\", "*.*", SearchOption.AllDirectories);

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

                var byte1 = _fileSystem.File.ReadAllBytes(file);
                var byte2 = fileSystem.File.ReadAllBytes(file);

                if (!byte1.SequenceEqual(byte2))
                    throw new InvalidOperationException();
            }

            Assert.Equal(files.Count(), response.Count());
        }

        [Fact, TestPriority(6)]
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

        [Fact, TestPriority(7)]
        public async Task DeleteAsync()
        {
            var response = new List<DeleteFileVersionResponse>();

            var files = await Storage.Files.ListVersionsAsync(BucketId);
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
