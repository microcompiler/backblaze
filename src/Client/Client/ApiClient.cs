using System.Net.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Bytewizer.Backblaze.Client
{
    /// <summary>
    /// Represents a default implementation of the <see cref="ApiClient"/> which uses <see cref="HttpClient"/> for making HTTP requests.
    /// </summary>
    public class ApiClient : ApiRest, IApiClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiClient"/> class.
        /// </summary>
        public ApiClient(HttpClient httpClient, IClientOptions options, ILogger<ApiRest> logger, IMemoryCache cache)
            : base(httpClient, options, logger, cache)
        { }
    }
}
