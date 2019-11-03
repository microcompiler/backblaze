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
using System.Diagnostics;

namespace Backblaze.Tests.Integration
{
    public class PartsTest : BaseFixture
    {
        private static readonly MockFileSystem _fileSystem = new MockFileSystem();
        private static readonly string _fileName = "finished-file.bin";
        private static string _fileId;
        private static readonly string _fileNameCopy = "finished-file-copy.bin";
        private static string _largeFileId;

        private static Uri _uploadUrl;
        private static string _authorizationToken;
        private static string _filePartId;

        private static List<string> sha1Hash = new List<string>();

        public PartsTest(StorageClientFixture fixture) 
            : base(fixture)
        {
            var content = Enumerable.Range(0, (int)(ClientOptions.MinimumCutoffSize)).Select(i => (byte)i).ToArray();

            _fileSystem.AddFile(@"c:\part1.bin", new MockFileData(content));
            _fileSystem.AddFile(@"c:\part2.bin", new MockFileData(content));
            _fileSystem.AddFile($"c:\\{_fileName}", new MockFileData(ConcatByteArrays(content, content)));
        }

        [Fact, TestPriority(3)]
        public async Task UploadPartAsync()
        {
            var response = new List<UploadPartResponse>();
            var files = _fileSystem.Directory.GetFiles(@"c:\", "part*.*", SearchOption.AllDirectories);
            var partNumber = 1;

            foreach (var file in files)
            {            
                using (var content = _fileSystem.File.OpenRead(file))
                {
                    var results = await Storage.Parts.UploadAsync(_uploadUrl, partNumber, _authorizationToken, content, null);
                    if (results.IsSuccessStatusCode)
                    {
                        var fileSha1 = _fileSystem.File.OpenRead(file).ToSha1();
                        if (!fileSha1.Equals(results.Response.ContentSha1))
                            throw new InvalidOperationException();
                        _filePartId = results.Response.FileId;

                        Assert.Equal(partNumber, results.Response.PartNumber);
                        Assert.Equal(_fileSystem.File.OpenRead(file).Length, results.Response.ContentLength);
                        Assert.Equal(fileSha1, results.Response.ContentSha1);
                        Assert.True(results.Response.UploadTimestamp.IsClose());

                        partNumber++;
                        response.Add(results.Response);
                        sha1Hash.Add(fileSha1);
                    }
                }
            }

            Assert.Equal(files.Count(), response.Count());
        }

        [Fact, TestPriority(10)]
        public async Task CancelLargeFileAsync()
        {
            var results = await Storage.Parts.CancelLargeFileAsync(_largeFileId);
            results.EnsureSuccessStatusCode();

            Assert.Equal(typeof(CancelLargeFileResponse), results.Response.GetType());
            Assert.Equal(BucketId, results.Response.BucketId);
            Assert.Equal(Storage.AccountId, results.Response.AccountId);
            Assert.Equal(_largeFileId, results.Response.FileId);
            Assert.Equal(_fileNameCopy, results.Response.FileName);
        }

        [Fact, TestPriority(4)]
        public async Task CopyAsync()
        {
            var results = await Storage.Parts.StartLargeFileAsync(BucketId, _fileNameCopy);
            results.EnsureSuccessStatusCode();

            _largeFileId = results.Response.FileId;

            var request = new ListFileNamesRequest(BucketId);
            var fileList = await Storage.Files.GetEnumerableAsync(request);
            var firstFile = fileList.ToList().First();

            var copyResults = await Storage.Parts.CopyAsync(firstFile.FileId, _largeFileId, 1);
            copyResults.EnsureSuccessStatusCode();

            Assert.Equal(typeof(CopyPartResponse), copyResults.Response.GetType());

            Assert.Equal(results.Response.FileId, copyResults.Response.FileId);
            Assert.Equal(1, copyResults.Response.PartNumber);
            //Assert.Equal(ClientOptions.MinimumCutoffSize * 2, copyResults.Response.ContentLength);
            //Assert.Equal("none", copyResults.Response.ContentSha1);
            Assert.True(copyResults.Response.UploadTimestamp.IsClose());
        }

        [Fact, TestPriority(5)]
        public async Task GetAsync()
        {
            var results = await Storage.Parts.GetAsync(_fileId);

            Assert.Equal(typeof(List<PartItem>), results.GetType());
            Assert.True(results.ToList().Count() >= 1, "The actual count was not greater than one");
        }

        [Fact, TestPriority(10)]
        public async Task FinishLargeFileAsync()
        {
            var results = await Storage.Parts.FinishLargeFileAsync(_fileId, sha1Hash);
            results.EnsureSuccessStatusCode();

            Assert.Equal(typeof(UploadFileResponse), results.Response.GetType());
            Assert.Equal(ContentType, results.Response.ContentType);
            Assert.Equal(ActionType.Upload, results.Response.Action);
            Assert.Equal(ClientOptions.MinimumCutoffSize * 2, results.Response.ContentLength);
            Assert.Null(results.Response.ContentSha1);
            //Assert.True(results.Response.UploadTimestamp.IsClose());
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

        [Fact, TestPriority(5)]
        public async Task ListAsync()
        {
            var results = await Storage.Parts.ListAsync(_fileId);
            results.EnsureSuccessStatusCode();

            Assert.Equal(typeof(ListPartsResponse), results.Response.GetType());
            Assert.True(results.Response.Parts.Count >= 1, "The actual count was not greater than one");
        }

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

        [Fact, TestPriority(5)]
        public async Task GetEnumerableAsync()
        {
            var request = new ListPartsRequest(_fileId);
            var enumerable = await Storage.Parts.GetEnumerableAsync(request);

            Assert.Equal(typeof(PartEnumerable), enumerable.GetType());
            Assert.True(enumerable.ToList().Count() >= 1, "The actual count was not greater than one");
        }

        //[Fact, TestPriority(5)]
        //public async Task GetAsync()
        //{
        //    var results = await Storage.Parts.GetAsync();

        //    Assert.Equal(typeof(List<PartItem>), results.GetType());
        //    Assert.True(results.ToList().Count() >= 1, "The actual count was not greater than one");
        //}

        public static byte[] ConcatByteArrays(params byte[][] arrays)
        {
            return arrays.SelectMany(x => x).ToArray();
        }
    }
}
