using System.Linq;
using System.Net.Http.Headers;
using System.Collections.Generic;

using Bytewizer.Backblaze.Models;

namespace Bytewizer.Backblaze.Extensions
{
    /// <summary>
    /// Contains extension methods for <see cref="HttpRequestHeaders"/>.
    /// </summary>
    public static class HttpHeadersExtensions
    {
        /// <summary>
        /// Sets authorization header. 
        /// </summary>
        /// <param name="headers">The http request header.</param>
        /// <param name="value">The header value.</param>
        public static void SetAuthorization(this HttpRequestHeaders headers, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
                headers.TryAddWithoutValidation("Authorization", value);
        }

        /// <summary>
        /// Sets authorization header. 
        /// </summary>
        /// <param name="headers">The http request header.</param>
        /// <param name="value1">The header value.</param>
        /// <param name="value2">The header value.</param>
        public static void SetAuthorization(this HttpRequestHeaders headers, string value1, string value2)
        {
            if (string.IsNullOrWhiteSpace(value1))
            {
                headers.SetAuthorization(value2);
            }
            else
            {
                headers.SetAuthorization(value1);
            }
        }

        /// <summary>
        /// Sets range header.
        /// </summary>
        /// <param name="headers">The http request header.</param>
        /// <param name="value">The header value.</param>
        public static void SetRange(this HttpRequestHeaders headers, RangeHeaderValue value)
        {
            if (value != null)
                headers.Range = value;
        }

        /// <summary>
        /// Sets part number header.
        /// </summary>
        /// <param name="headers">The http request header.</param>
        /// <param name="value">The header value.</param>
        public static void SetPartNumber(this HttpRequestHeaders headers, int value)
        {
            if (value != 0)
                headers.Add("x-bz-part-number", value.ToString());
        }

        /// <summary>
        /// Sets test mode header.
        /// </summary>
        /// <param name="headers">The http request header.</param>
        /// <param name="value">The header value.</param>
        public static void SetTestMode(this HttpRequestHeaders headers, string value)
        {
            if (!string.IsNullOrWhiteSpace(value)) 
                    headers.Add("x-bz-test-mode", value);            
        }

        /// <summary>
        /// Sets bz-info header values.
        /// </summary>
        /// <param name="headers">The http request header.</param>
        /// <param name="value">The header value.</param>
        public static void SetBzInfo(this HttpRequestHeaders headers, IDictionary<string,string> value)
        {
            foreach (var h in value.ToDictionary(a => $"x-bz-info-{a.Key}", a => a.Value.ToUrlEncode()))
                headers.Add(h.Key, h.Value);
        }

        /// <summary>
        /// Gets bz-info header values.
        /// </summary>
        /// <param name="headers">The http request header.</param>
        /// <param name="response">The download file response.</param>
        public static void GetBzInfo(this HttpResponseHeaders headers, DownloadFileResponse response)
        {
            foreach (var header in headers.Select((KeyValuePair<string, IEnumerable<string>> x) => new { key = x.Key.ToLower(), value = x.Value.First() }))
            {
                if (header.key == "x-bz-file-id") response.FileId = header.value;
                if (header.key == "x-bz-file-name") response.FileName = header.value.ToUrlDecode();
                if (header.key == "x-bz-content-sha1") response.ContentSha1 = header.value;
                if (header.key.StartsWith("x-bz-info-")) response.FileInfo[header.key.Substring("x-bz-info-".Length)] = header.value.ToUrlDecode();
            }
        }
    }
}
