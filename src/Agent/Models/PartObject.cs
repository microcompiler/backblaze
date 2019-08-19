using System;
using System.Diagnostics;

using Newtonsoft.Json;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains response information related to a file part.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class PartObject
    {
        /// <summary>
        /// The unique identifier for this file. 
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string FileId { get; set; }

        /// <summary>
        /// The part number it is.  
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string PartNumber { get; set; }

        /// <summary>
        /// The number of bytes stored in the part.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public long ContentLength { get; set; }

        /// <summary>
        /// The SHA1 of the bytes stored in the part.  
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string ContentSha1 { get; set; }

        /// <summary>
        /// This is a UTC time when this part was uploaded.
        /// </summary>
        [JsonProperty(Required = Required.Always)]      
        public DateTime UploadTimestamp { get; set; }

        ///	<summary>
        ///	Debugger display for this object.
        ///	</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{{{nameof(FileId)}: {FileId}, {nameof(PartNumber)}: {PartNumber}, {nameof(UploadTimestamp)}: {UploadTimestamp}}}"; }
        }
    }
}
