using Bytewizer.Backblaze.Extensions;
using System.Diagnostics;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains the results of a <see cref="UploadFileRequest"/> operation.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class UploadFileResponse : FileItem, IResponse
    {
        /// <summary>
        /// Gets or sets the SHA1 of the bytes stored in the file as a 40-digit hex string. Large files do not have SHA1 checksums and the value is "none".
        /// The value is <c>null</c> when the action is <see cref="ActionType.Hide"/> or <see cref="ActionType.Folder"/>. 
        /// </summary>
        public override string ContentSha1
        {
            get
            {
                if (_contentSha1.Contains("none"))
                {
                    return FileInfo.GetLargeFileSha1();
                }
                else
                {
                    return _contentSha1;
                }
            }
            set { _contentSha1 = value; }
        }
        private string _contentSha1;

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
