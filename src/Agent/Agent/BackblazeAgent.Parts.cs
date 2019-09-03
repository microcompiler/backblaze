using System.Threading.Tasks;
using System.Collections.Generic;

using Bytewizer.Backblaze.Models;

namespace Bytewizer.Backblaze.Agent
{
    public partial class BackblazeAgent : IBackblazePartsAgent
    {
        public IBackblazePartsAgent Parts { get { return this; } }

        async Task<IApiResults<ListPartsResponse>> IBackblazePartsAgent.GetAsync(ListPartsRequest request)
        {
            return await InvokePolicy.ExecuteAsync(() => _client.ListPartsAsync(request, cancellationToken));
        }

        async Task<IApiResults<ListPartsResponse>> IBackblazePartsAgent.GetAsync(string fileId)
        {
            var request = new ListPartsRequest(fileId);
            return await InvokePolicy.ExecuteAsync(() => _client.ListPartsAsync(request, cancellationToken));
        }

        async Task<IApiResults<GetUploadPartUrlResponse>> IBackblazePartsAgent.GetUploadUrlAsync(string fileId)
        {
            var request = new GetUploadPartUrlRequest(fileId);
            return await InvokePolicy.ExecuteAsync(() => _client.GetUploadPartUrlAsync(request, cancellationToken));
        }

        async Task<IApiResults<StartLargeFileResponse>> IBackblazePartsAgent.StartLargeFileAsync(StartLargeFileRequest request)
        {
            return await InvokePolicy.ExecuteAsync(() => _client.StartLargeFileAsync(request, cancellationToken));
        }

        async Task<IApiResults<StartLargeFileResponse>> IBackblazePartsAgent.StartLargeFileAsync(string bucketId, string fileName)
        {
            var request = new StartLargeFileRequest(bucketId, fileName);
            return await InvokePolicy.ExecuteAsync(() => _client.StartLargeFileAsync(request, cancellationToken));
        }

        async Task<IApiResults<UploadFileResponse>> IBackblazePartsAgent.FinishLargeFileAsync(string fileId, List<string> partSha1Array)
        {
            var request = new FinishLargeFileRequest(fileId, partSha1Array);
            return await InvokePolicy.ExecuteAsync(() => _client.FinishLargeFileAsync(request, cancellationToken));
        }

        async Task<IApiResults<CancelLargeFileResponse>> IBackblazePartsAgent.CancelLargeFileAsync(string fileId)
        {
            var request = new CancelLargeFileRequest(fileId);
            return await InvokePolicy.ExecuteAsync(() => _client.CancelLargeFileAsync(request, cancellationToken));
        }
    }
}
