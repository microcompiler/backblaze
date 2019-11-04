using System;
using System.Threading;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

using Bytewizer.Backblaze.Client;
using Bytewizer.Backblaze.Models;

namespace Bytewizer.Backblaze.Enumerables
{
    /// <summary>
    /// Iterates sequentially through the <see cref="ListUnfinishedLargeFilesRequest"/> elements.
    /// </summary>
    public class UnfinishedEnumerable : BaseIterator<FileItem>
    {
        /// <summary>
        /// The request to send.
        /// </summary>
        private readonly ListUnfinishedLargeFilesRequest _request;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnfinishedEnumerable"/> class.
        /// </summary>
        public UnfinishedEnumerable(IApiClient client, ILogger logger, ListUnfinishedLargeFilesRequest request, TimeSpan cacheTTL, CancellationToken cancellationToken)
            : base(client, logger, cacheTTL, cancellationToken)
        {
            _request = request;
        }

        /// <summary>
        /// Returns the next iterator until completed.
        /// </summary>
        protected override List<FileItem> GetNextPage(out bool isCompleted)
        {
            var results = _client.ListUnfinishedLargeFilesAsync(_request, _cacheTTL, _cancellationToken).GetAwaiter().GetResult();
            
            if (results.IsSuccessStatusCode)
            {
                isCompleted = string.IsNullOrEmpty(results.Response.NextFileId);
                
                if (isCompleted)
                {
                    _logger.LogDebug($"'{GetType().Name}' sent request for {_request.MaxFileCount} files");
                }
                else
                {
                    _logger.LogDebug($"'{GetType().Name}' sent request for {_request.MaxFileCount} files including a next file name of '{_request.StartFileId}'");
                }

                _request.StartFileId = results.Response.NextFileId;
               
                return results.Response.Files;
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
