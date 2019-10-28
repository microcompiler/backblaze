using System.Diagnostics;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains the results of a <see cref="HideFileRequest"/> operation.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class HideFileResponse : FileItem, IResponse
    {
        ///	<summary>
        ///	Debugger display for this object.
        ///	</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{{{nameof(FileId)}: {FileId}, {nameof(FileName)}: {FileName}, {nameof(ContentType)}: {ContentType}, , {nameof(ContentSha1)}: {ContentSha1}}}"; }
        }
    }
}
