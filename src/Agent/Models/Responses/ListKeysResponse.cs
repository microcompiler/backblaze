using System.Diagnostics;
using System.Collections.Generic;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains the results of a <see cref="ListKeysRequest"/> operation.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class ListKeysResponse : IResponse
    {
        /// <summary>
        /// An list of key objects.
        /// </summary>
        public List<KeyItem> Keys { get; set; }

        /// <summary>
        /// What to pass in to <see cref="ListKeysRequest.StartApplicationKeyId"/> for the next search to continue where this one left off or null if there are no 
        /// more files. Note this this may not be the number of an actual key but using it is guaranteed to find the next file
        /// in the bucket.
        /// </summary>
        public string NextApplicationKeyId { get; internal set; }

        ///	<summary>
        ///	Debugger display for this object.
        ///	</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{{{nameof(Keys)} {nameof(Keys.Count)} = {Keys.Count}, {nameof(NextApplicationKeyId)}: {NextApplicationKeyId}}}"; }
        }
    }
}
