using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Bytewizer.Backblaze.Models;

namespace Bytewizer.Backblaze.Agent
{
    public partial class BackblazeAgent : IBackblazePartsAgent
    {
        public IBackblazePartsAgent Parts { get { return this; } }

        public Task<List<PartItem>> ListAsync(ListPartsRequest request, int cacheTTL)
        {
            return Task.FromResult(new Parts(_client, request, cacheTTL, cancellationToken).ToList());
        }

        async Task<IApiResults<ListPartsResponse>> IBackblazePartsAgent.GetAsync(ListPartsRequest request)
        {
            return await  _client.ListPartsAsync(request, cancellationToken);
        }

        async Task<IApiResults<ListPartsResponse>> IBackblazePartsAgent.GetAsync(string fileId)
        {
            var request = new ListPartsRequest(fileId);
            return await _client.ListPartsAsync(request, cancellationToken);
        }

        async Task<IApiResults<GetUploadPartUrlResponse>> IBackblazePartsAgent.GetUploadUrlAsync(string fileId)
        {
            var request = new GetUploadPartUrlRequest(fileId);
            return await _client.GetUploadPartUrlAsync(request, cancellationToken);
        }

        async Task<IApiResults<StartLargeFileResponse>> IBackblazePartsAgent.StartLargeFileAsync(StartLargeFileRequest request)
        {
            return await _client.StartLargeFileAsync(request, cancellationToken);
        }

        async Task<IApiResults<StartLargeFileResponse>> IBackblazePartsAgent.StartLargeFileAsync(string bucketId, string fileName)
        {
            var request = new StartLargeFileRequest(bucketId, fileName);
            return await _client.StartLargeFileAsync(request, cancellationToken);
        }

        async Task<IApiResults<UploadFileResponse>> IBackblazePartsAgent.FinishLargeFileAsync(string fileId, List<string> partSha1Array)
        {
            var request = new FinishLargeFileRequest(fileId, partSha1Array);
            return await _client.FinishLargeFileAsync(request, cancellationToken);
        }

        async Task<IApiResults<CancelLargeFileResponse>> IBackblazePartsAgent.CancelLargeFileAsync(string fileId)
        {
            var request = new CancelLargeFileRequest(fileId);
            return await _client.CancelLargeFileAsync(request, cancellationToken);
        }

        async Task<IApiResults<CopyPartResponse>> IBackblazePartsAgent.CopyAsync(CopyPartRequest request)
        {
            return await _client.CopyPartAsync(request, cancellationToken);
        }

        async Task<IApiResults<CopyPartResponse>> IBackblazePartsAgent.CopyAsync(string sourceFileId, string largeFileId, int partNumber)
        {
            var request = new CopyPartRequest(sourceFileId, largeFileId, partNumber);
            return await _client.CopyPartAsync(request, cancellationToken);
        }
    }
}
