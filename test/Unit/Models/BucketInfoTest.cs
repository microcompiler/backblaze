using System;
using System.Net.Http;
using Bytewizer.Backblaze.Agent;
using Bytewizer.Backblaze.Client;
using Bytewizer.Backblaze.Models;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Backblaze.Tests.Unit
{
    public class BucketInfoTest
    {
                
        [Fact]
        public void Tester()
        {
            var options = new ClientOptions() { KeyId = "e14ecff4c2db", ApplicationKey = "0007eb0f509d3f8d7b40f8594b10ea501dd48303e8" };
            var loggerFactory = new LoggerFactory();
            var apiClient = new ApiClient(new HttpClient(), options, loggerFactory.CreateLogger<ApiRest>(), null);
            //apiClient.Connect("e14ecff4c2db", "0007eb0f509d3f8d7b40f8594b10ea501dd48303e8");
            var storage = new StorageService(apiClient, loggerFactory.CreateLogger<StorageService>());
            var testBucket = storage.Buckets.FindByNameAsync("integraton-test-bucket-db7e").GetAwaiter().GetResult();

        }
        
        [Fact]
        public void BucketInfoEquatable()
        {
            var bucketinfo1 = new BucketInfo
                {
                    { "key1", "value1" },
                    { "key2", "value2" }
                };

            var bucketinfo2 = new BucketInfo
                {
                    { "key1", "value1" },
                    { "key2", "value2" }
                };

            Assert.True(bucketinfo1.Equals(bucketinfo2));
            Assert.Equal(bucketinfo1.GetHashCode(), bucketinfo2.GetHashCode());
        }

        [Fact]
        public void MaximumItems()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var list = new BucketInfo();

                for (int i = 0; i < BucketInfo.MaximumBucketItemsAllowed + 1; i++)
                {
                    list.Add(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
                }
            });
        }
    }
}
