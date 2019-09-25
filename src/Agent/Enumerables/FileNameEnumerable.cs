using System;
using System.Threading;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

using Bytewizer.Backblaze.Client;
using Bytewizer.Backblaze.Models;

namespace Bytewizer.Backblaze.Adapters
{
    /// <summary>
    /// Iterates sequentially through the <see cref="ListFileNamesResponse"/> elements.
    /// </summary>
    public class FileNameEnumerable : BaseIterator<FileItem>
    {
        /// <summary>
        /// The request to send.
        /// </summary>
        private readonly ListFileNamesRequest _request;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileNameEnumerable"/> class.
        /// </summary>
        public FileNameEnumerable(IApiClient client, ILogger logger, ListFileNamesRequest request, TimeSpan cacheTTL, CancellationToken cancellationToken)
            : base(client, logger, cacheTTL, cancellationToken)
        {
            _request = request;
        }

        /// <summary>
        /// Returns the next iterator until completed.
        /// </summary>
        protected override List<FileItem> GetNextPage(out bool isCompleted)
        {
            var results = _client.ListFileNamesAsync(_request, _cacheTTL, _cancellationToken).GetAwaiter().GetResult();
            if (results.IsSuccessStatusCode)
            {
                _logger.LogDebug($"File name adapter sent request for {_request.MaxFileCount} files including a next file name of '{_request.StartFileName}'");
                _request.StartFileName = results.Response.NextFileName;
                isCompleted = string.IsNullOrEmpty(results.Response.NextFileName);
                return results.Response.Files;
            }
            else
            {
                _logger.LogError($"File name adapter failed sending request with error: {results.Error.Message}");
                isCompleted = true;
                return new List<FileItem>();
            }
        }
    }
}
