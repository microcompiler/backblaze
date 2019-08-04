using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;

namespace Bytewizer.Backblaze.Client
{
    /// <summary>
    /// Represents a default implementation of the <see cref="Storage"/> which uses <see cref="HttpClient"/> for making HTTP requests.
    /// </summary>
    public class ApiClient : Storage, IApiClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiClient"/> class.
        /// </summary>
        public ApiClient() : base(null, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Storage"/> class.
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient"/> used for making requests.</param>
        public ApiClient(HttpClient httpClient, ILogger<Storage> logger)
            : base(httpClient, logger)
        { }

        /// <summary>
        /// Creates an initialized instance of the client connected to the Backblaze B2 API server.
        /// </summary>
        public static ApiClient Initialize(string applicationKeyId, string applicationKey)
        {
            // Create client
            var client = new ApiClient();
            if (client == null) throw new ArgumentNullException(nameof(client));

            // Connect to server
            client.Connect(applicationKeyId, applicationKey);

            // Return initialized device
            return client;
        }
    }
}
