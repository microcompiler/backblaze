using System;
using System.Net.Http;

using Microsoft.Extensions.Logging;

namespace Bytewizer.Backblaze.Client
{
    /// <summary>
    /// Represents a default implementation of the <see cref="ApiClient"/> which uses <see cref="HttpClient"/> for making HTTP requests.
    /// </summary>
    public class ApiClient : Storage, IApiClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiClient"/> class.
        /// </summary>
        public ApiClient() : base(null, null, null, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiClient"/> class.
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient"/> used for making HTTP requests.</param>
        /// <param name="logger">The <see cref="ILogger"/> used for application logging.</param>
        /// <param name="cache">The <see cref="ICacheManager"/> used for application caching.</param>
        /// <param name="policy">The <see cref="IPolicyManager"/> used for application resilience.</param>
        public ApiClient(HttpClient httpClient, ILogger<Storage> logger, ICacheManager cache, IPolicyManager policy)
            : base(httpClient, logger, cache, policy)
        { }

        /// <summary>
        /// Creates an initialized instance of the client connected to Backblaze B2 Cloud Storage.
        /// </summary>
        /// <param name="keyId">The identifier for the key.</param>
        /// <param name="applicationKey">The secret part of the key. You can use either the master application key or a normal application key.</param>
        public static ApiClient Initialize(string keyId, string applicationKey)
        {
            // Create client
            var client = new ApiClient();
            if (client == null) throw new ArgumentNullException(nameof(client));

            // Connect to server
            client.Connect(keyId, applicationKey);

            // Return initialized device
            return client;
        }
    }
}
