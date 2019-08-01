using System;
using System.Diagnostics;

using Newtonsoft.Json;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains information to create a list parts request.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class ListPartsRequest : IRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListPartsRequest"/> class.
        /// </summary>
        /// <param name="fileId">The unique identifier for the file.</param>
        public ListPartsRequest(string fileId)
        {
            // Validate required arguments
            if (string.IsNullOrWhiteSpace(fileId))
                throw new ArgumentException("Argument can not be null, empty, or consist only of white-space characters.", nameof(fileId));

            // Initialize and set required properties
            FileId = fileId;
        }

        /// <summary>
        /// The file id whose parts will be listed. 
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string FileId { get; private set; }

        /// <summary>
        /// The maximum number of parts to return from this call. The default value is 100 and the maximum allowed is 1000. 
        public int MaxPartCount 
        {
            get { return _maxPartCount; }
            set
            {
                if (value < 1 || value > 1000)
                    throw new ArgumentOutOfRangeException($"Argument must be a minimum of 1 and a maximum of 1000.", nameof(MaxPartCount));
                _maxPartCount = value;
            }
        }
        private int _maxPartCount;

        /// <summary>
        /// The first part to return. If there is a part with this number it will be returned as the first
        /// in the list. If not the returned list will start with the first part number after this one. 
        /// </summary>
        public string StartPartNumber { get; set; }

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
