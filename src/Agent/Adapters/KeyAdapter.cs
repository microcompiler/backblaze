using System.Threading;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

using Bytewizer.Backblaze.Client;
using Bytewizer.Backblaze.Models;

namespace Bytewizer.Backblaze.Adapters
{
    /// <summary>
    /// Iterates sequentially through the <see cref="ListKeysResponse"/> response elements.
    /// </summary>
    public class KeyAdapter : BaseIterator<KeyItem>
    {
        /// <summary>
        /// The request to send.
        /// </summary>
        private readonly ListKeysRequest _request;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyAdapter"/> class.
        /// </summary>
        public KeyAdapter(IApiClient client, ILogger logger, ListKeysRequest request, int cacheTTL, CancellationToken cancellationToken)
            : base(client, logger, cacheTTL, cancellationToken)
        {
            _request = request;
        }

        /// <summary>
        /// Returns the next iterator until completed.
        /// </summary>
        protected override List<KeyItem> GetNextPage(out bool isCompleted)
        {
            var results = _client.ListKeysAsync(_request, _cacheTTL, _cancellationToken).GetAwaiter().GetResult();
            if (results.IsSuccessStatusCode)
            {
                _request.StartApplicationKeyId = results.Response.NextApplicationKeyId;
                isCompleted = string.IsNullOrEmpty(results.Response.NextApplicationKeyId);
                return results.Response.Keys;
            }
            else
            {
                isCompleted = true;
                return new List<KeyItem>();
            }
        }
    }
}
