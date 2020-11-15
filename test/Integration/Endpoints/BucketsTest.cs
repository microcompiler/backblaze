using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Collections.Generic;

using Bytewizer.Backblaze.Models;

using Xunit;

namespace Backblaze.Tests.Integration
{
    public class BucketsTest : BaseFixture
    {
        private static readonly string _bucketName = $"{Guid.NewGuid()}";
        private static string _bucketId;

        public BucketsTest(StorageClientFixture fixture) 
            : base(fixture)
        { }

        [Fact, TestPriority(1)]
        public async Task CreateAsync()
        {
            var request = new CreateBucketRequest(Storage.AccountId, _bucketName, BucketType.AllPrivate)
            {
                BucketInfo = new BucketInfo
                {
                    { "key1", "value1" },
                    { "key2", "value2" }
                },
                CacheControl = new CacheControlHeaderValue
                {
                    MaxAge = TimeSpan.FromSeconds(120)
                },
                LifecycleRules = new LifecycleRules
                {   new LifecycleRule
                    {
                        DaysFromHidingToDeleting = 6,
                        DaysFromUploadingToHiding = 5,
                        FileNamePrefix = "backup/",
                    },
                    new LifecycleRule
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
        public async Task FindByIdAsync()
        {
            var results = await Storage.Buckets.FindByIdAsync(_bucketId);

            Assert.Equal(typeof(BucketItem), results.GetType());
            Assert.Equal(_bucketName, results.BucketName);
        }

        [Fact, TestPriority(2)]
        public async Task FindByNameAsync()
        {
            var results = await Storage.Buckets.FindByNameAsync(_bucketName);

            Assert.Equal(typeof(BucketItem), results.GetType());
            Assert.Equal(_bucketId, results.BucketId);
        }

        [Fact, TestPriority(2)]
        public async Task ListAsync()
        {
            var results = await Storage.Buckets.ListAsync();
            results.EnsureSuccessStatusCode();

            Assert.Equal(typeof(ListBucketsResponse), results.Response.GetType());
            Assert.True(results.Response.Buckets.Count >= 1, "The actual count was less than one");
        }

        [Fact, TestPriority(2)]
        public async Task ListAsync_WithRequest()
        {
            var request = new ListBucketsRequest(Storage.AccountId)
            {
                BucketTypes = new BucketTypes { BucketFilter.AllPrivate }
            };

            var results = await Storage.Buckets.ListAsync(request);
            results.EnsureSuccessStatusCode();

            Assert.Equal(typeof(ListBucketsResponse), results.Response.GetType());
            Assert.True(results.Response.Buckets.Count >= 1, "The actual count was less than one");
        }

        [Fact, TestPriority(2)]
        public async Task GetAsync()
        {
            var results = await Storage.Buckets.GetAsync();

            Assert.Equal(typeof(List<BucketItem>), results.GetType());
            Assert.True(results.ToList().Count() >= 1, "The actual count was less than one");
        }

        [Fact, TestPriority(3)]
        public async Task UpdateAsync()
        {
            var results = await Storage.Buckets.UpdateAsync(_bucketId, BucketType.AllPrivate);
            results.EnsureSuccessStatusCode();

            Assert.Equal(typeof(UpdateBucketResponse), results.Response.GetType());
            Assert.Equal(_bucketId, results.Response.BucketId);
            Assert.Equal(BucketType.AllPrivate, results.Response.BucketType);
        }

        [Fact, TestPriority(100)]
        public async Task DeleteAsync()
        {
            var results = await Storage.Buckets.DeleteAsync(_bucketId);
            results.EnsureSuccessStatusCode();

            Assert.Equal(typeof(DeleteBucketResponse), results.Response.GetType());
            Assert.Equal(_bucketId, results.Response.BucketId);
            Assert.Equal(BucketType.AllPrivate, results.Response.BucketType);
        }
    }
}
