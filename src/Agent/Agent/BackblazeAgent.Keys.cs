using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Bytewizer.Backblaze.Models;

namespace Bytewizer.Backblaze.Agent
{
    public partial class BackblazeAgent : IBackblazeKeysAgent
    {
        public IBackblazeKeysAgent Keys { get { return this; } }

        public Task<List<KeyItem>> ListAsync(ListKeysRequest request, int cacheTTL)
        {
            return Task.FromResult(new Keys(_client, request, cacheTTL, cancellationToken).ToList());
        }

        async Task<IApiResults<ListKeysResponse>> IBackblazeKeysAgent.GetAsync()
        {
            var request = new ListKeysRequest(AccountId);
            return await _client.ListKeysAsync(request, cancellationToken);
        }

        async Task<IApiResults<ListKeysResponse>> IBackblazeKeysAgent.GetAsync(ListKeysRequest request)
        {
            return await _client.ListKeysAsync(request, cancellationToken);
        }

        async Task<IApiResults<CreateKeyResponse>> IBackblazeKeysAgent.CreateAsync(CreateKeyRequest request)
        {
            return await _client.CreateKeyAsync(request, cancellationToken);
        }

        async Task<IApiResults<CreateKeyResponse>> IBackblazeKeysAgent.CreateAsync(Capabilities capabilities, string keyName)
        {
            var request = new CreateKeyRequest(AccountId, capabilities, keyName);
            return await  _client.CreateKeyAsync(request, cancellationToken);
        }

        async Task<IApiResults<CreateKeyResponse>> IBackblazeKeysAgent.CreateAsync(string accountId, Capabilities capabilities, string keyName)
        {
            var request = new CreateKeyRequest(AccountId, capabilities, keyName);
            return await _client.CreateKeyAsync(request, cancellationToken);
        }

        async Task<IApiResults<DeleteKeyResponse>> IBackblazeKeysAgent.DeleteAsync(string applicationKeyId)
        {
            var request = new DeleteKeyRequest(applicationKeyId);
            return await  _client.DeleteKeyAsync(request, cancellationToken);
        }
    }
}
