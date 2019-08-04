using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Polly;

using Bytewizer.Backblaze.Client;
using Bytewizer.Backblaze.Models;

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
                _client.RetryCount = _options.AgentRetryCount;

                // Connect to the Backblaze B2 API server
                _client.Connect(_options.KeyId, _options.ApplicationKey);
            }
            catch (Exception ex)
            {
                //Log exception error
                _logger.LogError(ex, "fatal error");

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
            return await _client.UploadAsync(request, content, progress, cancel);
        }

        #endregion

        #region DownloadAsync

        // Download by file id
        public async Task<IApiResults<DownloadFileResponse>> DownloadAsync
            (string fileId, Stream content)
        {
            var request = new DownloadFileByIdRequest(fileId);
            return await DownloadAsync(request, content, null, cancellationToken);
        }

        public async Task<IApiResults<DownloadFileResponse>> DownloadAsync
            (string fileId, Stream content, IProgress<ICopyProgress> progress)
        {
            var request = new DownloadFileByIdRequest(fileId);
            return await DownloadAsync(request, content, progress, cancellationToken);
        }

        public async Task<IApiResults<DownloadFileResponse>> DownloadAsync
            (string fileId, Stream content, CancellationToken cancel)
        {
            var request = new DownloadFileByIdRequest(fileId);
            return await DownloadAsync(request, content, null, cancel);
        }

        public async Task<IApiResults<DownloadFileResponse>> DownloadAsync
            (string fileId, Stream content, IProgress<ICopyProgress> progress, CancellationToken cancel)
        {
            var request = new DownloadFileByIdRequest(fileId);
            return await DownloadAsync(request, content, progress, cancel);
        }

        public async Task<IApiResults<DownloadFileResponse>> DownloadAsync
            (DownloadFileByIdRequest request, Stream content)
        {
            return await DownloadAsync(request, content, null, cancellationToken);
        }

        public async Task<IApiResults<DownloadFileResponse>> DownloadAsync
            (DownloadFileByIdRequest request, Stream content, IProgress<ICopyProgress> progress)
        {
            return await DownloadAsync(request, content, progress, cancellationToken);
        }

        public async Task<IApiResults<DownloadFileResponse>> DownloadAsync
            (DownloadFileByIdRequest request, Stream content, CancellationToken cancel)
        {
            return await DownloadAsync(request, content, null, cancel);
        }

        public async Task<IApiResults<DownloadFileResponse>> DownloadAsync
            (DownloadFileByIdRequest request, Stream content, IProgress<ICopyProgress> progress, CancellationToken cancel)
        {
            return await _client.DownloadAsync(request, content, progress, cancel);
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
            return await _client.DownloadAsync(request, content, progress, cancel);
        }

        #endregion

        #endregion

        #region Private Methods

        private IApiResults<T> HandleResults<T>(IApiResults<T> results)
            where T : IResponse
        {
            if (!results.IsSuccessStatusCode)
                _logger.LogError(results.Error?.Message);

            return results;
        }

        #endregion
    }
}






//private async Task<IApiResults<TResponse>> HandleResultsAsync(IApiResults<T> T)
//    where 
//{
//    if (response == null)
//        throw new ArgumentNullException(nameof(response));

//    if (!response.IsSuccessStatusCode)
//    {
//        var error = await ReadAsJsonAsync<ErrorResponse>(response);
//        return new ApiResults<TResponse>(response, error);
//    }
//    var results = await ReadAsJsonAsync<TResponse>(response);
//    return new ApiResults<TResponse>(response, results);
//}


//static async Task RetryAsync(Func<CancellationToken, Task> func, int retryCount, TimeSpan timeout)
//{
//    using (var cts = new CancellationTokenSource(timeout))
//    {
//        var policy = Policy.Handle<Exception>(ex => !(ex is OperationCanceledException))
//            .RetryAsync(retryCount);
//        await policy.ExecuteAsync(() => func(cts.Token)).ConfigureAwait(false);
//    }
//}

//private async Task<IApiClient> InvokeClientAsync (IApiClient client)
//{

//    var results = await Policy
//                    .Handle<AuthenticationException>(ex => ex.Error.Code == "bad_auth_token" || ex.Error.Code == "expired_auth_token")
//                    .WaitAndRetryAsync(3,
//                        retryAttempt => GetSleepDuration(retryAttempt),
//                        async (exception, retryCount) => await _client.ConnectAsync(_options.ApplicationKeyId, _options.ApplicationKey))
//                    .ExecuteAsync(async () => await _client.ListFileNamesAsync(request, cancellationToken));
//}
