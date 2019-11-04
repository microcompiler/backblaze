using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Bytewizer.Backblaze.Client;
using Bytewizer.Backblaze.Models;
using Bytewizer.Backblaze.Extensions;
using Bytewizer.Backblaze.Enumerables;

using Xunit;

namespace Backblaze.Tests.Integration
{
    [Collection("Sequential")]
    public class PartsTest : BaseFixture
    {
        private static readonly string[] _partId = new string[2];
        private static readonly string[] _partHash = new string[2];

        private static string _fileId;
        private static readonly string _fileName = "finished-file.bin";

        private static string _copyId;
        private static readonly string _copyName = "finished-file-copy.bin";

        private static Uri _uploadUrl;
        private static string _authorizationToken;

        public PartsTest(StorageClientFixture fixture)
            : base(fixture)
        { }

        [Fact, TestPriority(1)]
        public async Task StartLargeFileAsync()
        {
            var results = await Storage.Parts.StartLargeFileAsync(BucketId, _fileName);
            results.EnsureSuccessStatusCode();

            _fileId = results.Response.FileId;

            Assert.Equal(typeof(StartLargeFileResponse), results.Response.GetType());
            Assert.Equal(ContentType, results.Response.ContentType);
            Assert.Equal(0, results.Response.ContentLength);
            Assert.Equal("none", results.Response.ContentSha1);
            Assert.True(results.Response.UploadTimestamp.IsClose());
        }

        [Fact, TestPriority(2)]
        public async Task GetUploadUrlAsync()
        {
            var results = await Storage.Parts.GetUploadUrlAsync(_fileId);
            results.EnsureSuccessStatusCode();

            _uploadUrl = results.Response.UploadUrl;
            _authorizationToken = results.Response.AuthorizationToken;

            Assert.Equal(typeof(GetUploadPartUrlResponse), results.Response.GetType());
            Assert.NotNull(results.Response.UploadUrl);
            Assert.NotNull(results.Response.AuthorizationToken);
            Assert.NotNull(results.Response.FileId);
        }

        [Fact, TestPriority(3)]
        public async Task UploadPartAsync()
        {
            var bytes = Enumerable.Range(0, (int)(ClientOptions.MinimumCutoffSize)).Select(i => (byte)i).ToArray();
            
            for (int i = 0; i <= 1; i++)
            {
                using (var content = new MemoryStream(bytes))
                {
                    var part1Results = await Storage.Parts.UploadAsync(_uploadUrl, i + 1, _authorizationToken, content, null);
                    if (part1Results.IsSuccessStatusCode)
                    {
                        var sha1 = content.ToSha1();
                        Assert.Equal(i + 1, part1Results.Response.PartNumber);
                        Assert.Equal(sha1, part1Results.Response.ContentSha1);

                        _partId[i] = part1Results.Response.FileId;
                        _partHash[i] = sha1;
                    }
                }
            }
        }

        [Fact, TestPriority(4)]
        public async Task GetAsync()
        {
            var results = await Storage.Parts.GetAsync(_fileId);

            Assert.Equal(typeof(List<PartItem>), results.GetType());
            Assert.True(results.ToList().Count() == 2, "The actual count was not two");
        }

        [Fact, TestPriority(4)]
        public async Task ListAsync()
        {
            var results = await Storage.Parts.ListAsync(_fileId);
            results.EnsureSuccessStatusCode();

            Assert.Equal(typeof(ListPartsResponse), results.Response.GetType());
            Assert.True(results.Response.Parts.Count == 2, "The actual count was not two");
        }

        [Fact, TestPriority(4)]
        public async Task GetEnumerableAsync()
        {
            var request = new ListPartsRequest(_fileId);
            var enumerable = await Storage.Parts.GetEnumerableAsync(request);

            Assert.Equal(typeof(PartEnumerable), enumerable.GetType());
            Assert.True(enumerable.ToList().Count() == 2, "The actual count was not two");
        }

        [Fact, TestPriority(5)]
        public async Task FinishLargeFileAsync()
        {
            var results = await Storage.Parts.FinishLargeFileAsync(_fileId, _partHash.ToList());
            results.EnsureSuccessStatusCode();

            _fileId = results.Response.FileId;

            Assert.Equal(typeof(UploadFileResponse), results.Response.GetType());
            Assert.Equal(ContentType, results.Response.ContentType);
            Assert.Equal(ActionType.Upload, results.Response.Action);
            Assert.Equal(ClientOptions.MinimumCutoffSize * 2, results.Response.ContentLength);
            Assert.Null(results.Response.ContentSha1);
        }

        [Fact, TestPriority(6)]
        public async Task CopyAsync()
        {
            var results = await Storage.Parts.StartLargeFileAsync(BucketId, _copyName);
            results.EnsureSuccessStatusCode();

            _copyId = results.Response.FileId;

            var copyResults = await Storage.Parts.CopyAsync(_fileId, _copyId, 1);
            copyResults.EnsureSuccessStatusCode();

            Assert.Equal(typeof(CopyPartResponse), copyResults.Response.GetType());
            Assert.Equal(results.Response.FileId, copyResults.Response.FileId);
            Assert.Equal(1, copyResults.Response.PartNumber);
            Assert.Equal(ClientOptions.MinimumCutoffSize * 2, copyResults.Response.ContentLength);
        }

        [Fact, TestPriority(7)]
        public async Task CancelLargeFileAsync()
        {
            var results = await Storage.Parts.CancelLargeFileAsync(_copyId);
            results.EnsureSuccessStatusCode();

            Assert.Equal(typeof(CancelLargeFileResponse), results.Response.GetType());
            Assert.Equal(BucketId, results.Response.BucketId);
            Assert.Equal(Storage.AccountId, results.Response.AccountId);
            Assert.Equal(_copyId, results.Response.FileId);
            Assert.Equal(_copyName, results.Response.FileName);
        }

        [Fact, TestPriority(100)]
        public async Task DeleteAsync()
        {
            var results = await Storage.Files.DeleteAsync(_fileId, _fileName);
            results.EnsureSuccessStatusCode();
        }
    }
}
