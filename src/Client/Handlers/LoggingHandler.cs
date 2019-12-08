using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace Bytewizer.Backblaze.Handlers
{
    /// <summary>
    /// The logger handler.
    /// </summary>
    public class LoggingHandler : DelegatingHandler
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingHandler" /> class.
        /// </summary>
        public LoggingHandler(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Add the user agent to the outgoing request.
        /// </summary>
        /// <param name="request">The HTTP request message.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            _logger.LogTrace($"Request: {request}");
            try
            {
                var response = await base.SendAsync(request, cancellationToken);
                _logger.LogTrace($"Response: {response}");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get response: {ex}");
                throw;
            }
        }
    }
}
