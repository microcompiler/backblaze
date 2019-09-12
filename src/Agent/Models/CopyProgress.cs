using System;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Represents the rate of a stream copy operation.
    /// </summary>
    public class CopyProgress : ICopyProgress
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CopyProgress"/> class.
        /// </summary>
        /// <param name="totalTransferTime">The total time elapsed so far.</param>
        /// <param name="bytesPerSecond">The instantaneous data transfer rate.</param>
        /// <param name="bytesTotal">The total number of bytes transferred so far.</param>
        /// <param name="expectedBytes">The total number of bytes expected to be copied.</param>
        public CopyProgress(TimeSpan totalTransferTime, long bytesPerSecond, long bytesTotal, long expectedBytes)
        {
            TransferTime = totalTransferTime;
            BytesPerSecond = bytesPerSecond;
            BytesTransferred = bytesTotal;
            ExpectedBytes = expectedBytes;
        }

        /// <summary>
        /// Gets the total time elapsed so far.
        /// </summary>
        public TimeSpan TransferTime { get; private set; }

        /// <summary>
        /// Gets the instantaneous data transfer rate.
        /// </summary>
        public long BytesPerSecond { get; private set; }
        
        /// <summary>
        /// Gets the total number of bytes transferred so far.
        /// </summary>
        public long BytesTransferred { get; private set; }
        
        /// <summary>
        /// Gets the total number of bytes expected to be copied.
        /// </summary>
        public long ExpectedBytes { get; private set; }
        
        /// <summary>
        /// The percentage complete as a value 0-1.
        /// </summary>
        public double PercentComplete => ExpectedBytes <= 0 ? 0 : (double)BytesTransferred / ExpectedBytes;
    }
}
