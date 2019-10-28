using System.Diagnostics;

using Newtonsoft.Json;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains the results of a <see cref="DeleteFileVersionRequest"/> operation.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class DeleteFileVersionResponse : IResponse
    {
        /// <summary>
        /// The unique id of the file version that was deleted.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string FileId { get; internal set; }

        /// <summary>
        /// The name of the file.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string FileName { get; internal set; }

        ///	<summary>
        ///	Debugger display for this object.
        ///	</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{{{nameof(FileId)}: {FileId}, {nameof(FileName)}: {FileName}}}"; }
        }
    }
}
