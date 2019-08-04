using System.Threading.Tasks;

using Bytewizer.Backblaze.Models;

namespace Bytewizer.Backblaze.Agent
{
    public partial class BackblazeAgent : IBackblazeKeysAgent
    {
        public IBackblazeKeysAgent Keys { get { return this; } }

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

        async Task<IApiResults<CreateKeyResponse>> IBackblazeKeysAgent.CreateAsync(string[] capabilities, string keyName)
        {
            var request = new CreateKeyRequest(AccountId, capabilities, keyName);
            return await _client.CreateKeyAsync(request, cancellationToken);
        }

        async Task<IApiResults<CreateKeyResponse>> IBackblazeKeysAgent.CreateAsync(string accountId, string[] capabilities, string keyName)
        {
            var request = new CreateKeyRequest(AccountId, capabilities, keyName);
            return await _client.CreateKeyAsync(request, cancellationToken);
        }

        async Task<IApiResults<DeleteKeyResponse>> IBackblazeKeysAgent.DeleteAsync(string applicationKeyId)
        {
            var request = new DeleteKeyRequest(applicationKeyId);
            return await _client.DeleteKeyAsync(request, cancellationToken);
        }
    }
}
