using System;
using System.Diagnostics;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains information to create a finish large file request.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class FinishLargeFileRequest : IRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FinishLargeFileRequest"/> class. 
        /// </summary>
        /// <param name="fileId">The file id of the large file to finish.</param>
        /// <param name="partSha1Array">An array of hex SHA1 checksums for the parts of the large file.</param>
        public FinishLargeFileRequest(string fileId, List<string> partSha1Array)
        {
            // Validate required arguments
            if (string.IsNullOrWhiteSpace(fileId))
                throw new ArgumentException("Argument can not be null, empty, or consist only of white-space characters.", nameof(fileId));

            if (partSha1Array.Count == 0)
                throw new ArgumentException($"Argument must containe at least 1 SHA1 checksum.");

            // Initialize and set required properties
            FileId = fileId;
            PartSha1Array = partSha1Array;
        }

        /// <summary>
        /// The file id of the large file to finish.
        /// </summary>
        /// <seealso cref="StartLargeFileResponse"/>
        [JsonProperty(Required = Required.Always)]
        public string FileId { get; private set; }

        /// <summary>
        /// An array of hex SHA1 checksums for the parts of the large file. 
        /// </summary>
        /// <remarks>
        /// This is a double-check that the right parts were uploaded in the right order and that none were missed. 
        /// Note that the part numbers start at 1 and the SHA1 of the part 1 is the first string in the array at index 0.  
        /// </remarks>
        [JsonProperty(Required = Required.Always)]
        public List<string> PartSha1Array { get; private set; }

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
