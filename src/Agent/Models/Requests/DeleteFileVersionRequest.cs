using System;
using System.Diagnostics;

using Newtonsoft.Json;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains information to create a delete file version request.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class DeleteFileVersionRequest: IRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteFileVersionRequest"/> class.
        /// </summary>
        /// <param name="fileName">The name of the file to delete.</param>
        /// <param name="fileId">The id of the file to delete.</param>
        public DeleteFileVersionRequest(string fileId, string fileName)
        {
            // Validate required arguments
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException($"Argument '{nameof(fileName)}' can not be null, empty, or consist only of white-space characters.", nameof(fileName));

            if (string.IsNullOrWhiteSpace(fileId))
                throw new ArgumentException($"Argument '{nameof(fileId)}' can not be null, empty, or consist only of white-space characters.", nameof(fileId));

            // Initialize and set required properties
            FileId = fileId;
            FileName = fileName;
        }

        /// <summary>
        /// The name of the file to delete. 
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string FileName { get; private set; }

        /// <summary>
        /// The id of the file to delete.
        /// See response <see cref="UploadFileResponse"/>, <see cref="ListFileNamesResponse"/>, <see cref="ListFileVersionResponse"/>. 
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
            get { return $"{{{nameof(FileName)}: {FileName}, {nameof(FileId)}: {FileId}}}"; }
        }
    }
}
