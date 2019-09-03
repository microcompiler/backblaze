using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Polly;

using Bytewizer.Backblaze.Client;
using Bytewizer.Backblaze.Models;
using System.Security.Authentication;

namespace Bytewizer.Backblaze.Agent
{
    public partial class BackblazeAgent : DisposableObject, IBackblazeAgent
    {

        //TODO: Backblaze agent class comments.
        //TODO: Disable checksum option using "do_not_verify" sha1.

        #region Lifetime

        /// <summary>
        /// Creates an instance.
        /// </summary>
        public BackblazeAgent(IAgentOptions options, IApiClient client, ILogger<BackblazeAgent> logger)
        {
            try
            {
                // Initialize components
                _options = options ?? throw new ArgumentNullException(nameof(options));
                _client = client ?? throw new ArgumentNullException(nameof(client));
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));

                // Sets client options 
                _client.TestMode = _options.TestMode;
                _client.UploadCutoffSize = _options.UploadCutoffSize;
                _client.UploadPartSize = _options.UploadPartSize;
                _client.DownloadCutoffSize = _options.DownloadCutoffSize;
                _client.DownloadPartSize = _options.DownloadPartSize;
                _client.AccountInfo.AuthUrl = _options.AuthUrl;

                // Initialize policies
                InvokePolicy = CreateInvokePolicy();
                DownloadPolicy = CreateDownloadPolicy();
                UploadPolicy = CreateUploadPolicy();

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
        /// Application options
        /// </summary>
        private readonly IAgentOptions _options;

        /// <summary>
        /// Application logging
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Connected client to the Backblaze B2 API server.
        /// </summary>
        public readonly IApiClient _client;

        #endregion

        #region Public Properties

        /// <summary>
        /// The identifier for the account.
        /// </summary>
        public string AccountId => _client?.AccountInfo.AccountId;

        /// <summary>
        /// The recommended size for each part of a large file. We recommend using this
        /// part size for optimal upload performance.
        /// </summary>
        public long RecommendedPartSize => _client.AccountInfo.RecommendedPartSize;

        /// <summary>
        /// The smallest possible size of a part of a large file (except the last one). This is smaller 
        /// than the <see cref="RecommendedPartSize"/>. If you use it, you may find that it takes longer
        /// overall to upload a large file.
        /// </summary>
        public long AbsoluteMinimumPartSize => _client.AccountInfo.AbsoluteMinimumPartSize;

        /// <summary>
        /// The cancellation token associated with this <see cref="BackblazeAgent"/> instance.
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

        #region Private Properties

        /// <summary>
        /// Retry policy used for downloading.
        /// </summary>
        protected IAsyncPolicy DownloadPolicy { get; private set; }


        /// <summary>
        /// Retry policy used for uploading.
        /// </summary>
        protected IAsyncPolicy UploadPolicy { get; private set; }

        /// <summary>
        /// Retry policy used for invoking post requests.
        /// </summary>
        protected IAsyncPolicy InvokePolicy { get; private set; }

        #endregion

        #region Public Methods

        #region UploadAsync

        public async Task<IApiResults<UploadFileResponse>> UploadAsync
            (string bucketId, string fileName, Stream content)
        {
            var request = new UploadFileByBucketIdRequest(bucketId, fileName);
            return await UploadAsync(request, content, null, cancellationToken);
        }

        public async Task<IApiResults<UploadFileResponse>> UploadAsync
           (string bucketId, string fileName, Stream content, IProgress<ICopyProgress> progress)
        {
            var request = new UploadFileByBucketIdRequest(bucketId, fileName);
            return await UploadAsync(request, content, progress, cancellationToken);
        }

        public async Task<IApiResults<UploadFileResponse>> UploadAsync
            (string bucketId, string fileName, Stream content, CancellationToken cancel)
        {
            var request = new UploadFileByBucketIdRequest(bucketId, fileName);
            return await UploadAsync(request, content, null, cancel);
        }

        public async Task<IApiResults<UploadFileResponse>> UploadAsync
            (string bucketId, string fileName, Stream content, IProgress<ICopyProgress> progress, CancellationToken cancel)
        {
            var request = new UploadFileByBucketIdRequest(bucketId, fileName);
            return await UploadAsync(request, content, progress, cancel);
        }

        public async Task<IApiResults<UploadFileResponse>> UploadAsync
            (UploadFileByBucketIdRequest request, Stream content)
        {
            return await UploadAsync(request, content, null, cancellationToken);
        }

        public async Task<IApiResults<UploadFileResponse>> UploadAsync
            (UploadFileByBucketIdRequest request, Stream content, IProgress<ICopyProgress> progress)
        {
            return await UploadAsync(request, content, progress, cancellationToken);
        }

        public async Task<IApiResults<UploadFileResponse>> UploadAsync
            (UploadFileByBucketIdRequest request, Stream content, CancellationToken cancel)
        {
            return await UploadAsync(request, content, null, cancel);
        }

        public async Task<IApiResults<UploadFileResponse>> UploadAsync
            (UploadFileByBucketIdRequest request, Stream content, IProgress<ICopyProgress> progress, CancellationToken cancel)
        {
            return await UploadPolicy.ExecuteAsync(async () =>
            {
                return await _client.UploadAsync(request, content, progress, cancel);
            });
        }

        #endregion

        #region DownloadAsync

        // Download by file id
        public async Task<IApiResults<DownloadFileResponse>> DownloadByIdAsync
            (string fileId, Stream content)
        {
            var request = new DownloadFileByIdRequest(fileId);
            return await DownloadByIdAsync(request, content, null, cancellationToken);
        }

        public async Task<IApiResults<DownloadFileResponse>> DownloadByIdAsync
            (string fileId, Stream content, IProgress<ICopyProgress> progress)
        {
            var request = new DownloadFileByIdRequest(fileId);
            return await DownloadByIdAsync(request, content, progress, cancellationToken);
        }

        public async Task<IApiResults<DownloadFileResponse>> DownloadByIdAsync
            (string fileId, Stream content, CancellationToken cancel)
        {
            var request = new DownloadFileByIdRequest(fileId);
            return await DownloadByIdAsync(request, content, null, cancel);
        }

        public async Task<IApiResults<DownloadFileResponse>> DownloadByIdAsync
            (string fileId, Stream content, IProgress<ICopyProgress> progress, CancellationToken cancel)
        {
            var request = new DownloadFileByIdRequest(fileId);
            return await DownloadByIdAsync(request, content, progress, cancel);
        }

        public async Task<IApiResults<DownloadFileResponse>> DownloadByIdAsync
            (DownloadFileByIdRequest request, Stream content)
        {
            return await DownloadByIdAsync(request, content, null, cancellationToken);
        }

        public async Task<IApiResults<DownloadFileResponse>> DownloadByIdAsync
            (DownloadFileByIdRequest request, Stream content, IProgress<ICopyProgress> progress)
        {
            return await DownloadByIdAsync(request, content, progress, cancellationToken);
        }

        public async Task<IApiResults<DownloadFileResponse>> DownloadByIdAsync
            (DownloadFileByIdRequest request, Stream content, CancellationToken cancel)
        {
            return await DownloadByIdAsync(request, content, null, cancel);
        }

        public async Task<IApiResults<DownloadFileResponse>> DownloadByIdAsync
            (DownloadFileByIdRequest request, Stream content, IProgress<ICopyProgress> progress, CancellationToken cancel)
        {
            return await DownloadPolicy.ExecuteAsync(async () =>
            {
                return await _client.DownloadByIdAsync(request, content, progress, cancel);
            });
        }

        // Download by bucket name and file name
        public async Task<IApiResults<DownloadFileResponse>> DownloadAsync
            (string bucketName, string fileName, Stream content)
        {
            var request = new DownloadFileByNameRequest(bucketName, fileName);
            return await DownloadAsync(request, content, null, cancellationToken);
        }

        public async Task<IApiResults<DownloadFileResponse>> DownloadAsync
            (string bucketName, string fileName, Stream content, IProgress<ICopyProgress> progress)
        {
            var request = new DownloadFileByNameRequest(bucketName, fileName);
            return await DownloadAsync(request, content, progress, cancellationToken);
        }

        public async Task<IApiResults<DownloadFileResponse>> DownloadAsync
            (string bucketName, string fileName, Stream content, CancellationToken cancel)
        {
            var request = new DownloadFileByNameRequest(bucketName, fileName);
            return await DownloadAsync(request, content, null, cancel);
        }

        public async Task<IApiResults<DownloadFileResponse>> DownloadAsync
            (string bucketName, string fileName, Stream content, IProgress<ICopyProgress> progress, CancellationToken cancel)
        {
            var request = new DownloadFileByNameRequest(bucketName, fileName);
            return await DownloadAsync(request, content, progress, cancel);
        }

        public async Task<IApiResults<DownloadFileResponse>> DownloadAsync
            (DownloadFileByNameRequest request, Stream content)
        {
            return await DownloadAsync(request, content, null, cancellationToken);
        }

        public async Task<IApiResults<DownloadFileResponse>> DownloadAsync
            (DownloadFileByNameRequest request, Stream content, IProgress<ICopyProgress> progress)
        {
            return await DownloadAsync(request, content, progress, cancellationToken);
        }

        public async Task<IApiResults<DownloadFileResponse>> DownloadAsync
            (DownloadFileByNameRequest request, Stream content, CancellationToken cancel)
        {
            return await DownloadAsync(request, content, null, cancel);
        }

        public async Task<IApiResults<DownloadFileResponse>> DownloadAsync
            (DownloadFileByNameRequest request, Stream content, IProgress<ICopyProgress> progress, CancellationToken cancel)
        {
            return await DownloadPolicy.ExecuteAsync(async () =>
            {
                return await _client.DownloadAsync(request, content, progress, cancel);
            });
        }

        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        /// Create a retry policy for upload execptions.
        /// </summary>
        private IAsyncPolicy CreateUploadPolicy()
        {
            var auth = CreateAuthenticationPolicy();
            var hash = CreateInvalidHashPolicy();
            var bulk = Policy.BulkheadAsync(_options.UploadConnections, int.MaxValue);

            return Policy.WrapAsync(auth, hash, bulk);
        }

        /// <summary>
        /// Create a retry policy for download execptions.
        /// </summary>
        private IAsyncPolicy CreateDownloadPolicy()
        {

            var auth = CreateAuthenticationPolicy();
            var hash = CreateInvalidHashPolicy();
            var bulk = Policy.BulkheadAsync(_options.DownloadConnections, int.MaxValue);

            return Policy.WrapAsync(auth, hash, bulk);
        }

        /// <summary>
        /// Create a retry policy for Invoke execptions.
        /// </summary>
        private IAsyncPolicy CreateInvokePolicy()
        {
            var auth = CreateAuthenticationPolicy();
            var hash = CreateInvalidHashPolicy();
            var bulk = Policy.BulkheadAsync(_options.DownloadConnections, int.MaxValue);
            return Policy.WrapAsync(auth, hash, bulk);
        }

        /// <summary>
        /// Create a retry policy for authentication execptions.
        /// </summary>
        private IAsyncPolicy CreateAuthenticationPolicy()
        {
            var auth = Policy
                .Handle<AuthenticationException>()
                .WaitAndRetryAsync(_options.AgentRetryCount,
                    retryAttempt => GetSleepDuration(retryAttempt),
                    onRetry: async (exception, timeSpan, count, context) =>
                    {
                        _logger.LogWarning($"{exception.Message} Retry attempt {count} waiting {timeSpan.TotalSeconds} seconds before next retry.");
                        await _client.ConnectAsync(_options.KeyId, _options.ApplicationKey);
                    });

            return auth;
        }

        /// <summary>
        /// Create a retry policy for invalid hash execptions.
        /// </summary>
        private IAsyncPolicy CreateInvalidHashPolicy()
        {
            var hash = Policy
               .Handle<InvalidHashException>()
               .WaitAndRetryAsync(_options.AgentRetryCount,
                   retryAttempt => GetSleepDuration(retryAttempt),
                   onRetry: (exception, timeSpan, count, context) =>
                   {
                       _logger.LogWarning($"{exception.Message} Retry attempt {count} waiting {timeSpan.TotalSeconds} seconds before next retry.");
                   });

            return hash;
        }

        /// <summary>
        /// Get the duration to wait (exponential backoff) allowing an increasing wait time.
        /// </summary>
        /// <param name="retryAttempt">The retry attempt count.</param>
        public static TimeSpan GetSleepDuration(int retryAttempt)
        {
            Random jitterer = new Random();

            return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                    + TimeSpan.FromMilliseconds(jitterer.Next(10, 1000));
        }

        #endregion
    }
}
