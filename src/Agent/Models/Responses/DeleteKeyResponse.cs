using System.Diagnostics;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains the results of a <see cref="DeleteKeyRequest"/> operation.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class DeleteKeyResponse : KeyObject, IResponse
    {
        ///	<summary>
        ///	Debugger display for this object.
        ///	</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{{{nameof(ApplicationKeyId)}: {ApplicationKeyId}, {nameof(KeyName)}: {KeyName}, {nameof(NamePrefix)}: {NamePrefix}, {nameof(ExpirationTimestamp)}: {ExpirationTimestamp}}}"; }
        }
    }
}
