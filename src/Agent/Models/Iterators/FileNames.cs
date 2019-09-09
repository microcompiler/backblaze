using System.Threading;
using System.Collections.Generic;

using Bytewizer.Backblaze.Client;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Iterates sequentially through the <see cref="ListFileNamesResponse"/> response elements.
    /// </summary>
    public class FileNames : BaseIterator<FileItem>
    {
        /// <summary>
        /// The request to send.
        /// </summary>
        private readonly ListFileNamesRequest _request;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileNames"/> class.
        /// </summary>
        public FileNames(IApiClient client, ListFileNamesRequest request, int cacheTTL, CancellationToken cancellationToken)
            : base(client, cacheTTL, cancellationToken)
        {
            _request = request;
        }

        /// <summary>
        /// Returns the next iterator until completed.
        /// </summary>
        protected override List<FileItem> GetNextPage(out bool isCompleted)
        {
            var results = _client.ListFileNamesAsync(_request, _cacheManagerTTL, _cancellationToken).GetAwaiter().GetResult();
            if (results.IsSuccessStatusCode)
            {
                _request.StartFileName = results.Response.NextFileName;
                isCompleted = string.IsNullOrEmpty(results.Response.NextFileName);
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
