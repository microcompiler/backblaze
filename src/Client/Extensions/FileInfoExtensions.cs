using System;
using System.Net.Http.Headers;

using Bytewizer.Backblaze.Models;

namespace Bytewizer.Backblaze.Extensions
{
    /// <summary>
    /// Contains extension methods for <see cref="FileInfo"/> object.
    /// </summary>
    public static class FileInfoExtensions
    {
        /// <summary>
        /// Sets the content disposition to the file info dictionary.
        /// </summary>
        /// <param name="fileInfo">The file info dictionary.</param>
        /// <param name="value">The date value to set.</param>
        public static void SetContentDisposition(this FileInfo fileInfo, ContentDispositionHeaderValue value)
        {
            if (value != null)
                fileInfo.Add("b2-content-disposition", value.ToString());
        }

        /// <summary>
        /// Gets the content disposition from the file info dictionary.
        /// </summary>
        /// <param name="fileInfo">The file info dictionary.</param>
        public static ContentDispositionHeaderValue GetContentDisposition(this FileInfo fileInfo)
        {
            fileInfo.TryGetValue("b2-content-disposition", out string value);
            return ContentDispositionHeaderValue.Parse(value);
        }

        /// <summary>
        /// Sets the last modified date to the file info dictionary.
        /// </summary>
        /// <param name="fileInfo">The file info dictionary.</param>
        /// <param name="value">The date value to set.</param>
        public static void SetLastModified(this FileInfo fileInfo, DateTime value)
        {
            if (value != DateTime.MinValue)
                fileInfo.Add("src_last_modified_millis", value.ToEpoch().ToString());
        }

        /// <summary>
        /// Gets the last modified date from the file info dictionary.
        /// </summary>
        /// <param name="fileInfo">The file info dictionary.</param>
        public static DateTime GetLastModified(this FileInfo fileInfo)
        {
            fileInfo.TryGetValue("src_last_modified_millis", out string value);
            return long.Parse(value).FromEpoch();
        }

        /// <summary>
        /// Sets the large file SHA1 header.
        /// </summary>
        /// <param name="fileInfo">The file info dictionary.</param>
        /// <param name="value">The SHA1 checksum of the file.</param>
        public static void SetLargeFileSha1(this FileInfo fileInfo, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
                fileInfo.Add("large_file_sha1", value);
        }

        /// <summary>
        /// Sets the large file SHA1 header.
        /// </summary>
        /// <param name="fileInfo">The file info dictionary.</param>
        public static string GetLargeFileSha1(this FileInfo fileInfo)
        {
            fileInfo.TryGetValue("large_file_sha1", out string value);
            return value;
        }
    }
}
