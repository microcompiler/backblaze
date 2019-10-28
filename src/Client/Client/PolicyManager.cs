using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Security.Authentication;

using Microsoft.Extensions.Logging;

using Polly;

namespace Bytewizer.Backblaze.Client
{
    /// <summary>
    /// Policy objects used for application resilience.
    /// </summary>
    public class PolicyManager : IPolicyManager
    {
        private static readonly Random _jitterer = new Random();

        /// <summary>
        /// The <see cref="IClientOptions"/> used for application options.
        /// </summary>
        private readonly IClientOptions _options;

        /// <summary>
        /// The <see cref="ILogger"/> used for application logging.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PolicyManager"/> class.
        /// </summary>
        /// <param name="options">Options for application configuration.</param>
        /// <param name="logger">Logger for application caching.</param>
        public PolicyManager(IClientOptions options, ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options ?? throw new ArgumentNullException(nameof(options));
           
            // Initialize policies
            InvokePolicy = CreateInvokePolicy();
            InvokeDownload = CreateDownloadPolicy();
            InvokeUpload = CreateUploadPolicy();
            InvokeClient = Policy.NoOpAsync<HttpResponseMessage>(); //CreateRetryPolicy(); //Policy.NoOpAsync<HttpResponseMessage>()
        }

        #region Public Properties

        /// <summary>
        /// Connect to Backblaze B2 Cloud Storage and initialize <see cref="AccountInfo"/>.
        /// </summary>
        public Func<Task> ConnectAsync { get; set; }

        /// <summary>
        /// Retry policy used for invoking post requests.
        /// </summary>
        public IAsyncPolicy InvokePolicy { get; private set; }

        /// <summary>
        /// Retry policy used for downloading.
        /// </summary>
        public IAsyncPolicy InvokeDownload { get; private set; }

        /// <summary>
        /// Retry policy used for uploading.
        /// </summary>
        public IAsyncPolicy InvokeUpload { get; private set; }

        /// <summary>
        /// Retry policy used for invoking send requests.
        /// </summary>
        public IAsyncPolicy<HttpResponseMessage> InvokeClient { get; private set; }

        /// <summary>
        /// Get the duration to wait (exponential backoff) allowing an increasing wait time.
        /// </summary>
        /// <param name="retryAttempt">The retry attempt count.</param>
        public static TimeSpan GetSleepDuration(int retryAttempt)
        {
            int jitter;

            lock (_jitterer)
            {
                jitter = _jitterer.Next(10, 10000);
            }

            return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                    + TimeSpan.FromMilliseconds(jitter);
        }

        /// <summary>
        /// Create a retry policy for http response messages execptions.
        /// </summary>
        public static IAsyncPolicy<HttpResponseMessage> CreateRetryPolicy(int retryCount, ILogger logger)
        {
            HttpStatusCode[] httpStatusCodes =
                {
                    HttpStatusCode.RequestTimeout, // 408
                    HttpStatusCode.InternalServerError, // 500
                    HttpStatusCode.BadGateway, // 502
                    HttpStatusCode.ServiceUnavailable, // 503
                    HttpStatusCode.GatewayTimeout, // 504
                    (HttpStatusCode)429 // 429
                };

            return Policy
                .Handle<HttpRequestException>()
                .Or<IOException>()
                .OrResult<HttpResponseMessage>(r => httpStatusCodes.Contains(r.StatusCode))
                .WaitAndRetryAsync(retryCount,
                        retryAttempt => PolicyManager.GetSleepDuration(retryAttempt),
                        onRetry: (exception, timeSpan, count, context) =>
                        {
                            logger.LogWarning($"Status Code: {exception.Result?.StatusCode} Request Message: {exception.Result?.RequestMessage} Retry attempt {count} waiting {timeSpan.TotalSeconds} seconds before next retry.");
                            //_logger.LogWarning($"Retry {retryCount} implemented of {context.PolicyKey} at {context.OperationKey} due to: {exception}");
                        });
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
            return Policy
                .Handle<AuthenticationException>()
                .WaitAndRetryAsync(_options.RetryCount,
                    retryAttempt => GetSleepDuration(retryAttempt),
                    onRetry: async (exception, timeSpan, count, context) =>
                    {
                        _logger.LogWarning($"{exception.Message} Retry attempt {count} waiting {timeSpan.TotalSeconds} seconds before next retry.");
                        await ConnectAsync.Invoke();
                    });   
        }

        /// <summary>
        /// Create a retry policy for invalid hash execptions.
        /// </summary>
        private IAsyncPolicy CreateInvalidHashPolicy()
        {
            return Policy
               .Handle<InvalidHashException>()
               .WaitAndRetryAsync(_options.RetryCount,
                   retryAttempt => GetSleepDuration(retryAttempt),
                   onRetry: (exception, timeSpan, count, context) =>
                   {
                       _logger.LogWarning($"{exception.Message} Retry attempt {count} waiting {timeSpan.TotalSeconds} seconds before next retry.");
                   });
        }

        private IAsyncPolicy<HttpResponseMessage> CreateRetryPolicy()
        {
            HttpStatusCode[] httpStatusCodes = 
                {
                    HttpStatusCode.RequestTimeout, // 408
                    HttpStatusCode.InternalServerError, // 500
                    HttpStatusCode.BadGateway, // 502
                    HttpStatusCode.ServiceUnavailable, // 503
                    HttpStatusCode.GatewayTimeout, // 504
                    (HttpStatusCode)429 // 429
                };  

            return Policy
                .Handle<HttpRequestException>()
                .Or<IOException>()
                .OrResult<HttpResponseMessage>(r => httpStatusCodes.Contains(r.StatusCode))
                .WaitAndRetryAsync(_options.RetryCount,
                        retryAttempt => GetSleepDuration(retryAttempt),
                        onRetry: (exception, timeSpan, count, context) =>
                        {
                            _logger.LogWarning($"Status Code: {exception.Result?.StatusCode} Request Message: {exception.Result?.RequestMessage} Retry attempt {count} waiting {timeSpan.TotalSeconds} seconds before next retry.");
                            //_logger.LogWarning($"Retry {retryCount} implemented of {context.PolicyKey} at {context.OperationKey} due to: {exception}");
                        });
        }

        #endregion
    }
}
