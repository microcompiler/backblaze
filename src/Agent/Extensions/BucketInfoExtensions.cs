using System.Net.Http.Headers;

using Bytewizer.Backblaze.Models;

namespace Bytewizer.Backblaze.Extensions
{
    /// <summary>
    /// Extensions methods for <see cref="BucketInfo"/> object.
    /// </summary>
    public static class BucketInfoExtensions
    {
        /// <summary>
        /// Sets the cache-control header.
        /// </summary>
        /// <param name="bucketInfo">The bucket info dictionary.</param>
        /// <param name="value">The cache control values.</param>
        public static void SetCacheControl(this BucketInfo bucketInfo, CacheControlHeaderValue value)
        {
            if (value != null)
                bucketInfo.Add("Cache-Control", value.ToString());
        }

        /// <summary>
        /// Gets the Cache-Control header.
        /// </summary>
        /// <param name="bucketInfo">The bucket info dictionary.</param>
        public static CacheControlHeaderValue GetCacheControl(this BucketInfo bucketInfo)
        {
            bucketInfo.TryGetValue("Cache-Control", out string value);
            return CacheControlHeaderValue.Parse(value);
        }
    }
}
