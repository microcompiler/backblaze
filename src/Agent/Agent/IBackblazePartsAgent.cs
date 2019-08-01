using System.Threading.Tasks;
using System.Collections.Generic;

using Bytewizer.Backblaze.Models;

namespace Bytewizer.Backblaze.Agent
{
    public interface IBackblazePartsAgent
    {
        Task<IApiResults<ListPartsResponse>> GetAsync(string fileId);
        Task<IApiResults<ListPartsResponse>> GetAsync(ListPartsRequest request);
        Task<IApiResults<GetUploadPartUrlResponse>> GetUploadUrlAsync(string fileId);
        Task<IApiResults<StartLargeFileResponse>> StartLargeFileAsync(StartLargeFileRequest request);
        Task<IApiResults<StartLargeFileResponse>> StartLargeFileAsync(string bucketId, string fileName);
        Task<IApiResults<UploadFileResponse>> FinishLargeFileAsync(string fileId, List<string> partSha1Array);
        Task<IApiResults<CancelLargeFileResponse>> CancelLargeFileAsync(string fileId);
    }
}