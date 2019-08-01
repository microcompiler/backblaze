using System.Diagnostics;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains the results of a <see cref="ListPartsRequest"/> operation.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class ListPartsResponse : IResponse
    {
        /// <summary>
        /// A list of part objects each one describing one part.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public List<PartObject> Parts { get; internal set; }

        /// <summary>
        /// What to pass in to <see cref="ListPartsRequest.StartPartNumber"/> for the next search to continue where this one left off or null if there are no 
        /// more files. Note this this may not be the number of an actual part but using it is guaranteed to find the next file
        /// in the bucket.
        /// </summary>
        public string NextPartNumber { get; internal set; }

        ///	<summary>
        ///	Debugger display for this object.
        ///	</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{{{nameof(Parts)} {nameof(Parts.Count)} = {Parts.Count}, {nameof(NextPartNumber)}: {NextPartNumber}}}"; }
        }
    }
}
