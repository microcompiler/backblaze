using System;

namespace Bytewizer.Backblaze.Extensions
{
    /// <summary>
    /// Extensions methods for <see cref="string"/>.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Convert a string to a url encoded string.
        /// </summary>
        /// <param name="s">String to convert</param>
        static public string ToUrlEncode(this string s) => string.Equals(s, "/") ? s : Uri.EscapeDataString(s);

        /// <summary>
        /// Convert a url encoded string to a string.
        /// </summary>
        /// <param name="s">Url encoded string to convert</param>
        static public string ToUrlDecode(this string s) => string.Equals(s, "+") ? " " : Uri.EscapeDataString(s);
    }
}
