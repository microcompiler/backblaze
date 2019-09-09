using System.Threading;
using System.Collections.Generic;

using Bytewizer.Backblaze.Client;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Iterates sequentially through the <see cref="ListPartsResponse"/> response elements.
    /// </summary>
    public class Parts : BaseIterator<PartItem>
    {
        /// <summary>
        /// The request to send.
        /// </summary>
        private readonly ListPartsRequest _request;

        /// <summary>
        /// Initializes a new instance of the <see cref="Parts"/> class.
        /// </summary>
        public Parts(IApiClient client, ListPartsRequest request, int cacheTTL, CancellationToken cancellationToken)
            : base(client, cacheTTL, cancellationToken)
        {
            _request = request;
        }

        /// <summary>
        /// Returns the next iterator until completed.
        /// </summary>
        protected override List<PartItem> GetNextPage(out bool isCompleted)
        {
            var results = _client.ListPartsAsync(_request, _cacheManagerTTL, _cancellationToken).GetAwaiter().GetResult();
            if (results.IsSuccessStatusCode)
            {
                _request.StartPartNumber = results.Response.NextPartNumber;
                isCompleted = string.IsNullOrEmpty(results.Response.NextPartNumber);
                return results.Response.Parts;
            }
            else
            {
                isCompleted = true;
                return new List<PartItem>();
            }
        }
    }
}
