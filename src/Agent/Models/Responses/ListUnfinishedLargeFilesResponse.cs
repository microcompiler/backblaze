using System.Diagnostics;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains the results of a <see cref="ListUnfinishedLargeFilesRequest"/> operation.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class ListUnfinishedLargeFilesResponse : IResponse
    {
        /// <summary>
        /// A part objects each one describing one unfinished file.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public List<FileObject> Files { get; internal set; }

        /// <summary>
        /// What to pass in to <see cref="ListUnfinishedLargeFilesRequest.StartFileId"/> for the next search to continue where this one left off 
        /// or null if there are no more files. Note this this may not be the id of an 
        /// actual upload but using it is guaranteed to find the next upload.  
        /// </summary>
        public string NextFileId { get; internal set; }

        ///	<summary>
        ///	Debugger display for this object.
        ///	</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{{{nameof(Files)} {nameof(Files.Count)} = {Files.Count}, {nameof(NextFileId)}: {NextFileId}}}"; }
        }
    }
}
