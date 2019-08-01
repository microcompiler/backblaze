using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Bytewizer.Backblaze.Models;

namespace Bytewizer.Backblaze.Extensions
{
    /// <summary>
    /// Extensions methods for <see cref="HttpClient"/> which give progress reporting support.
    /// </summary>
    public static class HttpClientExtensions
    {
        /// <summary>
        /// Perform an HTTP Send with progress reporting capabilities.
        /// </summary>
        /// <param name="client">Extension variable.</param>
        /// <param name="request">The HTTP request mesage to send.</param>
        /// <param name="destination">The output stream to write the data response to.</param>
        /// <param name="progressReport">A progress action which fires every time the write buffer is cycled.</param>
        /// <param name="cancel">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The full HTTP response. Reading from the response stream is discouraged.</returns>
        public static async Task<HttpResponseMessage> SendAsync(this HttpClient client, HttpRequestMessage request, Stream destination, IProgress<ICopyProgress> progressReport = null, CancellationToken cancel = default)
        {
            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancel);
            long contentLength = response.Content.Headers.ContentLength ?? 0;

            using (var download = await response.Content.ReadAsStreamAsync())
            {
                if (progressReport == null)
                {
                    await download.CopyToAsync(destination);
                }
                else
                {
                    await download.CopyToAsync(
                        destination,
                        16384,
                        expectedTotalBytes: contentLength,
                        progressReport: progressReport,
                        cancel: cancel);
                }
                if (destination.CanSeek) { destination.Position = 0; }
            }
            return response;
        }
    }
}
