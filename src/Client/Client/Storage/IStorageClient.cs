using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Authentication;

using Bytewizer.Backblaze.Models;

namespace Bytewizer.Backblaze.Client
{
    /// <summary>
    /// An interface for <see cref="Storage"/>.
    /// </summary>
    public interface IStorageClient : IDisposable
    {
        #region Lifetime

        /// <summary>
        /// Provides methods to access file operations.
        /// </summary>
        IStorageFiles Files { get; }

        /// <summary>
        /// Provides methods to access directory operations.
        /// </summary>
        IStorageDirectories Directories { get; }

        /// <summary>
        /// Provides methods to access bucket operations.
        /// </summary>
        IStorageBuckets Buckets { get; }

        /// <summary>
        /// Provides methods to access key operations.
        /// </summary>
        IStorageKeys Keys { get; }

        /// <summary>
        /// Provides methods to access large file part operations.
        /// </summary>
        IStorageParts Parts { get; }

        #endregion

        /// <summary>
        /// The identifier for the account.
        /// </summary>
        string AccountId { get; }

        /// <summary>
        /// The cancellation token associated with this <see cref="ApiClient"/> instance.
        /// </summary>
        CancellationToken CancellationToken { get; set; }

        /// <summary>
        /// Connect to Backblaze B2 Cloud Storage and initialize <see cref="AccountInfo"/>.
        /// </summary>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        AuthorizeAccountResponse Connect();

        /// <summary>
        /// Connect to Backblaze B2 Cloud Storage and initialize <see cref="AccountInfo"/>.
        /// </summary>
        /// <param name="keyId">The identifier for the key.</param>
        /// <param name="applicationKey">The secret part of the key. You can use either the master application key or a normal application key.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        AuthorizeAccountResponse Connect(string keyId, string applicationKey);

        /// <summary>
        /// Connect to Backblaze B2 Cloud Storage and initialize <see cref="AccountInfo"/>.
        /// </summary>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<AuthorizeAccountResponse> ConnectAsync();

        /// <summary>
        /// Connect to Backblaze B2 Cloud Storage and initialize <see cref="AccountInfo"/>.
        /// </summary>
        /// <param name="keyId">The identifier for the key.</param>
        /// <param name="applicationKey">The secret part of the key. You can use either the master application key or a normal application key.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<AuthorizeAccountResponse> ConnectAsync(string keyId, string applicationKey);

        #region UploadAsync

        /// <summary>
        /// Upload content stream to Backblaze B2 Cloud Storage. 
        /// </summary>
        /// <param name="bucketId">The bucket id you want to upload to.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="content"> The content stream of the content payload.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="CapExceededExecption">Thrown when a cap is exceeded or an account in bad standing.</exception>
        /// <exception cref="InvalidHashException">Thrown when a checksum hash is not valid.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<UploadFileResponse>> UploadAsync(string bucketId, string fileName, Stream content);

        /// <summary>
        /// Upload content stream to Backblaze B2 Cloud Storage. 
        /// </summary>
        /// <param name="request">The <see cref="UploadFileRequest"/> to send.</param>
        /// <param name="content"> The content stream of the content payload.</param>
        /// <param name="progress">A progress action which fires every time the write buffer is cycled.</param>
        /// <param name="cancel">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="CapExceededExecption">Thrown when a cap is exceeded or an account in bad standing.</exception>
        /// <exception cref="InvalidHashException">Thrown when a checksum hash is not valid.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<UploadFileResponse>> UploadAsync(UploadFileByBucketIdRequest request, Stream content, IProgress<ICopyProgress> progress, CancellationToken cancel);

        #endregion

        #region DownloadAsync

        /// <summary>
        /// Download a specific version of content by bucket and file name from Backblaze B2 Cloud Storage. 
        /// </summary>
        /// <param name="bucketName">The unique name of the bucket the file is in.</param>
        /// <param name="fileName">The name of the file to download.</param>
        /// <param name="content">The download content to receive.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="CapExceededExecption">Thrown when a cap is exceeded or an account in bad standing.</exception>
        /// <exception cref="InvalidHashException">Thrown when a checksum hash is not valid.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<DownloadFileResponse>> DownloadAsync(string bucketName, string fileName, Stream content);

        /// <summary>
        /// Download a specific version of content by bucket and file name from Backblaze B2 Cloud Storage. 
        /// </summary>
        /// <param name="request">The <see cref="DownloadFileByNameRequest"/> to send.</param>
        /// <param name="content">The download content to receive.</param>
        /// <param name="progress">A progress action which fires every time the write buffer is cycled.</param>
        /// <param name="cancel">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="CapExceededExecption">Thrown when a cap is exceeded or an account in bad standing.</exception>
        /// <exception cref="InvalidHashException">Thrown when a checksum hash is not valid.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<DownloadFileResponse>> DownloadAsync(DownloadFileByNameRequest request, Stream content, IProgress<ICopyProgress> progress, CancellationToken cancel);

        /// <summary>
        /// Download a specific version of content by file id from Backblaze B2 Cloud Storage. 
        /// </summary>
        /// <param name="fileId">The unique id of the file to download.</param>
        /// <param name="content">The download content to receive.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="CapExceededExecption">Thrown when a cap is exceeded or an account in bad standing.</exception>
        /// <exception cref="InvalidHashException">Thrown when a checksum hash is not valid.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<DownloadFileResponse>> DownloadByIdAsync(string fileId, Stream content);

        /// <summary>
        /// Download a specific version of content by file id from Backblaze B2 Cloud Storage. 
        /// </summary>
        /// <param name="request">The <see cref="DownloadFileByIdRequest"/> to send.</param>
        /// <param name="content">The download content to receive.</param>
        /// <param name="progress">A progress action which fires every time the write buffer is cycled.</param>
        /// <param name="cancel">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="CapExceededExecption">Thrown when a cap is exceeded or an account in bad standing.</exception>
        /// <exception cref="InvalidHashException">Thrown when a checksum hash is not valid.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<DownloadFileResponse>> DownloadByIdAsync(DownloadFileByIdRequest request, Stream content, IProgress<ICopyProgress> progress, CancellationToken cancel);

        #endregion

    }
}