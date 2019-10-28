using System;
using System.Collections.Generic;
using System.Diagnostics;

using Newtonsoft.Json;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Represents information related to a <see cref="FileItem"/>.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class FileItem : IItem
    {
        /// <summary>
        /// Gets or sets the account that owns the file. 
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string AccountId { get; set; }

        /// <summary>
        /// Gets or sets the file action state.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public ActionType Action { get; set; }

        /// <summary>
        /// Gets or sets the bucket id the file is located in. 
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string BucketId { get; set; }

        /// <summary>
        /// Gets or sets the number of bytes stored in the file. Only useful when the action is <see cref="ActionType.Upload"/>. 
        /// Always 0 when the action is <see cref="ActionType.Start"/>, <see cref="ActionType.Hide"/>", or <see cref="ActionType.Folder"/>.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public long ContentLength { get; set; }

        /// <summary>
        /// Gets or sets the SHA1 of the bytes stored in the file as a 40-digit hex string. Large files do not have SHA1 checksums and the value is "none".
        /// The value is <c>null</c> when the action is <see cref="ActionType.Hide"/> or <see cref="ActionType.Folder"/>. 
        /// </summary>
        public string ContentSha1 { get; set; }

        /// <summary>
        /// Gets or sets the file content type. When the action is <see cref="ActionType.Upload"/> or <see cref="ActionType.Start"/>, the MIME type of the file as specified when the file 
        /// was uploaded. For <see cref="ActionType.Hide"/> action always "application/x-bz-hide-marker". For <see cref="ActionType.Folder"/> action is always <c>null</c>. 
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for this version of this file. Used with <see cref="GetFileInfoRequest.FileId"/>, <see cref="DownloadFileByIdRequest.FileId"/>,
        /// and <see cref="DeleteFileVersionRequest.FileId"/>. The value is <c>null</c> when for action <see cref="ActionType.Folder"/>. 
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string FileId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Dictionary{TKey, TValue}"/> stored with the file. List is limited to 10 items.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public FileInfo FileInfo { get; set; } 

        /// <summary>
        /// Gets or sets the name of this file.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the UTC time when this file was uploaded.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public DateTime UploadTimestamp { get; set; }

        /// <summary>
        /// Debugger display for this object.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{{{nameof(FileName)}: {FileName}, {nameof(ContentType)}: {ContentType}}}"; }
        }
    }
}
