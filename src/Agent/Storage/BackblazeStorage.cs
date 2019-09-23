using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Authentication;

using Microsoft.Extensions.Logging;

using Bytewizer.Backblaze.Agent;
using Bytewizer.Backblaze.Client;
using Bytewizer.Backblaze.Models;

namespace Bytewizer.Backblaze.Storage
{
    /// <summary>
    /// Represents a default implementation of the <see cref="BackblazeStorage"/> which uses <see cref="ApiClient"/> for making HTTP requests.
    /// </summary>
    public partial class BackblazeStorage : DisposableObject, IBackblazeStorage
    {
        #region Lifetime

        /// <summary>
        /// Initializes a new instance of the <see cref="BackblazeStorage"/> class.
        /// </summary>
        public BackblazeStorage(IAgentOptions options, IApiClient client, ILogger<BackblazeStorage> logger)
        {
            try
            {
                // Initialize components
                _options = options ?? throw new ArgumentNullException(nameof(options));
                _client = client ?? throw new ArgumentNullException(nameof(client));
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));

                // Sets client options 
                _client.Options = _options as ClientOptions;

                // Connect to the Backblaze B2 API server
                _client.Connect(_options.KeyId, _options.ApplicationKey);
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
        /// The <see cref="IAgentOptions"/> used for application options.
        /// </summary>
        private readonly IAgentOptions _options;

        /// <summary>
        /// The <see cref="ILogger"/> used for application logging.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// The <see cref="IApiClient"/> connected client to Backblaze B2 Cloud Storage.
        /// </summary>
        public readonly IApiClient _client;

        #endregion

        #region Public Properties

        /// <summary>
        /// The identifier for the account.
        /// </summary>
        public string AccountId => _client?.AccountInfo.AccountId;

        /// <summary>
        /// The cancellation token associated with this <see cref="BackblazeStorage"/> instance.
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

        #region UploadAsync

        /// <summary>
        /// Upload content stream to Backblaze B2 Cloud Storage. 
        /// </summary>
        /// <param name="bucketId">The bucket id you want to upload to.</param>
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
