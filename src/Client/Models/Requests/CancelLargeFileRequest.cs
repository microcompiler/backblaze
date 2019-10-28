using System;
using System.Diagnostics;

using Newtonsoft.Json;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains information to create a <see cref="CancelLargeFileRequest"/>.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class CancelLargeFileRequest : IRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CancelLargeFileRequest"/> class.
        /// </summary>
        /// <param name="fileId">The file id to cancel.</param>
        public CancelLargeFileRequest(string fileId)
        {
            // Validate required arguments
            if (string.IsNullOrWhiteSpace(fileId))
                throw new ArgumentException("Argument can not be null, empty, or consist only of white-space characters.", nameof(fileId));

            // Initialize and set required properties
            FileId = fileId;
        }

        /// <summary>
        /// The file id to cancel.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string FileId { get; private set; }

        /// <summary>
        /// Debugger display for this object.
        /// </summary>
        [JsonIgnore]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{{{nameof(FileId)}: {FileId}}}"; }
        }
    }
}
