using System.Diagnostics;
using System.Collections.Generic;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains the results of a <see cref="ListBucketsRequest"/> operation.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class ListBucketsResponse :  IResponse
    {
        /// <summary>
        /// An list of bucket objects.
        /// </summary>
        public List<BucketItem> Buckets { get; set; }

        ///	<summary>
        ///	Debugger display for this object.
        ///	</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{{{nameof(Buckets)} {nameof(Buckets.Count)} = {Buckets.Count}}}"; }
        }
    }
}
