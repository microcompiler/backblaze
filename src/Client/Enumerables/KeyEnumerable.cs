using System;
using System.Threading;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

using Bytewizer.Backblaze.Client;
using Bytewizer.Backblaze.Models;

namespace Bytewizer.Backblaze.Enumerables
{
    /// <summary>
    /// Iterates sequentially through the <see cref="ListKeysResponse"/> elements.
    /// </summary>
    public class KeyEnumerable : BaseIterator<KeyItem>
    {
        /// <summary>
        /// The request to send.
        /// </summary>
        private readonly ListKeysRequest _request;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyEnumerable"/> class.
        /// </summary>
        public KeyEnumerable(IApiClient client, ILogger logger, ListKeysRequest request, TimeSpan cacheTTL, CancellationToken cancellationToken)
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
                isCompleted = string.IsNullOrEmpty(results.Response.NextApplicationKeyId);
                
                if (isCompleted)
                {
                    _logger.LogDebug($"'{GetType().Name}' sent request for {_request.MaxKeyCount} keys");
                }
                else
                {
                    _logger.LogDebug($"'{GetType().Name}' sent request for {_request.MaxKeyCount} keys including a next key id of '{_request.StartApplicationKeyId}'");
                }

                _request.StartApplicationKeyId = results.Response.NextApplicationKeyId;
                
                return results.Response.Keys;
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
