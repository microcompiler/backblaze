using System.Diagnostics;

using Newtonsoft.Json;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains the results of a <see cref="CancelLargeFileRequest"/> operation.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class CancelLargeFileResponse : IResponse
    {
        /// <summary>
        /// The unique identifier of the file whose upload that was canceled.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string FileId { get; internal set; }

        /// <summary>
        /// The account that the bucket is in.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string AccountId { get; internal set; }

        /// <summary>
        /// The unique identifier of the bucket.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string BucketId { get; internal set; }

        /// <summary>
        /// The name of the file that was canceled.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string FileName { get; internal set; }

        ///	<summary>
        ///	Debugger display for this object.
        ///	</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{{{nameof(BucketId)}: {BucketId}, {nameof(FileId)}: {FileId}, {nameof(FileName)}: {FileName}}}"; }
        }
    }
}
