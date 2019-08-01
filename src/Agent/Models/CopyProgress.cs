using System;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// The rate of a stream copy operation.
    /// </summary>
    public class CopyProgress : ICopyProgress
    {
        /// <summary>
        /// Create a new CopyRate instance.
        /// </summary>
        public CopyProgress(TimeSpan totalTransferTime, long bytesPerSecond, long bytesTotal, long expectedBytes)
        {
            TransferTime = totalTransferTime;
            BytesPerSecond = bytesPerSecond;
            BytesTransferred = bytesTotal;
            ExpectedBytes = expectedBytes;
        }

        /// <summary>
        /// The total time elapsed so far.
        /// </summary>
        public TimeSpan TransferTime { get; private set; }
        
        /// <summary>
        /// The instantaneous data transfer rate.
        /// </summary>
        public long BytesPerSecond { get; private set; }
        
        /// <summary>
        /// The total number of bytes transferred so far.
        /// </summary>
        public long BytesTransferred { get; private set; }
        
        /// <summary>
        /// The total number of bytes expected to be copied.
        /// </summary>
        public long ExpectedBytes { get; private set; }
        
        /// <summary>
        /// The percentage complete as a value 0-1.
        /// </summary>
        public double PercentComplete => ExpectedBytes <= 0 ? 0 : (double)BytesTransferred / ExpectedBytes;
    }
}
