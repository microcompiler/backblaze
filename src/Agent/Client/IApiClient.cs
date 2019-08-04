using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Bytewizer.Backblaze.Models;

namespace Bytewizer.Backblaze.Client
{
    public interface IApiClient
    { 
        AccountInfo AccountInfo { get; }
        AuthToken AuthToken { get;}
        string TestMode { get; set; }
        int RetryCount { get; set; }
        long UploadCutoffSize { get; set; }
        long UploadPartSize { get; set; }
        long DownloadCutoffSize { get; set; }
        long DownloadPartSize { get; set; }
        void Connect(string accountId, string applicationKey);
        Task ConnectAsync(string applicationKeyId, string applicationKey);
        Task<IApiResults<UploadFileResponse>> UploadAsync(UploadFileByBucketIdRequest request, Stream content, IProgress<ICopyProgress> progress, CancellationToken cancel);
        Task<IApiResults<DownloadFileResponse>> DownloadAsync(DownloadFileByIdRequest request, Stream content, IProgress<ICopyProgress> progress, CancellationToken cancel);
        Task<IApiResults<DownloadFileResponse>> DownloadAsync(DownloadFileByNameRequest request, Stream content, IProgress<ICopyProgress> progress, CancellationToken cancel);
        Task<IApiResults<AuthorizeAccountResponse>> AuthorizeAccountAync(string accountId, string applicationKey, CancellationToken cancellationToken);
        Task<IApiResults<CancelLargeFileResponse>> CancelLargeFileAsync(CancelLargeFileRequest request, CancellationToken cancellationToken);
        Task<IApiResults<CreateBucketResponse>> CreateBucketAsync(CreateBucketRequest request, CancellationToken cancellationToken);
        Task<IApiResults<CreateKeyResponse>> CreateKeyAsync(CreateKeyRequest request, CancellationToken cancellationToken);
        Task<IApiResults<DeleteBucketResponse>> DeleteBucketAsync(DeleteBucketRequest request, CancellationToken cancellationToken);
        Task<IApiResults<DeleteFileVersionResponse>> DeleteFileVersionAsync(DeleteFileVersionRequest request, CancellationToken cancellationToken);
        Task<IApiResults<DeleteKeyResponse>> DeleteKeyAsync(DeleteKeyRequest request, CancellationToken cancellationToken);
        Task<IApiResults<DownloadFileResponse>> DownloadFileByIdAsync(DownloadFileByIdRequest request, CancellationToken cancellationToken);
        Task<IApiResults<DownloadFileResponse>> DownloadFileByIdAsync(DownloadFileByIdRequest request, Stream content, IProgress<ICopyProgress> progress, CancellationToken cancellationToken);
        Task<IApiResults<DownloadFileResponse>> DownloadFileByNameAsync(DownloadFileByNameRequest request, CancellationToken cancellationToken);
        Task<IApiResults<DownloadFileResponse>> DownloadFileByNameAsync(DownloadFileByNameRequest request, Stream content, IProgress<ICopyProgress> progress, CancellationToken cancellationToken);
        Task<IApiResults<UploadFileResponse>> FinishLargeFileAsync(FinishLargeFileRequest request, CancellationToken cancellationToken);
        Task<IApiResults<GetDownloadAuthorizationResponse>> GetDownloadAuthorizationAsync(GetDownloadAuthorizationRequest request, CancellationToken cancellationToken);
        Task<IApiResults<GetFileInfoResponse>> GetFileInfoAsync(GetFileInfoRequest request, CancellationToken cancellationToken);
        Task<IApiResults<GetUploadPartUrlResponse>> GetUploadPartUrlAsync(GetUploadPartUrlRequest request, CancellationToken cancellationToken);
        Task<IApiResults<GetUploadUrlResponse>> GetUploadUrlAsync(GetUploadUrlRequest request, CancellationToken cancellationToken);
        Task<IApiResults<HideFileResponse>> HideFileAsync(HideFileRequest request, CancellationToken cancellationToken);
        Task<IApiResults<ListBucketsResponse>> ListBucketsAsync(ListBucketsRequest request, CancellationToken cancellationToken);
        Task<IApiResults<ListFileNamesResponse>> ListFileNamesAsync(ListFileNamesRequest request, CancellationToken cancellationToken);
        Task<IApiResults<ListFileVersionResponse>> ListFileVersionsAsync(ListFileVersionRequest request, CancellationToken cancellationToken);
        Task<IApiResults<ListKeysResponse>> ListKeysAsync(ListKeysRequest request, CancellationToken cancellationToken);
        Task<IApiResults<ListPartsResponse>> ListPartsAsync(ListPartsRequest request, CancellationToken cancellationToken);
        Task<IApiResults<ListUnfinishedLargeFilesResponse>> ListUnfinishedLargeFilesAsync(ListUnfinishedLargeFilesRequest request, CancellationToken cancellationToken);
        Task<IApiResults<StartLargeFileResponse>> StartLargeFileAsync(StartLargeFileRequest request, CancellationToken cancellationToken);
        Task<IApiResults<UpdateBucketResponse>> UpdateBucketAsync(UpdateBucketRequest request, CancellationToken cancellationToken);
        Task<IApiResults<UploadFileResponse>> UploadFileAsync(UploadFileRequest request, IProgress<ICopyProgress> progress, CancellationToken cancellationToken);
        Task<IApiResults<UploadPartResponse>> UploadPartAsync(UploadPartRequest request, IProgress<ICopyProgress> progress, CancellationToken cancellationToken);
        bool IsDisposed { get; }
        void Dispose();
    }
}