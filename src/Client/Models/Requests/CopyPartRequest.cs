using System;
using System.Diagnostics;
using System.Net.Http.Headers;

using Newtonsoft.Json;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains information to create a copy part request.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class CopyPartRequest : IRequest
    {
        /// <summary>
        /// Minimum numbers of large file parts.
        /// </summary>
        public static readonly int MinimumPartNumber = 1;

        /// <summary>
        /// Maximum numbers of large file parts.
        /// </summary>
        public static readonly int MaximumPartNumber = 10000;

        /// <summary>
        /// Initializes a new instance of the <see cref="CopyPartRequest"/> class.
        /// </summary>
        /// <param name="sourceFileId">The unique identifier of the source file being copied.</param>
        /// <param name="largeFileId">The unique identifier of the large file the part will belong to.</param>
        /// <param name="partNumber">The part number of the file.</param>
        public CopyPartRequest(string sourceFileId, string largeFileId, int partNumber)
        {
            // Validate required arguments
            if (string.IsNullOrWhiteSpace(sourceFileId))
                throw new ArgumentException("Argument can not be null, empty, or consist only of white-space characters.", nameof(sourceFileId));

            if (string.IsNullOrWhiteSpace(largeFileId))
                throw new ArgumentException("Argument can not be null, empty, or consist only of white-space characters.", nameof(largeFileId));

            if (partNumber < MinimumPartNumber || partNumber > MaximumPartNumber)
                throw new ArgumentOutOfRangeException($"Argument must be a minimum of {MinimumPartNumber} and a maximum of {MaximumPartNumber}.", nameof(partNumber));

            // Initialize and set required properties
            SourceFileId = sourceFileId;
            LargeFileId = largeFileId;
            PartNumber = partNumber;
        }

        /// <summary>
        /// The unique identifier of the source file being copied.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string SourceFileId { get; private set; }

        /// <summary>
        /// The unique identifier of the large file the part will belong to as returned by <see cref="StartLargeFileResponse"/>.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string LargeFileId { get; private set; }

        /// <summary>
        /// A number from 1 to 10000. The parts uploaded for one file must have contiguous numbers starting with 1.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public int PartNumber { get; private set; } = 1;

        /// <summary>
        /// A standard byte-range request to copy. If not provided the whole source file will be copied. 
        /// </summary>
        public RangeHeaderValue Range { get; set; }

        /// <summary>
        /// Debugger display for this object.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{{{nameof(SourceFileId)}: {SourceFileId}, {nameof(LargeFileId)}: {LargeFileId}, {nameof(PartNumber)}: {PartNumber}}}"; }
        }
    }
}
