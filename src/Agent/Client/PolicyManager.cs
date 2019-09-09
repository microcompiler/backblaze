using System;
using System.Threading.Tasks;
using System.Security.Authentication;

using Microsoft.Extensions.Logging;

using Polly;

namespace Bytewizer.Backblaze.Client
{
    public class PolicyManager : IPolicyManager
    {
        /// <summary>
        /// The <see cref="ILogger"/> used for application logging.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Client configuration options.
        /// </summary>
        private readonly ClientOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="PolicyManager"/> class.
        /// </summary>
        /// <param name="logger">Logger for application caching.</param>
        public PolicyManager(ILogger logger, ClientOptions options)
        {
            _logger = logger;
            _options = options;

            // Initialize policies
            InvokePolicy = CreateInvokePolicy();
            DownloadPolicy = CreateDownloadPolicy();
            UploadPolicy = CreateUploadPolicy();
        }

        #region Public Properties

        public Func<Task> ConnectAsync { get; set; }

        /// <summary>
        /// Retry policy used for downloading.
        /// </summary>
        public IAsyncPolicy DownloadPolicy { get; private set; }

        /// <summary>
        /// Retry policy used for uploading.
        /// </summary>
        public IAsyncPolicy UploadPolicy { get; private set; }

        /// <summary>
        /// Retry policy used for invoking post requests.
        /// </summary>
        public IAsyncPolicy InvokePolicy { get; private set; }

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

        #region Private Methods

        /// <summary>
        /// Create a retry policy for upload execptions.
        /// </summary>
        private IAsyncPolicy CreateUploadPolicy()
        {
            var auth = CreateAuthenticationPolicy();
            var hash = CreateInvalidHashPolicy();
            var bulk = Policy.BulkheadAsync(_options.UploadMaxParallel, int.MaxValue);

            return Policy.WrapAsync(auth, hash, bulk);
        }

        /// <summary>
        /// Create a retry policy for download execptions.
        /// </summary>
        private IAsyncPolicy CreateDownloadPolicy()
        {

            var auth = CreateAuthenticationPolicy();
            var hash = CreateInvalidHashPolicy();
            var bulk = Policy.BulkheadAsync(_options.DownloadMaxParallel, int.MaxValue);

            return Policy.WrapAsync(auth, hash, bulk);
        }

        /// <summary>
        /// Create a retry policy for Invoke execptions.
        /// </summary>
        private IAsyncPolicy CreateInvokePolicy()
        {
            var auth = CreateAuthenticationPolicy();
            var hash = CreateInvalidHashPolicy();
            var bulk = Policy.BulkheadAsync(_options.RequestMaxParallel, int.MaxValue);
            return Policy.WrapAsync(auth, hash, bulk);
        }

        /// <summary>
        /// Create a retry policy for authentication execptions.
        /// </summary>
        private IAsyncPolicy CreateAuthenticationPolicy()
        {
            var auth = Policy
                .Handle<AuthenticationException>()
                .WaitAndRetryAsync(_options.RetryCount,
                    retryAttempt => GetSleepDuration(retryAttempt),
                    onRetry: async (exception, timeSpan, count, context) =>
                    {
                        _logger.LogWarning($"{exception.Message} Retry attempt {count} waiting {timeSpan.TotalSeconds} seconds before next retry.");
                        await ConnectAsync.Invoke();
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
               .WaitAndRetryAsync(_options.RetryCount,
                   retryAttempt => GetSleepDuration(retryAttempt),
                   onRetry: (exception, timeSpan, count, context) =>
                   {
                       _logger.LogWarning($"{exception.Message} Retry attempt {count} waiting {timeSpan.TotalSeconds} seconds before next retry.");
                   });

            return hash;
        }

        #endregion
    }
}
