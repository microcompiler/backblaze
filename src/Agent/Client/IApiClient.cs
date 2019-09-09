using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Authentication;

using Bytewizer.Backblaze.Models;

namespace Bytewizer.Backblaze.Client
{
    public interface IApiClient
    { 
        #region Lifetime

        /// <summary>
        /// Frees resources owned by this instance.
        /// </summary>
        /// <param name="disposing">
        /// True when called via <see cref="IDisposable.Dispose()"/>, false when called from the finalizer.
        /// </param>
        void Dispose();

        /// <summary>
        /// Indicates this instance has been disposed.
        /// </summary>
        bool IsDisposed { get; }

        #endregion

        #region Public Properties

        /// <summary>
        /// Client options for the Backblaze B2 Cloud Storage service.
        /// </summary>
        ClientOptions Options { get; set; } 

        /// <summary>
        /// The account information returned from the Backblaze B2 Cloud Storage service.
        /// </summary>
        AccountInfo AccountInfo { get; }

        /// <summary>
        /// The authorization token to use with all calls other than <see cref="AuthorizeAccountAync"/>. 
        /// This authorization token is valid for at most 24 hours.
        /// </summary>
        AuthToken AuthToken { get; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Connect to Backblaze B2 Cloud Storage service and initialize <see cref="AccountInfo"/>.
        /// </summary>
        /// <param name="keyId">The identifier for the key.</param>
        /// <param name="applicationKey">The secret part of the key. You can use either the master application key or a normal application key.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        void Connect(string keyId, string applicationKey);

        /// <summary>
        /// Connect to Backblaze B2 Cloud Storage service and initialize <see cref="AccountInfo"/>.
        /// </summary>
        /// <param name="keyId">The identifier for the key.</param>
        /// <param name="applicationKey">The secret part of the key. You can use either the master application key or a normal application key.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task ConnectAsync(string keyId, string applicationKey);

        /// <summary>
        /// Upload content stream to the Backblaze B2 Cloud Storage service. 
        /// </summary>
        /// <param name="request">The upload file request to send.</param>
        /// <param name="content"> The content stream of the content payload.</param>
        /// <param name="progress">A progress action which fires every time the write buffer is cycled.</param>
        /// <param name="cancel">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="CapExceededExecption">Thrown when a cap is exceeded or an account in bad standing.</exception>
        /// <exception cref="InvalidHashException">Thrown when a validation hash is not valid.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<UploadFileResponse>> UploadAsync(UploadFileByBucketIdRequest request, Stream content, IProgress<ICopyProgress> progress, CancellationToken cancel);

        /// <summary>
        /// Download a specific version of content by bucket and file name from the Backblaze B2 Cloud Storage service. 
        /// </summary>
        /// <param name="request">The download file request to send.</param>
        /// <param name="content">The download content to receive.</param>
        /// <param name="progress">A progress action which fires every time the write buffer is cycled.</param>
        /// <param name="cancel">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="CapExceededExecption">Thrown when a cap is exceeded or an account in bad standing.</exception>
        /// <exception cref="InvalidHashException">Thrown when a validation hash is not valid.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<DownloadFileResponse>> DownloadAsync(DownloadFileByNameRequest request, Stream content, IProgress<ICopyProgress> progress, CancellationToken cancel);

        /// <summary>
        /// Download a specific version of content by file id from the Backblaze B2 Cloud Storage service. 
        /// </summary>
        /// <param name="request">The download file request to send.</param>
        /// <param name="content">The download content to receive.</param>
        /// <param name="progress">A progress action which fires every time the write buffer is cycled.</param>
        /// <param name="cancel">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="CapExceededExecption">Thrown when a cap is exceeded or an account in bad standing.</exception>
        /// <exception cref="InvalidHashException">Thrown when a validation hash is not valid.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<DownloadFileResponse>> DownloadByIdAsync(DownloadFileByIdRequest request, Stream content, IProgress<ICopyProgress> progress, CancellationToken cancel);

        #endregion

        #region Client Endpoints

        /// <summary>
        /// Authenticate to the Backblaze B2 Cloud Storage service.
        /// </summary>
        /// <param name="keyId">The identifier for the key.</param>
        /// <param name="applicationKey">The secret part of the key. You can use either the master application key or a normal application key.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        /// <returns>
		/// The <see cref="AuthorizeAccountResponse" /> of this <see cref="IApiResults{T}.Response"/> value, or <see cref="null"/>, if the response was was error data.
		/// </returns>
        Task<IApiResults<AuthorizeAccountResponse>> AuthorizeAccountAync(string keyId, string applicationKey, CancellationToken cancellationToken);

        /// <summary>
        /// Cancels the upload of a large file and deletes all parts that have been uploaded. 
        /// </summary>
        /// <param name="request">The cancel large file request to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<CancelLargeFileResponse>> CancelLargeFileAsync(CancelLargeFileRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Creates a new file by copying from an existing file.
        /// </summary>
        /// <param name="request">The copy file request to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="CapExceededExecption">Thrown when a cap is exceeded or an account in bad standing.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<CopyFileResponse>> CopyFileAsync(CopyFileRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Creates a new file part by copying from an existing file and storing it as a part of a large file which has already been started.
        /// </summary>
        /// <param name="request">The copy file part request to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="CapExceededExecption">Thrown when a cap is exceeded or an account in bad standing.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<CopyPartResponse>> CopyPartAsync(CopyPartRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Creates a new bucket belonging to the specified account. 
        /// </summary>
        /// <param name="request">The create bucket request to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<CreateBucketResponse>> CreateBucketAsync(CreateBucketRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Creates a new application key. There is a limit of 100 million key creations per account.
        /// </summary>
        /// <param name="request">The create application key request to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<CreateKeyResponse>> CreateKeyAsync(CreateKeyRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes the bucket specified. Only buckets that contain no version of any files can be deleted. 
        /// </summary>
        /// <param name="request">The delete bucket request to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<DeleteBucketResponse>> DeleteBucketAsync(DeleteBucketRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes a specific version of a file. 
        /// </summary>
        /// <param name="request">The delete file request to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<DeleteFileVersionResponse>> DeleteFileVersionAsync(DeleteFileVersionRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes the application key specified. 
        /// </summary>
        /// <param name="request">The delete key request to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<DeleteKeyResponse>> DeleteKeyAsync(DeleteKeyRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Downloads a specific version of a file by file id without content.  
        /// </summary>
        /// <param name="request">The download file request to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<DownloadFileResponse>> DownloadFileByIdAsync(DownloadFileByIdRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Downloads a specific version of a file by file id.  
        /// </summary>
        /// <param name="request">The download file request to send.</param>
        /// <param name="content">The download content to receive.</param>
        /// <param name="progress">A progress action which fires every time the write buffer is cycled.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="InvalidHashException">Thrown when a validation hash is not valid.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<DownloadFileResponse>> DownloadFileByIdAsync(DownloadFileByIdRequest request, Stream content, IProgress<ICopyProgress> progress, CancellationToken cancellationToken);

        /// <summary>
        /// Downloads the most recent version of a file by name without content.
        /// </summary>
        /// <param name="request">The download file request content to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<DownloadFileResponse>> DownloadFileByNameAsync(DownloadFileByNameRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Downloads the most recent version of a file by name. 
        /// </summary>
        /// <param name="request">The download file request to send.</param>
        /// <param name="content">The download content to receive.</param>
        /// <param name="progress">A progress action which fires every time the write buffer is cycled.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="InvalidHashException">Thrown when a validation hash is not valid.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<DownloadFileResponse>> DownloadFileByNameAsync(DownloadFileByNameRequest request, Stream content, IProgress<ICopyProgress> progress, CancellationToken cancellationToken);

        /// <summary>
        /// Converts file parts that have been uploaded into a single file. 
        /// </summary>
        /// <param name="request">The finish large file request to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<UploadFileResponse>> FinishLargeFileAsync(FinishLargeFileRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Generate an authorization token that can be used to download files from a private bucket. 
        /// </summary>
        /// <param name="request">The download authorization request to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<GetDownloadAuthorizationResponse>> GetDownloadAuthorizationAsync(GetDownloadAuthorizationRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Gets information about a file. 
        /// </summary>
        /// <param name="request">The file info request to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<GetFileInfoResponse>> GetFileInfoAsync(GetFileInfoRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Gets a url for uploading parts of a large file. 
        /// </summary>
        /// <param name="request">The upload part url request to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<GetUploadPartUrlResponse>> GetUploadPartUrlAsync(GetUploadPartUrlRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Gets a url for uploading parts of a large file from memory cache. 
        /// </summary>
        /// <param name="request">The upload part url request to send.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now in seconds.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<GetUploadPartUrlResponse>> GetUploadPartUrlAsync(GetUploadPartUrlRequest request, int cacheTTL, CancellationToken cancellationToken);

        /// <summary>
        /// Gets a url for uploading files. 
        /// </summary>
        /// <param name="request">The get upload part url request to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<GetUploadUrlResponse>> GetUploadUrlAsync(GetUploadUrlRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Gets a url for uploading files from memory cache. 
        /// </summary>
        /// <param name="request">The get upload part url request to send.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now in seconds.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<GetUploadUrlResponse>> GetUploadUrlAsync(GetUploadUrlRequest request, int cacheTTL, CancellationToken cancellationToken);

        /// <summary>
        /// Hides a file so that <see cref="DownloadFileByNameAsync"/> will not find the file but previous versions of the file are still stored.   
        /// </summary>
        /// <param name="request">The hide file request to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<HideFileResponse>> HideFileAsync(HideFileRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// List buckets associated with an account in alphabetical order by bucket name. When using an authorization token
        /// that is restricted to a bucket you must include the <see cref="ListBucketsRequest.BucketId"/>
        /// or <see cref="ListBucketsRequest.BucketName"/> of that bucket in the request or the request will be denied. 
        /// </summary>
        /// <param name="request">The list buckets request to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<ListBucketsResponse>> ListBucketsAsync(ListBucketsRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// List buckets from memory cache associated with an account in alphabetical order by bucket name. When using an authorization token
        /// that is restricted to a bucket you must include the <see cref="ListBucketsRequest.BucketId"/>
        /// or <see cref="ListBucketsRequest.BucketName"/> of that bucket in the request or the request will be denied. 
        /// </summary>
        /// <param name="request">The list buckets request to send.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now in seconds.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<ListBucketsResponse>> ListBucketsAsync(ListBucketsRequest request, int cacheTTL, CancellationToken cancellationToken);

        /// <summary>
        /// List the names of all files in a bucket starting at a given name.
        /// </summary>
        /// <param name="request">The list file name request to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<ListFileNamesResponse>> ListFileNamesAsync(ListFileNamesRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// List the names of all files in a bucket starting at a given name from memory cache. 
        /// </summary>
        /// <param name="request">The list of file name request to send.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now in seconds.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<ListFileNamesResponse>> ListFileNamesAsync(ListFileNamesRequest request, int cacheTTL, CancellationToken cancellationToken);

        /// <summary>
        /// List all versions of the files contained in one bucket in alphabetical order by file name
        /// and by reverse of date/time uploaded for versions of files with the same name. 
        /// </summary>
        /// <param name="request">The list file versions request to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<ListFileVersionResponse>> ListFileVersionsAsync(ListFileVersionRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// List all versions of the files contained in one bucket in alphabetical order by file name
        /// and by reverse of date/time uploaded for versions of files with the same name from memory cache. 
        /// </summary>
        /// <param name="request">The list file versions request to send.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now in seconds.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<ListFileVersionResponse>> ListFileVersionsAsync(ListFileVersionRequest request, int cacheTTL, CancellationToken cancellationToken);

        /// <summary>
        /// List application keys associated with an account. 
        /// </summary>
        /// <param name="request">The list application keys request to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<ListKeysResponse>> ListKeysAsync(ListKeysRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// List application keys associated with an account from memory cache. 
        /// </summary>
        /// <param name="request">The list application keys request to send.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now in seconds.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<ListKeysResponse>> ListKeysAsync(ListKeysRequest request, int cacheTTL, CancellationToken cancellationToken);

        /// <summary>
        /// List parts that have been uploaded for a large file that has not been finished yet. 
        /// </summary>
        /// <param name="request">The list parts request to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<ListPartsResponse>> ListPartsAsync(ListPartsRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// List parts that have been uploaded for a large file that has not been finished yet from memory cache. 
        /// </summary>
        /// <param name="request">The list parts request to send.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now in seconds.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<ListPartsResponse>> ListPartsAsync(ListPartsRequest request, int cacheTTL, CancellationToken cancellationToken);

        /// <summary>
        /// List information about large file uploads that have been started but have not been finished or canceled. 
        /// </summary>
        /// <param name="request">The list unfinished large files request to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<ListUnfinishedLargeFilesResponse>> ListUnfinishedLargeFilesAsync(ListUnfinishedLargeFilesRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// List information about large file uploads that have been started but have not been finished or canceled from memory cache. 
        /// </summary>
        /// <param name="request">The list unfinished large files request to send.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now in seconds.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<ListUnfinishedLargeFilesResponse>> ListUnfinishedLargeFilesAsync(ListUnfinishedLargeFilesRequest request, int cacheTTL, CancellationToken cancellationToken);

        /// <summary>
        /// Prepares for uploading parts of a large file. 
        /// </summary>
        /// <param name="request">The start large file request to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<StartLargeFileResponse>> StartLargeFileAsync(StartLargeFileRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Update an existing bucket belonging to the specific account. 
        /// </summary>
        /// <param name="request">The update bucket request to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<UpdateBucketResponse>> UpdateBucketAsync(UpdateBucketRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Upload content stream to the Backblaze B2 Cloud Storage service. 
        /// </summary>
        /// <param name="request">The upload file request to send.</param>
        /// <param name="content"> The content stream of the content payload.</param>
        /// <param name="progress">A progress action which fires every time the write buffer is cycled.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="CapExceededExecption">Thrown when a cap is exceeded or an account in bad standing.</exception>
        /// <exception cref="InvalidHashException">Thrown when a validation hash is not valid.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<UploadFileResponse>> UploadFileAsync(UploadFileRequest request, Stream content, IProgress<ICopyProgress> progress, CancellationToken cancellationToken);

        /// <summary>
        /// Uploads one part of a multi-part content stream using file id obtained from <see cref="StartLargeFileAsync"/>. 
        /// </summary>
        /// <param name="request">The upload file request to send.</param>
        /// <param name="content"> The content stream of the content payload.</param>
        /// <param name="progress">A progress action which fires every time the write buffer is cycled.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="CapExceededExecption">Thrown when a cap is exceeded or an account in bad standing.</exception>
        /// <exception cref="InvalidHashException">Thrown when a validation hash is not valid.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<UploadPartResponse>> UploadPartAsync(UploadPartRequest request, Stream content, IProgress<ICopyProgress> progress, CancellationToken cancellationToken);

        #endregion
    }
}