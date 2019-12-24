using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using System.IO.Abstractions.TestingHelpers;

using Bytewizer.Backblaze.Client;
using Bytewizer.Backblaze.Models;
using Bytewizer.Backblaze.Progress;
using Bytewizer.Backblaze.Extensions;

using Xunit;

namespace Backblaze.Tests.Integration
{
    [Collection("Sequential")]
    public class LargeFilesTest : BaseFixture
    {
        private readonly MockFileSystem _fileSystem = new MockFileSystem();
        private readonly string _fileName = "six-megabyte.bin";
        private static string _fileId;

        public LargeFilesTest(StorageClientFixture fixture) 
            : base(fixture)
        {
            var content = Enumerable.Range(0, (int)(ClientOptions.MinimumCutoffSize * 1.1)).Select(i => (byte)i).ToArray();
            _fileSystem.AddFile(_fileName, new MockFileData(content));
        }

        [Fact, TestPriority(10)]
        public async Task UploadAsync()
        {
            using (var content = _fileSystem.File.OpenRead(_fileName))
            {
                var request = new UploadFileByBucketIdRequest(BucketId, _fileName);
                var results = await Storage.UploadAsync(request, content, null, CancellationToken.None);
                if (results.IsSuccessStatusCode)
                {
                    var fileSha1 = _fileSystem.File.OpenRead(_fileName).ToSha1();
                    if (!fileSha1.Equals(results.Response.ContentSha1))
                        throw new InvalidOperationException();

                    _fileId = results.Response.FileId;
                }
            }

            Assert.Single(_fileSystem.AllFiles);
        }

        [Fact, TestPriority(11)]
        public async Task DownloadAsync()
        {        
            var fileSystem = new MockFileSystem();
            string fileSha1 = string.Empty;

            int progressCounter = 0;

            TimeSpan transferTime = TimeSpan.Zero;
            long bytesPerSecond = 0;
            long bytesTransferred = 0;
            long expectedBytes = 0;
            double percentComplete = 0;
            
            var progress = new NaiveProgress<ICopyProgress>(x =>
            {
                progressCounter++;

                transferTime = x.TransferTime;
                bytesPerSecond = x.BytesPerSecond;
                bytesTransferred = x.BytesTransferred;
                expectedBytes = x.ExpectedBytes;
                percentComplete = x.PercentComplete;
            });
            
            using (var content = fileSystem.File.Create(_fileName))
            {
                var request = new DownloadFileByNameRequest(BucketName, _fileName);
                var results = await Storage.DownloadAsync(request, content, progress, CancellationToken.None);
                if (results.IsSuccessStatusCode)
                {
                    fileSha1 = results.Response.ContentSha1;
                }
            }

            var downloadSha1 = fileSystem.File.OpenRead(_fileName).ToSha1();
            if (!downloadSha1.Equals(fileSha1))
                throw new InvalidOperationException();

            Assert.Single(fileSystem.AllFiles);
            Assert.True(progressCounter > 0);
            Assert.True(transferTime > TimeSpan.Zero);
            Assert.True(bytesPerSecond > 0);
            Assert.Equal(524288, bytesTransferred);
            Assert.Equal(524288, expectedBytes);
            Assert.Equal(1, percentComplete);
        }

        [Fact, TestPriority(12)]
        public async Task DownloadAsync_ById()
        {
            var fileSystem = new MockFileSystem();
            string fileSha1 = string.Empty;

            int progressCounter = 0;

            TimeSpan transferTime = TimeSpan.Zero;
            long bytesPerSecond = 0;
            long bytesTransferred = 0;
            long expectedBytes = 0;
            double percentComplete = 0;

            var progress = new NaiveProgress<ICopyProgress>(x =>
            {
                progressCounter++;

                transferTime = x.TransferTime;
                bytesPerSecond = x.BytesPerSecond;
                bytesTransferred = x.BytesTransferred;
                expectedBytes = x.ExpectedBytes;
                percentComplete = x.PercentComplete;
            });

            using (var content = fileSystem.File.Create(_fileName))
            {
                var request = new DownloadFileByIdRequest(_fileId);
                var results = await Storage.DownloadByIdAsync(request, content, progress, CancellationToken.None);
                if (results.IsSuccessStatusCode)
                {
                    fileSha1 = results.Response.ContentSha1;
                }
            }

            var downloadSha1 = fileSystem.File.OpenRead(_fileName).ToSha1();
            if (!downloadSha1.Equals(fileSha1))
                throw new InvalidOperationException();

            Assert.Single(fileSystem.AllFiles);
            Assert.True(progressCounter > 0);
            Assert.True(transferTime > TimeSpan.Zero);
            Assert.True(bytesPerSecond > 0);
            Assert.Equal(524288, bytesTransferred);
            Assert.Equal(524288, expectedBytes);
            Assert.Equal(1, percentComplete);
        }

        [Fact, TestPriority(100)]
        public async Task DeleteAsync()
        {
            var results = await Storage.Files.DeleteAsync(_fileId, _fileName);
            results.EnsureSuccessStatusCode();
        }
    }
}
