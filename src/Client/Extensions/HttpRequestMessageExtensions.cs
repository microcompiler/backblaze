using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bytewizer.Backblaze.Extensions
{
    /// <summary>
    /// Contains extension methods for <see cref="HttpRequestMessage"/>.
    /// </summary>
    internal static class HttpRequestMessageExtensions
    {
        /// <summary>
        /// Clones a <see cref="HttpRequestMessage"/> instance.
        /// </summary>
        /// <param name="request">The <see cref="HttpRequestMessage"/> instance to clone.</param>
        /// <returns>
        /// A clone of <paramref name="request"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="request"/> is null.
        /// </exception>
        public static async Task<HttpRequestMessage> CloneAsync(this HttpRequestMessage request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var clone = new HttpRequestMessage();

            if (request.Content != null)
            {
                var content = new MemoryStream();
                await request.Content.CopyToAsync(content).ConfigureAwait(false);
                content.Position = 0;
                clone.Content = request.Content.Clone(content);
            }

            clone.Version = request.Version;
            clone.Method = request.Method;
            clone.RequestUri = request.RequestUri;

            foreach (var header in request.Headers)
            {
                clone.Headers.Add(header.Key, header.Value);
            }

            foreach (var prop in request.Properties)
            {
                clone.Properties.Add(prop);
            }

            return clone;
        }
    }
}
