using System.Threading;
using System.Collections.Generic;

using Bytewizer.Backblaze.Client;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Iterates sequentially through the <see cref="ListKeysResponse"/> response elements.
    /// </summary>
    public class Keys : BaseIterator<KeyItem>
    {
        /// <summary>
        /// The request to send.
        /// </summary>
        private readonly ListKeysRequest _request;

        /// <summary>
        /// Initializes a new instance of the <see cref="Keys"/> class.
        /// </summary>
        public Keys(IApiClient client, ListKeysRequest request, int cacheTTL, CancellationToken cancellationToken)
            : base(client, cacheTTL, cancellationToken)
        {
            _request = request;
        }

        /// <summary>
        /// Returns the next iterator until completed.
        /// </summary>
        protected override List<KeyItem> GetNextPage(out bool isCompleted)
        {
            var results = _client.ListKeysAsync(_request, _cacheManagerTTL, _cancellationToken).GetAwaiter().GetResult();
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
