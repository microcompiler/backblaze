using System.Diagnostics;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains the results of a <see cref="ListFileNamesRequest"/> operation.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class ListFileNamesResponse : IResponse
    {
        /// <summary>
        /// An list of file objects each one describing one file or folder.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public List<FileObject> Files { get; internal set; }

        /// <summary>
        /// What to pass in to startFileName for the next search to continue where this one left off,
        /// or null if there are no more files. Note this this may not be the name of an actual file,
        /// but using it is guaranteed to find the next file in the bucket. 
        /// </summary>
        public string NextFileName { get; internal set; }

        ///	<summary>
        ///	Debugger display for this object.
        ///	</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{{{nameof(Files)} {nameof(Files.Count)} = {Files.Count}, {nameof(NextFileName)}: {NextFileName}}}"; }
        }
    }
}
