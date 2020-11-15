using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Bytewizer.Backblaze.Models;
using Bytewizer.Backblaze.Enumerables;

using Xunit;

namespace Backblaze.Tests.Integration
{
    public class KeysTest : BaseFixture
    {
        private static readonly string _keyName = $"{Guid.NewGuid().ToString()}";
        private static readonly string[] _keyId = new string[2];

        public KeysTest(StorageClientFixture fixture) 
            : base(fixture)
        { }

        [Fact, TestPriority(1)]
        public async Task CreateAsync()
        {
            var request = new CreateKeyRequest(Storage.AccountId, _keyName, Capabilities.ReadOnly())
            {
                ValidDurationInSeconds = DateTime.Now.AddDays(5).Second
            };
            var results = await Storage.Keys.CreateAsync(request);
            results.EnsureSuccessStatusCode();

            _keyId[0] = results.Response.ApplicationKeyId;

            Assert.Equal(typeof(CreateKeyResponse), results.Response.GetType());
            Assert.Equal(Capabilities.ReadOnly(), results.Response.Capabilities);
        }

        [Fact, TestPriority(1)]
        public async Task CreateAsync_With_BucketId()
        {
            var namePrefix = "prefix";
            var request = new CreateKeyRequest(Storage.AccountId, $"{_keyName}-with-bucket-id", Capabilities.BucketOnly())
            {
                ValidDurationInSeconds = DateTime.Now.AddDays(5).Second,
                BucketId = BucketId,
                NamePrefix = namePrefix
            };
            var results = await Storage.Keys.CreateAsync(request);
            results.EnsureSuccessStatusCode();

            _keyId[1] = results.Response.ApplicationKeyId;

            Assert.Equal(typeof(CreateKeyResponse), results.Response.GetType());
            Assert.Equal(Capabilities.BucketOnly(), results.Response.Capabilities);
            Assert.Equal(namePrefix, results.Response.NamePrefix);
        }

        [Fact, TestPriority(2)]
        public async Task FindByNameAsync()
        {
            var results = await Storage.Keys.FindByNameAsync(_keyName);

            Assert.Equal(typeof(KeyItem), results.GetType());
            Assert.Equal(_keyId[0], results.ApplicationKeyId);
            Assert.Equal(Capabilities.ReadOnly(), results.Capabilities);
        }

        [Fact, TestPriority(2)]
        public async Task FindByIdAsync()
        {
            var results = await Storage.Keys.FindByIdAsync(_keyId[0]);

            Assert.Equal(typeof(KeyItem), results.GetType());
            Assert.Equal(_keyName, results.KeyName);
            Assert.Equal(Capabilities.ReadOnly(), results.Capabilities);
        }

        [Fact, TestPriority(2)]
        public async Task GetEnumerableAsync()
        {
            var request = new ListKeysRequest(Storage.AccountId);
            var enumerable = await Storage.Keys.GetEnumerableAsync(request);

            Assert.Equal(typeof(KeyEnumerable), enumerable.GetType());
            Assert.True(enumerable.ToList().Count() >= 1, "The actual count was less than one");
        }

        [Fact, TestPriority(2)]
        public async Task ListAsync()
        {
            var results = await Storage.Keys.ListAsync();
            results.EnsureSuccessStatusCode();

            Assert.Equal(typeof(ListKeysResponse), results.Response.GetType());
            Assert.True(results.Response.Keys.Count >= 1, "The actual count was less than one");
        }

        [Fact, TestPriority(2)]
        public async Task GetAsync()
        {
            var results = await Storage.Keys.GetAsync();

            Assert.Equal(typeof(List<KeyItem>), results.GetType());
            Assert.True(results.ToList().Count() >= 1, "The actual count was less than one");
        }

        [Fact, TestPriority(100)]
        public async Task DeleteAsync()
        {
            foreach (var id in _keyId)
            {
                var results = await Storage.Keys.DeleteAsync(id);
                results.EnsureSuccessStatusCode();

                Assert.Equal(typeof(DeleteKeyResponse), results.Response.GetType());
            }
        }

        //[Fact, TestPriority(100)]
        //public async Task DeleteAsync_Invalid_KeyId()
        //{
        //    var results = await Storage.Keys.DeleteAsync("34fddfeef");
        //    results.EnsureSuccessStatusCode();

        //    Assert.Equal(typeof(DeleteKeyResponse), results.Response.GetType());
        //}
    }
}
