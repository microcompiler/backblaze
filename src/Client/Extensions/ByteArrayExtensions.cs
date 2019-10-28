using System.Text;

namespace Bytewizer.Backblaze.Extensions
{
    /// <summary>
    /// Contains extension methods for byte arrays. 
    /// </summary>
    public static class ByteArrayExtensions
    {
        /// <summary>
        /// Convert a byte array to a hex string.
        /// </summary>
        /// <param name="buffer">The byte array.</param>
        public static string ToHex(this byte[] buffer)
        {
            var sb = new StringBuilder(buffer.Length * 2);

            foreach (var b in buffer)
                sb.AppendFormat("{0:x2}", b);

            return sb.ToString();
        }
    }
}
