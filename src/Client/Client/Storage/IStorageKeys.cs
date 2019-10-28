using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Authentication;

using Bytewizer.Backblaze.Models;

namespace Bytewizer.Backblaze.Client
{
    /// <summary>
    /// An interface for <see cref="Storage"/>.
    /// </summary>
    public interface IStorageKeys
    {
        #region ApiClient

        /// <summary>
        /// Creates a new application key. There is a limit of 100 million key creations per account.
        /// </summary>
        /// <param name="keyName">The name for this key.</param>
        /// <param name="capabilities">A list of <see cref="Capability"/> each one naming a capability the new key should have.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<CreateKeyResponse>> CreateAsync(string keyName, Capabilities capabilities);

        /// <summary>
        /// Creates a new application key. There is a limit of 100 million key creations per account.
        /// </summary>
        /// <param name="request">The <see cref="CreateKeyRequest"/> to send.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<CreateKeyResponse>> CreateAsync(CreateKeyRequest request);

        /// <summary>
        /// Deletes the application key specified. 
        /// </summary>
        /// <param name="applicationKeyId">The application key id to delete.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<DeleteKeyResponse>> DeleteAsync(string applicationKeyId);

        /// <summary>
        /// List application keys associated with an account. 
        /// </summary>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<ListKeysResponse>> ListAsync();

        /// <summary>
        /// List application keys associated with an account. 
        /// </summary>
        /// <param name="request">The <see cref="ListKeysRequest"/> to send.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<ListKeysResponse>> ListAsync(ListKeysRequest request, TimeSpan cacheTTL = default);

        #endregion

        /// <summary>
        /// Returns an enumerator that iterates through all application keys associated with an account. 
        /// </summary>
        /// <param name="request">The <see cref="ListKeysRequest"/> to send.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IEnumerable<KeyItem>> GetEnumerableAsync(ListKeysRequest request, TimeSpan cacheTTL = default);

        /// <summary>
        /// Gets all keys associated with an account in alphabetical order by key name. 
        /// </summary>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IEnumerable<KeyItem>> GetAsync();

        /// <summary>
        /// Gets all keys associated with an account in alphabetical order by key name. 
        /// </summary>
        /// <param name="request">The <see cref="ListBucketsRequest"/> to send.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IEnumerable<KeyItem>> GetAsync(ListKeysRequest request, TimeSpan cacheTTL = default);

        /// <summary>
        /// Find key by name.
        /// </summary>
        /// <param name="keyName">The key name to retrive.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<KeyItem> FindByNameAsync(string keyName, TimeSpan cacheTTL = default);

        /// <summary>
        /// Find key by id.
        /// </summary>
        /// <param name="keyId">The key id to retrive.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<KeyItem> FindByIdAsync(string keyId, TimeSpan cacheTTL = default);
    }
}
