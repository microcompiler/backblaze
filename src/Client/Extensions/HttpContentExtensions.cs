using System.IO;
using System.Net.Http;

namespace Bytewizer.Backblaze.Extensions
{
    /// <summary>
    /// Contains extension methods for <see cref="HttpContent"/>.
    /// </summary>
    internal static class HttpContentExtensions
    {
        /// <summary>
        /// Makes a clone of a <see cref="HttpContent"/> instance.
        /// </summary>
        /// <param name="httpContent">The <see cref="HttpContent"/> to clone.</param>
        /// <param name="content">The content to use for the clone.</param>
        /// <returns>
        /// A clone of <paramref name="httpContent"/>, if it is not null; otherwise, null.
        /// </returns>
        public static HttpContent Clone(this HttpContent httpContent, Stream content)
        {
            if (httpContent == null) { return null; }

            var clone = new StreamContent(content);

            foreach (var header in httpContent.Headers)
            {
                clone.Headers.Add(header.Key, header.Value);
            }

            return clone;
        }
    }
}
