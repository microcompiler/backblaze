using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Bytewizer.Backblaze.Extensions;

using Polly;


namespace Bytewizer.Backblaze.Handlers
{
    /// <summary>
    /// Handles transient faults that occur during requests according to the provided policy.
    /// </summary>
    public sealed class TransientFaultHandler : DelegatingHandler
    {
        /// <summary>
        /// Initializes an instance of the <see cref="TransientFaultHandler"/> class.
        /// </summary>
        /// <param name="policy">The transient fault handling policy used for sending requests.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="policy"/> is null.
        /// </exception>
        public TransientFaultHandler(IAsyncPolicy<HttpResponseMessage> policy)
        {
            this.policy = policy ?? throw new ArgumentNullException(nameof(policy));
        }

        private readonly IAsyncPolicy<HttpResponseMessage> policy;

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

            return policy.ExecuteAsync(async cancellation =>
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
