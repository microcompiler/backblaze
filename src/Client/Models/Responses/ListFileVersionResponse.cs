using System.Diagnostics;
using System.Collections.Generic;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains the results of a <see cref="ListFileVersionRequest"/> operation.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class ListFileVersionResponse : IResponse
    {
        /// <summary>
        /// An list of file objects each one describing one file or folder.
        /// </summary>
        public List<FileItem> Files { get; set; }

        /// <summary>
        /// What to pass in to <see cref="ListFileVersionRequest.StartFileName"/> for the next search to continue where this one left off
        /// or null if there are no more files. Note this this may not be the name of an actual file
        /// but using it is guaranteed to find the next file in the bucket. 
        /// </summary>
        public string NextFileName { get; set; }

        /// <summary>
        /// What to pass in to <see cref="ListFileVersionRequest.StartFileId"/> for the next search to continue where this one left off or null
        /// if there are no more files. Note this this may not be the ID of an actual file but using it is
        /// guaranteed to find the next file version in the bucket. 
        /// </summary>
        public string NextFileId { get; set; }

        ///	<summary>
        ///	Debugger display for this object.
        ///	</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{{{nameof(Files)} {nameof(Files.Count)} = {Files.Count}, {nameof(NextFileName)}: {NextFileName}, {nameof(NextFileId)}: {NextFileId}}}"; }
        }
    }
}
