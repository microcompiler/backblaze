using System;
using System.Diagnostics;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains response information related to a file.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class FileObject
    {
        /// <summary>
        /// The account that owns the file. 
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string AccountId { get; internal set; }

        /// <summary>
        /// The file action state.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public ActionType Action { get; internal set; }

        /// <summary>
        /// The bucket that the file is in. 
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string BucketId { get; internal set; }

        /// <summary>
        /// The number of bytes stored in the file. Only useful when the action is <see cref="ActionType.Upload"/>. 
        /// Always 0 when the action is <see cref="ActionType.Start"/>, <see cref="ActionType.Hide"/>", or <see cref="ActionType.Folder"/>.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public long ContentLength { get; internal set; }

        /// <summary>
        /// The SHA1 of the bytes stored in the file as a 40-digit hex string. Large files do not have SHA1 checksums, and the value is "none".
        /// The value is null when the action is <see cref="ActionType.Hide"/> or <see cref="ActionType.Folder"/>. 
        /// </summary>
        public string ContentSha1 { get; internal set; }

        /// <summary>
        /// When the action is <see cref="ActionType.Upload"/> or <see cref="ActionType.Start"/>, the MIME type of the file as specified when the file 
        /// was uploaded. For <see cref="ActionType.Hide"/> action always "application/x-bz-hide-marker". For <see cref="ActionType.Folder"/> action is always null. 
        /// </summary>
        public string ContentType { get; internal set; }

        /// <summary>
        /// The unique identifier for this version of this file. Used with <see cref="GetFileInfoRequest.FileId"/>, <see cref="DownloadFileByIdRequest.FileId"/>,
        /// and <see cref="DeleteFileVersionRequest.FileId"/>. The value is null when for action <see cref="ActionType.Folder"/>. 
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string FileId { get; internal set; }

        /// <summary>
        /// The custom information that was uploaded with the file. 
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public Dictionary<string, string> FileInfo { get; internal set; }

        /// <summary>
        /// The name of this file.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string FileName { get; internal set; }

        /// <summary>
        /// This is a UTC time when this file was uploaded.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public DateTime UploadTimestamp { get; internal set; }

        ///	<summary>
        ///	Debugger display for this object.
        ///	</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{{{nameof(FileName)}: {FileName}, {nameof(ContentType)}: {ContentType}}}"; }
        }
    }
}
