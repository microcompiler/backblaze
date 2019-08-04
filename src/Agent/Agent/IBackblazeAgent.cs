using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Bytewizer.Backblaze.Client;
using Bytewizer.Backblaze.Models;

namespace Bytewizer.Backblaze.Agent
{
    public interface IBackblazeAgent
    {
        IBackblazeFilesAgent Files { get; }
        IBackblazeBucketsAgent Buckets { get; }
        IBackblazeKeysAgent Keys { get; }
        IBackblazePartsAgent Parts { get; }

        string AccountId { get; }

        bool IsDisposed { get; }
        void Dispose();

        Task<IApiResults<UploadFileResponse>> UploadAsync(string bucketId, string fileName, Stream content);
        Task<IApiResults<UploadFileResponse>> UploadAsync(string bucketId, string fileName, Stream content, IProgress<ICopyProgress> progress);
        Task<IApiResults<UploadFileResponse>> UploadAsync(string bucketId, string fileName, Stream content, CancellationToken cancel);
        Task<IApiResults<UploadFileResponse>> UploadAsync(string bucketId, string fileName, Stream content, IProgress<ICopyProgress> progress, CancellationToken cancel);
        Task<IApiResults<UploadFileResponse>> UploadAsync(UploadFileByBucketIdRequest request, Stream content);
        Task<IApiResults<UploadFileResponse>> UploadAsync(UploadFileByBucketIdRequest request, Stream content, IProgress<ICopyProgress> progress);
        Task<IApiResults<UploadFileResponse>> UploadAsync(UploadFileByBucketIdRequest request, Stream content, CancellationToken cancel);
        Task<IApiResults<UploadFileResponse>> UploadAsync(UploadFileByBucketIdRequest request, Stream content, IProgress<ICopyProgress> progress, CancellationToken cancel);
        Task<IApiResults<DownloadFileResponse>> DownloadAsync(string fileId, Stream content);
        Task<IApiResults<DownloadFileResponse>> DownloadAsync(string fileId, Stream content, IProgress<ICopyProgress> progress);
        Task<IApiResults<DownloadFileResponse>> DownloadAsync(string fileId, Stream content, CancellationToken cancel);
        Task<IApiResults<DownloadFileResponse>> DownloadAsync(string fileId, Stream content, IProgress<ICopyProgress> progress, CancellationToken cancel);
        Task<IApiResults<DownloadFileResponse>> DownloadAsync(DownloadFileByIdRequest request, Stream content);
        Task<IApiResults<DownloadFileResponse>> DownloadAsync(DownloadFileByIdRequest request, Stream content, IProgress<ICopyProgress> progress);
        Task<IApiResults<DownloadFileResponse>> DownloadAsync(DownloadFileByIdRequest request, Stream content, CancellationToken cancel);
        Task<IApiResults<DownloadFileResponse>> DownloadAsync(DownloadFileByIdRequest request, Stream content, IProgress<ICopyProgress> progress, CancellationToken cancel);
        Task<IApiResults<DownloadFileResponse>> DownloadAsync(string bucketName, string fileName, Stream content);
        Task<IApiResults<DownloadFileResponse>> DownloadAsync(string bucketName, string fileName, Stream content, IProgress<ICopyProgress> progress);
        Task<IApiResults<DownloadFileResponse>> DownloadAsync(string bucketName, string fileName, Stream content, CancellationToken cancel);
        Task<IApiResults<DownloadFileResponse>> DownloadAsync(string bucketName, string fileName, Stream content, IProgress<ICopyProgress> progress, CancellationToken cancel);
        Task<IApiResults<DownloadFileResponse>> DownloadAsync(DownloadFileByNameRequest request, Stream content);
        Task<IApiResults<DownloadFileResponse>> DownloadAsync(DownloadFileByNameRequest request, Stream content, IProgress<ICopyProgress> progress);
        Task<IApiResults<DownloadFileResponse>> DownloadAsync(DownloadFileByNameRequest request, Stream content, CancellationToken cancel);
        Task<IApiResults<DownloadFileResponse>> DownloadAsync(DownloadFileByNameRequest request, Stream content, IProgress<ICopyProgress> progress, CancellationToken cancel);
    }
}