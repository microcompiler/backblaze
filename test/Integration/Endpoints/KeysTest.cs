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
    public class KeysTest : BaseFixture
    {
        private static readonly string _keyName = $"{Guid.NewGuid().ToString()}";
        private static string _keyId;

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

            _keyId = results.Response.ApplicationKeyId;

            Assert.Equal(typeof(CreateKeyResponse), results.Response.GetType());
            //Assert.Equal(DateTime.Now.AddDays(5).Date, results.Response.ExpirationTimestamp.Date);
            Assert.Equal(Capabilities.ReadOnly(), results.Response.Capabilities);
        }

        [Fact, TestPriority(2)]
        public async Task FindByNameAsync()
        {
            var results = await Storage.Keys.FindByNameAsync(_keyName);

            Assert.Equal(typeof(KeyItem), results.GetType());
            Assert.Equal(_keyId, results.ApplicationKeyId);
            Assert.Equal(Capabilities.ReadOnly(), results.Capabilities);
        }

        [Fact, TestPriority(2)]
        public async Task FindByIdAsync()
        {
            var results = await Storage.Keys.FindByIdAsync(_keyId);

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
            Assert.True(enumerable.ToList().Count() >= 1, "The actual count was not greater than one");
        }

        [Fact, TestPriority(2)]
        public async Task ListAsync()
        {
            var results = await Storage.Keys.ListAsync();
            results.EnsureSuccessStatusCode();

            Assert.Equal(typeof(ListKeysResponse), results.Response.GetType());
            Assert.True(results.Response.Keys.Count >= 1, "The actual count was not greater than one");
        }

        [Fact, TestPriority(2)]
        public async Task GetAsync()
        {
            var results = await Storage.Keys.GetAsync();

            Assert.Equal(typeof(List<KeyItem>), results.GetType());
            Assert.True(results.ToList().Count() >= 1, "The actual count was not greater than one");
        }

        [Fact, TestPriority(3)]
        public async Task DeleteAsync()
        {
            var results = await Storage.Keys.DeleteAsync(_keyId);
            results.EnsureSuccessStatusCode();

            Assert.Equal(typeof(DeleteKeyResponse), results.Response.GetType());
            Assert.Equal(Capabilities.ReadOnly(), results.Response.Capabilities);
        }
    }
}
