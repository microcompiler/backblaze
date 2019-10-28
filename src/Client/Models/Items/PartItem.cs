using System;
using System.Diagnostics;

using Newtonsoft.Json;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Represents information related to a <see cref="PartItem"/>.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class PartItem : IItem
    {
        /// <summary>
        /// Gets or sets the unique identifier for this file. 
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string FileId { get; set; }

        /// <summary>
        /// Gets or sets the part number.  
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string PartNumber { get; set; }

        /// <summary>
        /// Gets or sets the number of bytes stored in the part.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public long ContentLength { get; set; }

        /// <summary>
        /// Gets or sets the SHA1 of the bytes stored in the part.  
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string ContentSha1 { get; set; }

        /// <summary>
        /// Gets or sets a UTC time when this part was uploaded.
        /// </summary>
        [JsonProperty(Required = Required.Always)]      
        public DateTime UploadTimestamp { get; set; }

        /// <summary>
        /// Debugger display for this object.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{{{nameof(FileId)}: {FileId}, {nameof(PartNumber)}: {PartNumber}, {nameof(UploadTimestamp)}: {UploadTimestamp}}}"; }
        }
    }
}
