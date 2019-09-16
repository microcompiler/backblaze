using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Authentication;

using Bytewizer.Backblaze.Client;
using Bytewizer.Backblaze.Models;

using Bytewizer.Backblaze.Adapters;

namespace Bytewizer.Backblaze.Storage
{
    /// <summary>
    /// Represents a default implementation of the <see cref="BackblazeStorage"/> which uses <see cref="ApiClient"/> for making HTTP requests.
    /// </summary>
    public partial class BackblazeStorage : IBackblazeKeys
    {
        /// <summary>
        /// Provides methods to access key operations.
        /// </summary>
        public IBackblazeKeys Keys { get { return this; } }

        #region ApiClient

        /// <summary>
        /// Creates a new application key. There is a limit of 100 million key creations per account.
        /// </summary>
        /// <param name="capabilities">A list of <see cref="Capability"/> each one naming a capability the new key should have.</param>
        /// <param name="keyName">The name for this key.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<CreateKeyResponse>> IBackblazeKeys.CreateAsync
            (Capabilities capabilities, string keyName)
        {
            var request = new CreateKeyRequest(AccountId, capabilities, keyName);
            return await _client.CreateKeyAsync(request, cancellationToken);
        }

        /// <summary>
        /// Creates a new application key. There is a limit of 100 million key creations per account.
        /// </summary>
        /// <param name="request">The <see cref="CreateKeyRequest"/> to send.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<CreateKeyResponse>> IBackblazeKeys.CreateAsync
            (CreateKeyRequest request)
        {
            return await _client.CreateKeyAsync(request, cancellationToken);
        }

        /// <summary>
        /// Deletes the application key specified. 
        /// </summary>
        /// <param name="applicationKeyId">The application key id to delete.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<DeleteKeyResponse>> IBackblazeKeys.DeleteAsync
            (string applicationKeyId)
        {
            var request = new DeleteKeyRequest(applicationKeyId);
            return await _client.DeleteKeyAsync(request, cancellationToken);
        }

        /// <summary>
        /// List application keys associated with an account. 
        /// </summary>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<ListKeysResponse>> IBackblazeKeys.ListAsync()
        {
            var request = new ListKeysRequest(AccountId);
            return await _client.ListKeysAsync(request, cancellationToken);
        }

        /// <summary>
        /// List application keys associated with an account. 
        /// </summary>
        /// <param name="request">The <see cref="ListKeysRequest"/> to send.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now in seconds.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<ListKeysResponse>> IBackblazeKeys.ListAsync
            (ListKeysRequest request, int cacheTTL)
        {
            return await _client.ListKeysAsync(request, cacheTTL, cancellationToken);
        }

        #endregion

        /// <summary>
        /// Gets all application keys associated with an account. 
        /// </summary>
        /// <param name="request">The <see cref="ListKeysRequest"/> to send.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now in seconds.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IEnumerable<KeyItem>> IBackblazeKeys.GetAsync(ListKeysRequest request, int cacheTTL)
        {
            var adapter = new KeyAdapter(_client, _logger, request, cacheTTL, cancellationToken) as IEnumerable<KeyItem>;
            return await Task.FromResult(adapter);
        }


        //public Task<List<KeyItem>> ListAsync(ListKeysRequest request, int cacheTTL)
        //{
        //    return Task.FromResult(new KeyAdapter(_client, request, cacheTTL, cancellationToken).ToList());
        //}

        //async Task<IApiResults<ListKeysResponse>> IBackblazeKeys.GetAsync()
        //{
        //    var request = new ListKeysRequest(AccountId);
        //    return await _client.ListKeysAsync(request, cancellationToken);
        //}

        //async Task<IApiResults<ListKeysResponse>> IBackblazeKeys.GetAsync(ListKeysRequest request)
        //{
        //    return await _client.ListKeysAsync(request, cancellationToken);
        //}

        //async Task<IApiResults<CreateKeyResponse>> IBackblazeKeys.CreateAsync(string accountId, Capabilities capabilities, string keyName)
        //{
        //    var request = new CreateKeyRequest(AccountId, capabilities, keyName);
        //    return await _client.CreateKeyAsync(request, cancellationToken);
        //}
    }
}
