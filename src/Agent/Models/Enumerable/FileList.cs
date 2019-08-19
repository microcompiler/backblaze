using System.Threading;
using System.Collections.Generic;

using Bytewizer.Backblaze.Client;

namespace Bytewizer.Backblaze.Models
{
    public class FileList : BaseList<FileObject>
    {
        private readonly IApiClient _client;
        private readonly ListFileNamesRequest _request;

        public FileList(IApiClient client, ListFileNamesRequest request)
            : base (client)
        {
            _client = client;
            _request = request;
        }

        protected override List<FileObject> GetNextPage(out bool isDone)
        {
            var results = _client.ListFileNamesAsync(_request, CancellationToken.None).GetAwaiter().GetResult();
            if (results.IsSuccessStatusCode)
            {
                _request.StartFileName = results.Response.NextFileName;
                isDone = _request.StartFileName == null;
                return results.Response.Files;
            }
            else
            {
                isDone = true;
                return new List<FileObject>();
            }
        }
    }
}
