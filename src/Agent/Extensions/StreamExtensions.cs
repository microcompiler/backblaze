using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

using Bytewizer.Backblaze.Models;

namespace Bytewizer.Backblaze.Extensions
{
    /// <summary>
    /// Extensions methods for <see cref="Stream"/>.
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        /// Provides a stream copy operation which supports a progress report action.
        /// </summary>
        /// <param name="source">The source stream. Must support reading.</param>
        /// <param name="destination">The destination stream. Must support writing.</param>
        /// <param name="bufferSize">The size of the buffer to allocate in bytes. Sane values are typically 4096-81920. Setting a buffer of more than ~85k is likely to degrade performance.</param>
        /// <param name="expectedTotalBytes">The number of bytes expected. If set to greater than zero, this will override source.Length for progress calculations.</param>
        /// <param name="progressReport">A progress action that will be used to report progress.</param>
        /// <param name="cancel">A typical cancellation token.</param>
        public static async Task CopyToAsync(this Stream source, Stream destination, int bufferSize = 32768, long expectedTotalBytes = 0, IProgress<ICopyProgress> progressReport = null, CancellationToken cancel = default)
        {
            if (source == null) { throw new ArgumentNullException("source"); }
            if (!source.CanRead) { throw new ArgumentException("Source stream must be readable.", "source"); }
            if (destination == null) { throw new ArgumentNullException("destination"); }
            if (!destination.CanWrite) { throw new ArgumentException("Destination stream must be writable.", "destination"); }
            if (bufferSize < 0) { throw new ArgumentOutOfRangeException(nameof(bufferSize)); }

            expectedTotalBytes = expectedTotalBytes == 0 ? (source.CanSeek ? source.Length : 0) : expectedTotalBytes;

            var buffer = new byte[bufferSize];
            long totalBytesRead = 0;
            int bytesRead;

            var totalTime = new System.Diagnostics.Stopwatch();
            var singleTime = new System.Diagnostics.Stopwatch();
            totalTime.Start();
            singleTime.Start();

            while ((bytesRead = await source.ReadAsync(buffer, 0, buffer.Length, cancel).ConfigureAwait(false)) != 0)
            {
                await destination.WriteAsync(buffer, 0, bytesRead, cancel).ConfigureAwait(false);
                totalBytesRead += bytesRead;
                long singleTicks = Math.Max(1, singleTime.ElapsedTicks);
                progressReport?.Report(new CopyProgress(totalTime.Elapsed, bytesRead * TimeSpan.TicksPerSecond / singleTicks, totalBytesRead, expectedTotalBytes));
                singleTime.Restart();

                if (cancel.IsCancellationRequested) { break; }
            }
        }

        /// <summary>
        /// Compute the SHA1 hash of a stream.
        /// </summary>
        /// <param name="source"></param>
        public static string ToSha1(this Stream source)
        {
            if (source == null)
            {
                return string.Empty;
            }
            else
            {
                using (var sha1 = SHA1.Create())
                {
                    var position = source.Position;

                    source.Position = 0;
                    var hash = sha1.ComputeHash(source).ToHex();
                    source.Position = position;
                    return hash;
                }
            }  
        }

        /// <summary>
        /// Compares this instance with a specified hash returns an <see cref="bool"/> that indicates whether this instance compares to specified hash.
        /// </summary>
        /// <param name="source"></param>
        /// /// <param name="hash"></param>
        public static bool CompareTo(this Stream source, string hash)
        {
            var streamHash = source.ToSha1();
            if (string.Equals(streamHash, hash)) 
                return true;

            return false;
        }
    }
}
