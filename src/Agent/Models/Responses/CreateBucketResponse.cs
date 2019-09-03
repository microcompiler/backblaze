using System.Diagnostics;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains the results of a <see cref="CreateBucketRequest"/> operation.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class CreateBucketResponse : BucketItem,  IResponse
    {
        ///	<summary>
        ///	Debugger display for this object.
        ///	</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{{{nameof(BucketId)}: {BucketId}, {nameof(BucketName)}: {BucketName}, {nameof(BucketType)}: {BucketType.ToString()}}}"; }
        }
    }
}
