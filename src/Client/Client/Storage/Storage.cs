using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Authentication;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;

using Bytewizer.Backblaze.Models;

namespace Bytewizer.Backblaze.Client
{
    /// <summary>
    /// Represents a base implementation of the <see cref="Storage"/> which uses <see cref="ApiClient"/> for making HTTP requests.
    /// </summary>
    public abstract partial class Storage : DisposableObject, IStorageClient
    {
        #region Lifetime

        /// <summary>
        /// Initializes a new instance of the <see cref="Storage"/> class.
        /// </summary>
        public Storage(HttpClient client, IClientOptions options, ILoggerFactory logger, IMemoryCache cache)
        {
            try
            {
                // Initialize components
                _client = new ApiClient(client, options, logger.CreateLogger<ApiClient>(), cache);
                _logger = logger.CreateLogger<Storage>();
            }
            catch (Exception ex)
            {
                //Log exception error
                _logger.LogError(ex, ex.Message);

                //Continue error
                throw;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Storage"/> class.
        /// </summary>
        public Storage(IApiClient client, ILogger logger)
        {
            try
            {
                // Initialize components
                _client = client ?? throw new ArgumentNullException(nameof(client));
                _logger = logger ?? NullLogger.Instance; 

                // Connect to the Backblaze B2 API server
                _client.Connect();
            }
            catch (Exception ex)
            {
                //Log exception error
                _logger.LogError(ex, ex.Message);

                //Continue error
                throw;
            }
        }

        #region IDisposable

        /// <summary>
        /// Frees resources owned by this instance.
        /// </summary>
        /// <param name="disposing">
        /// True when called via <see cref="IDisposable.Dispose()"/>, false when called from the finalizer.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            // Only managed resources to dispose
            if (!disposing)
                return;

            // Dispose owned objects
            _client?.Dispose();

        }

        #endregion IDisposable

        #endregion

        #region Private Fields

        /// <summary>
        /// The <see cref="ILogger"/> used for application logging.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// The <see cref="IApiClient"/> connected client to Backblaze B2 Cloud Storage.
        /// </summary>
        private readonly IApiClient _client;

        #endregion

        #region Public Properties

        /// <summary>
        /// The identifier for the account.
        /// </summary>
        public string AccountId => _client?.AccountInfo.AccountId;

        /// <summary>
        /// The cancellation token associated with this <see cref="Storage"/> instance.
        /// </summary>
        public CancellationToken CancellationToken
        {
            get { return cancellationToken; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(CancellationToken));
                cancellationToken = value;
            }
        }
        private CancellationToken cancellationToken = CancellationToken.None;

        #endregion

        #region Public Methods

        /// <summary>
        /// Connect to Backblaze B2 Cloud Storage and initialize <see cref="AccountInfo"/>.
        /// </summary>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        public void Connect() => _client.Connect();

        /// <summary>
        /// Connect to Backblaze B2 Cloud Storage and initialize <see cref="AccountInfo"/>.
        /// </summary>
        /// <param name="keyId">The identifier for the key.</param>
        /// <param name="applicationKey">The secret part of the key. You can use either the master application key or a normal application key.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        public void Connect(string keyId, string applicationKey) => _client.Connect(keyId, applicationKey);

        /// <summary>
        /// Connect to Backblaze B2 Cloud Storage and initialize <see cref="AccountInfo"/>.
        /// </summary>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        public Task ConnectAsync() => _client.ConnectAsync();

        /// <summary>
        /// Connect to Backblaze B2 Cloud Storage and initialize <see cref="AccountInfo"/>.
        /// </summary>
        /// <param name="keyId">The identifier for the key.</param>
        /// <param name="applicationKey">The secret part of the key. You can use either the master application key or a normal application key.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        public Task ConnectAsync(string keyId, string applicationKey) => _client.ConnectAsync(keyId, applicationKey);

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
        public async Task<IApiResults<UploadFileResponse>> UploadAsync
            (string bucketId, string fileName, Stream content)
        {
            var request = new UploadFileByBucketIdRequest(bucketId, fileName);
            return await UploadAsync(request, content, null, cancellationToken);
        }

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
        public async Task<IApiResults<UploadFileResponse>> UploadAsync
            (UploadFileByBucketIdRequest request, Stream content, IProgress<ICopyProgress> progress, CancellationToken cancel)
        {
            return await _client.UploadAsync(request, content, progress, cancel);   
        }

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
        public async Task<IApiResults<DownloadFileResponse>> DownloadAsync
            (string bucketName, string fileName, Stream content)
        {
            var request = new DownloadFileByNameRequest(bucketName, fileName);
            return await DownloadAsync(request, content, null, cancellationToken);
        }

        /// <summary>
        /// Download a specific version of content by bucket and file name from Backblaze B2 Cloud Storage. 
        /// </summary>
        /// <param name="request">The <see cref="DownloadFileByIdRequest"/> to send.</param>
        /// <param name="content">The download content to receive.</param>
        /// <param name="progress">A progress action which fires every time the write buffer is cycled.</param>
        /// <param name="cancel">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="CapExceededExecption">Thrown when a cap is exceeded or an account in bad standing.</exception>
        /// <exception cref="InvalidHashException">Thrown when a checksum hash is not valid.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        public async Task<IApiResults<DownloadFileResponse>> DownloadAsync
            (DownloadFileByNameRequest request, Stream content, IProgress<ICopyProgress> progress, CancellationToken cancel)
        {
            return await _client.DownloadAsync(request, content, progress, cancel);
        }

        /// <summary>
        /// Download a specific version of content by file id from Backblaze B2 Cloud Storage. 
        /// </summary>
        /// <param name="fileId">The unique id of the file to download.</param>
        /// <param name="content">The download content to receive.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="CapExceededExecption">Thrown when a cap is exceeded or an account in bad standing.</exception>
        /// <exception cref="InvalidHashException">Thrown when a checksum hash is not valid.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        public async Task<IApiResults<DownloadFileResponse>> DownloadByIdAsync
            (string fileId, Stream content)
        {
            var request = new DownloadFileByIdRequest(fileId);
            return await DownloadByIdAsync(request, content, null, cancellationToken);
        }

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
        public async Task<IApiResults<DownloadFileResponse>> DownloadByIdAsync
            (DownloadFileByIdRequest request, Stream content, IProgress<ICopyProgress> progress, CancellationToken cancel)
        {
                return await _client.DownloadByIdAsync(request, content, progress, cancel);
        }

        #endregion

        #endregion
    }
}
