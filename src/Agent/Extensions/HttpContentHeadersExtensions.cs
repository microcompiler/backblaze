using System.Net.Http.Headers;

using Bytewizer.Backblaze.Models;

namespace Bytewizer.Backblaze.Extensions
{
    /// <summary>
    /// Extensions methods for <see cref="HttpContentHeaders"/>.
    /// </summary>
    static public class HttpContentHeadersExtensions
    {
        /// <summary>
        /// Sets content SHA1 header.
        /// </summary>
        /// <param name="headers">The http content request header.</param>
        /// <param name="value">The header value.</param>
        public static void ContentSha1(this HttpContentHeaders headers, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
                headers.Add("x-bz-content-sha1", value);
        }

        /// <summary>
        /// Sets content file name header.
        /// </summary>
        /// <param name="headers">The http content request header.</param>
        /// <param name="value">The header value.</param>
        public static void SetContentFileName(this HttpContentHeaders headers, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
                headers.Add("x-bz-file-name", value.ToUrlEncode());
        }

        /// <summary>
        /// Sets content dispositon header.
        /// </summary>
        /// <param name="headers">The http content request header.</param>
        /// <param name="value">The header value.</param>
        public static void SetContentDisposition(this HttpContentHeaders headers, ContentDispositionHeaderValue value)
        {
            if (value != null)
                headers.Add("b2ContentDisposition", value.ToString());
        }

        /// <summary>
        /// Sets content disposition header.
        /// </summary>
        /// <param name="headers">The http content request header.</param>
        /// <param name="response">The download file request.</param>
        public static void SetContentDisposition(this HttpContentHeaders headers, DownloadFileByIdRequest request)
        {
            if (request.ContentDisposition != null)
                headers.ContentDisposition = request.ContentDisposition;
        }

        /// <summary>
        /// Sets content disposition header.
        /// </summary>
        /// <param name="headers">The http content request header.</param>
        /// <param name="response">The download file request.</param>
        public static void ContentDisposition(this HttpContentHeaders headers, DownloadFileByNameRequest request)
        {
            if (request.ContentDisposition != null)
                headers.ContentDisposition = request.ContentDisposition;
        }
    }
}