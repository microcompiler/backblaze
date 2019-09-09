using System.Threading;
using System.Collections.Generic;

using Bytewizer.Backblaze.Client;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Iterates sequentially through the <see cref="ListUnfinishedLargeFilesRequest"/> response elements.
    /// </summary>
    public class UnfinishedLargeFiles : BaseIterator<FileItem>
    {
        /// <summary>
        /// The request to send.
        /// </summary>
        private readonly ListUnfinishedLargeFilesRequest _request;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnfinishedLargeFiles"/> class.
        /// </summary>
        public UnfinishedLargeFiles(IApiClient client, ListUnfinishedLargeFilesRequest request, int cacheTTL, CancellationToken cancellationToken)
            : base(client, cacheTTL, cancellationToken)
        {
            _request = request;
        }

        /// <summary>
        /// Returns the next iterator until completed.
        /// </summary>
        protected override List<FileItem> GetNextPage(out bool isCompleted)
        {
            var results = _client.ListUnfinishedLargeFilesAsync(_request, _cacheManagerTTL, _cancellationToken).GetAwaiter().GetResult();
            if (results.IsSuccessStatusCode)
            {
                _request.StartFileId = results.Response.NextFileId;
                isCompleted = string.IsNullOrEmpty(results.Response.NextFileId);
                return results.Response.Files;
            }
            else
            {
                isCompleted = true;
                return new List<FileItem>();
            }
        }
    }
}
