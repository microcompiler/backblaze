using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Bytewizer.Backblaze.Models;
using Bytewizer.Backblaze.Adapters;

namespace Bytewizer.Backblaze.Storage
{
    public partial class BackblazeStorage : IBackblazeParts
    {
        public IBackblazeParts Parts { get { return this; } }


        public PartAdapter List(ListPartsRequest request, int cacheTTL)
        {
            return new PartAdapter(_client, request, cacheTTL, cancellationToken);
        }

        public Task<List<PartItem>> ListAsync(ListPartsRequest request, int cacheTTL)
        {
            return Task.FromResult(new PartAdapter(_client, request, cacheTTL, cancellationToken).ToList());
        }

        async Task<IApiResults<ListPartsResponse>> IBackblazeParts.GetAsync(ListPartsRequest request)
        {
            return await  _client.ListPartsAsync(request, cancellationToken);
        }

        async Task<IApiResults<ListPartsResponse>> IBackblazeParts.GetAsync(string fileId)
        {
            var request = new ListPartsRequest(fileId);
            return await _client.ListPartsAsync(request, cancellationToken);
        }

        async Task<IApiResults<GetUploadPartUrlResponse>> IBackblazeParts.GetUploadUrlAsync(string fileId)
        {
            var request = new GetUploadPartUrlRequest(fileId);
            return await _client.GetUploadPartUrlAsync(request, cancellationToken);
        }

        async Task<IApiResults<StartLargeFileResponse>> IBackblazeParts.StartLargeFileAsync(StartLargeFileRequest request)
        {
            return await _client.StartLargeFileAsync(request, cancellationToken);
        }

        async Task<IApiResults<StartLargeFileResponse>> IBackblazeParts.StartLargeFileAsync(string bucketId, string fileName)
        {
            var request = new StartLargeFileRequest(bucketId, fileName);
            return await _client.StartLargeFileAsync(request, cancellationToken);
        }

        async Task<IApiResults<UploadFileResponse>> IBackblazeParts.FinishLargeFileAsync(string fileId, List<string> partSha1Array)
        {
            var request = new FinishLargeFileRequest(fileId, partSha1Array);
            return await _client.FinishLargeFileAsync(request, cancellationToken);
        }

        async Task<IApiResults<CancelLargeFileResponse>> IBackblazeParts.CancelLargeFileAsync(string fileId)
        {
            var request = new CancelLargeFileRequest(fileId);
            return await _client.CancelLargeFileAsync(request, cancellationToken);
        }

        async Task<IApiResults<CopyPartResponse>> IBackblazeParts.CopyAsync(CopyPartRequest request)
        {
            return await _client.CopyPartAsync(request, cancellationToken);
        }

        async Task<IApiResults<CopyPartResponse>> IBackblazeParts.CopyAsync(string sourceFileId, string largeFileId, int partNumber)
        {
            var request = new CopyPartRequest(sourceFileId, largeFileId, partNumber);
            return await _client.CopyPartAsync(request, cancellationToken);
        }
    }
}
