using System;

namespace Bytewizer.Backblaze.Extensions
{
    /// <summary>
    /// Contains extension methods for <see cref="DateTime"/>.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Defines the epoch.
        /// </summary>
        private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Converts to a <see cref="DateTime"/> from epoch (ISO 8601) datetime.
        /// </summary>
        /// <param name="unixTime">The epoch milliseconds to convert.<see cref="long"/></param>
        public static DateTime FromEpoch(this long unixTime)
        {
            return _epoch.AddMilliseconds(unixTime);
        }

        /// <summary>
        /// Converts a <see cref="DateTime"/> to epoch (ISO 8601) datetime with UTC offset.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> to convert</param>
        public static long ToEpoch(this DateTime date)
        {
            return Convert.ToInt64((date.ToUniversalTime() - _epoch).TotalMilliseconds);
        }
    }
}
