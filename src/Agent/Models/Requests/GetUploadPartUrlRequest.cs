using System;
using System.Diagnostics;

using Newtonsoft.Json;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains information to create a get upload part url request.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class GetUploadPartUrlRequest : IRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetUploadPartUrlRequest"/> class.
        /// </summary>
        /// <param name="fileId">The large file id whose parts you want to upload.</param>
        public GetUploadPartUrlRequest(string fileId)
        {
            // Validate required arguments
            if (string.IsNullOrWhiteSpace(fileId))
                throw new ArgumentException("Argument can not be null, empty, or consist only of white-space characters.", nameof(fileId));

            // Initialize and set required properties
            FileId = fileId;
        }

        /// <summary>
        /// The large file id whose parts you want to upload.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string FileId { get; private set; }

        ///	<summary>
        ///	Debugger display for this object.
        ///	</summary>
        [JsonIgnore]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{{{nameof(FileId)}: {FileId}}}"; }
        }
    }
}
