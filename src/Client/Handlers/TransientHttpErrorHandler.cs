using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Bytewizer.Backblaze.Client;
using Bytewizer.Backblaze.Extensions;

using Polly;

namespace Bytewizer.Backblaze.Handlers
{
    /// <summary>
    /// Handles transient faults that occur during requests according to the provided policy.
    /// </summary>
    public sealed class TransientHttpErrorHandler : DelegatingHandler
    {
        private readonly IAsyncPolicy<HttpResponseMessage> _policy;

        /// <summary>
        /// Initializes an instance of the <see cref="TransientHttpErrorHandler"/> class.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="logger">Logger for transient error handler.</param>
        public TransientHttpErrorHandler(IClientOptions options, ILogger<StorageService> logger)
        {
            _policy = PolicyManager.CreateRetryPolicy(options.RetryCount, logger);
        }

        /// <summary>
        /// Initializes an instance of the <see cref="TransientHttpErrorHandler"/> class.
        /// </summary>
        /// <param name="policy">The transient fault handling policy used for sending requests.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="policy"/> is null.
        /// </exception>
        public TransientHttpErrorHandler(IAsyncPolicy<HttpResponseMessage> policy)
        {
            _policy = policy ?? throw new ArgumentNullException(nameof(policy));
        }

        /// <summary>
        /// Sends an HTTP request to the inner handler to send to the server as an asynchronous operation.
        /// </summary>
        /// <param name="request">The HTTP request message to send to the server.</param>
        /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="request"/> is null.
        /// </exception>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var isInitialSend = true;

            return _policy.ExecuteAsync(async cancellation =>
            {
                // When retrying, the request must be cloned as the same request object cannot be sent more than once.
                if (!isInitialSend)
                {
                    using (var oldRequest = request)
                    {
                        request = await oldRequest.CloneAsync().ConfigureAwait(false);
                    }
                }

                return await base.SendAsync(request, cancellation).ConfigureAwait(false);

            }, cancellationToken, false);
        }
    }
}
