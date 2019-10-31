using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Authentication;

using Bytewizer.Backblaze.Client;
using Bytewizer.Backblaze.Models;

using Bytewizer.Backblaze.Enumerables;

namespace Bytewizer.Backblaze.Client
{
    /// <summary>
    /// Represents a default implementation of the <see cref="Storage"/> which uses <see cref="ApiClient"/> for making HTTP requests.
    /// </summary>
    public partial class Storage : IStorageKeys
    {
        /// <summary>
        /// Provides methods to access key operations.
        /// </summary>
        public IStorageKeys Keys { get { return this; } }

        #region ApiClient

        /// <summary>
        /// Creates a new application key. There is a limit of 100 million key creations per account.
        /// </summary>
        /// <param name="keyName">The name for this key.</param>
        /// <param name="capabilities">A list of <see cref="Capability"/> each one naming a capability the new key should have.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<CreateKeyResponse>> IStorageKeys.CreateAsync
            (string keyName, Capabilities capabilities)
        {
            var request = new CreateKeyRequest(AccountId, keyName, capabilities);
            return await _client.CreateKeyAsync(request, cancellationToken);
        }

        /// <summary>
        /// Creates a new application key. There is a limit of 100 million key creations per account.
        /// </summary>
        /// <param name="request">The <see cref="CreateKeyRequest"/> to send.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<CreateKeyResponse>> IStorageKeys.CreateAsync
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
        async Task<IApiResults<DeleteKeyResponse>> IStorageKeys.DeleteAsync
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
        async Task<IApiResults<ListKeysResponse>> IStorageKeys.ListAsync()
        {
            var request = new ListKeysRequest(AccountId);
            return await _client.ListKeysAsync(request, cancellationToken);
        }

        /// <summary>
        /// List application keys associated with an account. 
        /// </summary>
        /// <param name="request">The <see cref="ListKeysRequest"/> to send.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<ListKeysResponse>> IStorageKeys.ListAsync
            (ListKeysRequest request, TimeSpan cacheTTL)
        {
            return await _client.ListKeysAsync(request, cacheTTL, cancellationToken);
        }

        #endregion

        /// <summary>
        /// Returns an enumerator that iterates through all application keys associated with an account. 
        /// </summary>
        /// <param name="request">The <see cref="ListKeysRequest"/> to send.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IEnumerable<KeyItem>> IStorageKeys.GetEnumerableAsync
            (ListKeysRequest request, TimeSpan cacheTTL)
        {
            var enumerable = new KeyEnumerable(_client, _logger, request, cacheTTL, cancellationToken) as IEnumerable<KeyItem>;
            return await Task.FromResult(enumerable).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets all keys associated with an account in alphabetical order by key name. 
        /// </summary>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IEnumerable<KeyItem>> IStorageKeys.GetAsync()
        {
            var request = new ListKeysRequest(AccountId);
            return await Keys.GetAsync(request);
        }

        /// <summary>
        /// Gets all keys associated with an account in alphabetical order by key name. 
        /// </summary>
        /// <param name="request">The <see cref="ListBucketsRequest"/> to send.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IEnumerable<KeyItem>> IStorageKeys.GetAsync
            (ListKeysRequest request, TimeSpan cacheTTL)
        {
            var enumerable = await Keys.GetEnumerableAsync(new ListKeysRequest(AccountId), cacheTTL);
            return enumerable.OrderBy(x => x.KeyName).ToList();
        }

        /// <summary>
        /// Find key by name.
        /// </summary>
        /// <param name="keyName">The key name to retrive.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<KeyItem> IStorageKeys.FindByNameAsync
            (string keyName, TimeSpan cacheTTL)
        {
            var enumerable = await Keys.GetEnumerableAsync(new ListKeysRequest(AccountId), cacheTTL);
            return enumerable.FirstOrDefault(x => x.KeyName == keyName);
        }

        /// <summary>
        /// Find key by id.
        /// </summary>
        /// <param name="keyId">The key id to retrive.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<KeyItem> IStorageKeys.FindByIdAsync
            (string keyId, TimeSpan cacheTTL)
        {
            var enumerable = await Keys.GetEnumerableAsync(new ListKeysRequest(AccountId), cacheTTL);
            return enumerable.FirstOrDefault(x => x.ApplicationKeyId == keyId);
        }
    }
}
