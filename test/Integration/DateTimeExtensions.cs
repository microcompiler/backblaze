using System;

/// <summary>
/// Contains extension methods for <see cref="DateTime"/> object.
/// </summary>
namespace Backblaze.Tests.Integration
{
    public static class DateTimeExtensions
    {
        public static bool IsClose(this DateTime value)
        {
            return (DateTime.UtcNow.Subtract(value).TotalSeconds < 10);
        }
    }
}
