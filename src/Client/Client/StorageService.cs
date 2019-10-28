using Microsoft.Extensions.Logging;

namespace Bytewizer.Backblaze.Client
{
    /// <summary>
    /// Represents a default implementation of the <see cref="StorageService"/> which uses <see cref="ApiClient"/> for making requests.
    /// </summary>
    public class StorageService : Storage, IStorageAgent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StorageService"/> class.
        /// </summary>
        public StorageService(IApiClient client, ILogger<StorageService> logger)
            : base (client, logger)
        { }
    }
}
