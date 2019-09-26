using System;

namespace Bytewizer.Backblaze.Extensions
{
    /// <summary>
    /// Extensions methods for <see cref="Uri"/>.
    /// </summary>
    public static class UriExtensions
    {
        /// <summary>
        /// Returns a <see cref="string"/> of the <see cref="Uri"/> absolute path.
        /// </summary>
        /// <param name="instance">The <see cref="Uri"/> instance to return.</param>
        public static string ToPath(this Uri instance)
        {
            if (instance.IsAbsoluteUri)
                return instance.AbsolutePath;

            return instance.OriginalString;
        }
    }
}
