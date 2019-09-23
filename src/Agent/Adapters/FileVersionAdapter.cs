using System;
using System.Threading;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

using Bytewizer.Backblaze.Client;
using Bytewizer.Backblaze.Models;

namespace Bytewizer.Backblaze.Adapters
{
    /// <summary>
    /// Iterates sequentially through the <see cref="ListFileVersionRequest"/> elements.
    /// </summary>
    public class FileVersionAdapter : BaseIterator<FileItem>
    {
        /// <summary>
        /// The request to send.
        /// </summary>
        private readonly ListFileVersionRequest _request;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileVersionAdapter"/> class.
        /// </summary>
        public FileVersionAdapter(IApiClient client, ILogger logger, ListFileVersionRequest request, TimeSpan cacheTTL, CancellationToken cancellationToken)
            : base(client, logger, cacheTTL, cancellationToken)
        {
            _request = request;
        }

        /// <summary>
        /// Returns the next iterator until completed.
        /// </summary>
        protected override List<FileItem> GetNextPage(out bool isCompleted)
        {
            var results = _client.ListFileVersionsAsync(_request, CancellationToken.None).GetAwaiter().GetResult();
            if (results.IsSuccessStatusCode)
            {
                _logger.LogDebug($"File version adapter sent request for {_request.MaxFileCount} files including a next file name of '{_request.StartFileName}'");
                _request.StartFileName = results.Response.NextFileName;
                isCompleted = string.IsNullOrEmpty(results.Response.NextFileName);
                return results.Response.Files;
            }
            else
            {
                _logger.LogError($"File version adapter failed sending request with error: {results.Error.Message}");
                isCompleted = true;
                return new List<FileItem>();
            }
        }
    }
}
