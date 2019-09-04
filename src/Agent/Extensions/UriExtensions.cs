using System;
using System.Collections.Generic;
using System.Text;

namespace Bytewizer.Backblaze.Extensions
{
    /// <summary>
    /// Extensions methods for <see cref="Uri"/>.
    /// </summary>
    public static class UriExtensions
    {
        public static string ToPath(this Uri instance)
        {
            if (instance.IsAbsoluteUri)
                return instance.AbsolutePath;

            return instance.OriginalString;
        }
    }
}
