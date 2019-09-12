using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Bytewizer.Backblaze.Models;
using Bytewizer.Backblaze.Adapters;

namespace Bytewizer.Backblaze.Storage
{
    public partial class BackblazeStorage : IBackblazeKeys
    {
        public IBackblazeKeys Keys { get { return this; } }

        public Task<List<KeyItem>> ListAsync(ListKeysRequest request, int cacheTTL)
        {
            return Task.FromResult(new KeyAdapter(_client, request, cacheTTL, cancellationToken).ToList());
        }

        async Task<IApiResults<ListKeysResponse>> IBackblazeKeys.GetAsync()
        {
            var request = new ListKeysRequest(AccountId);
            return await _client.ListKeysAsync(request, cancellationToken);
        }

        async Task<IApiResults<ListKeysResponse>> IBackblazeKeys.GetAsync(ListKeysRequest request)
        {
            return await _client.ListKeysAsync(request, cancellationToken);
        }

        async Task<IApiResults<CreateKeyResponse>> IBackblazeKeys.CreateAsync(CreateKeyRequest request)
        {
            return await _client.CreateKeyAsync(request, cancellationToken);
        }

        async Task<IApiResults<CreateKeyResponse>> IBackblazeKeys.CreateAsync(Capabilities capabilities, string keyName)
        {
            var request = new CreateKeyRequest(AccountId, capabilities, keyName);
            return await  _client.CreateKeyAsync(request, cancellationToken);
        }

        async Task<IApiResults<CreateKeyResponse>> IBackblazeKeys.CreateAsync(string accountId, Capabilities capabilities, string keyName)
        {
            var request = new CreateKeyRequest(AccountId, capabilities, keyName);
            return await _client.CreateKeyAsync(request, cancellationToken);
        }

        async Task<IApiResults<DeleteKeyResponse>> IBackblazeKeys.DeleteAsync(string applicationKeyId)
        {
            var request = new DeleteKeyRequest(applicationKeyId);
            return await  _client.DeleteKeyAsync(request, cancellationToken);
        }
    }
}
