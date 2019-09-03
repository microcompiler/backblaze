using System.Threading;
using System.Collections.Generic;

using Bytewizer.Backblaze.Client;

namespace Bytewizer.Backblaze.Models
{
    public class FileVersions : BaseList<FileItem>
    {
        private readonly IApiClient _client;
        private readonly ListFileVersionRequest _request;

        public FileVersions(IApiClient client, ListFileVersionRequest request)
            : base (client)
        {
            _client = client;
            _request = request;
        }

        protected override List<FileItem> GetNextPage(out bool isDone)
        {
            var results = _client.ListFileVersionsAsync(_request, CancellationToken.None).GetAwaiter().GetResult();
            if (results.IsSuccessStatusCode)
            {
                _request.StartFileName = results.Response.NextFileName;
                isDone = _request.StartFileName == null;
                return results.Response.Files;
            }
            else
            {
                isDone = true;
                return new List<FileItem>();
            }
        }
    }
}
