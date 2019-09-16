using System;
using System.Diagnostics;

using Newtonsoft.Json;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains request information to create a hide files request.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class HideFileRequest : IRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HideFileRequest"/> class.
        /// </summary>
        /// <param name="bucketId">The bucket id containing the file to hide.</param>
        /// <param name="fileName">The name of the file to hide.</param>
        public HideFileRequest(string bucketId, string fileName)
        {
            // Validate required arguments
            if (string.IsNullOrWhiteSpace(bucketId))
                throw new ArgumentException("Argument can not be null, empty, or consist only of white-space characters.", nameof(bucketId));

            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("Argument can not be null, empty, or consist only of white-space characters.", nameof(fileName));

            // Initialize and set required properties
            BucketId = bucketId;
            FileName = fileName;
        }

        /// <summary>
        /// The bucket id containing the file to hide.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string BucketId { get; private set; }

        /// <summary>
        /// The name of the file to hide. 
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string FileName { get; private set; }

        ///	<summary>
        ///	Debugger display for this object.
        ///	</summary>
        [JsonIgnore]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{{{nameof(BucketId)}: {BucketId}, {nameof(FileName)}: {FileName}}}"; }
        }
    }
}
