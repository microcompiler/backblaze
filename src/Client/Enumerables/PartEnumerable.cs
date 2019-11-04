using System;
using System.Threading;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

using Bytewizer.Backblaze.Client;
using Bytewizer.Backblaze.Models;

namespace Bytewizer.Backblaze.Enumerables
{
    /// <summary>
    /// Iterates sequentially through the <see cref="ListPartsResponse"/> elements.
    /// </summary>
    public class PartEnumerable : BaseIterator<PartItem>
    {
        /// <summary>
        /// The request to send.
        /// </summary>
        private readonly ListPartsRequest _request;

        /// <summary>
        /// Initializes a new instance of the <see cref="PartEnumerable"/> class.
        /// </summary>
        public PartEnumerable(IApiClient client, ILogger logger, ListPartsRequest request, TimeSpan cacheTTL, CancellationToken cancellationToken)
            : base(client, logger, cacheTTL, cancellationToken)
        {
            _request = request;
        }

        /// <summary>
        /// Returns the next iterator until completed.
        /// </summary>
        protected override List<PartItem> GetNextPage(out bool isCompleted)
        {
            var results = _client.ListPartsAsync(_request, _cacheTTL, _cancellationToken).GetAwaiter().GetResult();

            if (results.IsSuccessStatusCode)
            {
                isCompleted = string.IsNullOrEmpty(results.Response.NextPartNumber);
                
                if (isCompleted)
                {
                    _logger.LogDebug($"'{GetType().Name}' sent request for {_request.MaxPartCount} parts");
                }
                else
                {
                    _logger.LogDebug($"'{GetType().Name}' sent request for {_request.MaxPartCount} parts including a next part number of '{_request.StartPartNumber}'");
                }

                _request.StartPartNumber = results.Response.NextPartNumber;
         
                return results.Response.Parts;
            }
            else
            {
                _logger.LogError($"'{GetType().Name}' failed sending request with error: {results.Error?.Message}");
                isCompleted = true;
                
                return default;
            }
        }
    }
}
