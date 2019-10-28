using System.Diagnostics;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains the results of a <see cref="UploadPartRequest"/> operation.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class UploadPartResponse : PartItem, IResponse
    {
        ///	<summary>
        ///	Debugger display for this object.
        ///	</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{{{nameof(FileId)}: {FileId}, {nameof(PartNumber)}: {PartNumber}, {nameof(UploadTimestamp)}: {UploadTimestamp}, , {nameof(ContentSha1)}: {ContentSha1}}}"; }
        }
    }
}
