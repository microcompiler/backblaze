using System;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Collections.Generic;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains the results of a <see cref="DownloadFileByIdRequest"/> or <see cref="DownloadFileByNameRequest"/> operation.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class DownloadFileResponse : IResponse
    {
        /// <summary>
        /// The unique identifier for this version of this file. Used with <see cref="GetFileInfoRequest.FileId"/>, <see cref="DownloadFileByIdRequest.FileId"/>,
        /// and <see cref="DeleteFileVersionRequest.FileId"/>. The value is null when for action <see cref="ActionType.Folder"/>. 
        /// </summary>
        public string FileId { get; internal set; }

        /// <summary>
        /// The name of this file.
        /// </summary>
        public string FileName { get; internal set; }

        /// <summary>
        /// The number of bytes stored in the file. Only useful when the action is <see cref="ActionType.Upload"/>. 
        /// Always 0 when the action is <see cref="ActionType.Start"/>, <see cref="ActionType.Hide"/>", or <see cref="ActionType.Folder"/>.
        /// </summary>
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
        /// The custom information that was uploaded with the file. 
        /// </summary>
        public FileInfo FileInfo { get; internal set; } = new FileInfo();

        /// <summary>
        /// The UTC time when this file was uploaded.
        /// </summary>
        public DateTime UploadTimestamp { get; internal set; }

        /// <summary>
        /// Represents the value of the Cache-Control header. Max-age only inherited from bucket Info.
        /// </summary>
        public CacheControlHeaderValue CashControl { get; internal set; }

        /// <summary>
        /// If this is present B2 will use it as the value of the Content-Disposition header when the file is downloaded 
        /// unless it's overridden by a value given in the download request. Parameter continuations are not supported.
        /// </summary>
        /// <remarks>
        /// Note that this file info will not be included in downloads as a x-bz-info-b2-content-disposition header. 
        /// Instead, it (or the value specified in a request) will be in the Content-Disposition
        /// </remarks>
        public ContentDispositionHeaderValue ContentDisposition { get; internal set; }

        ///	<summary>
        ///	Debugger display for this object.
        ///	</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{{{nameof(FileId)}: {FileId}, {nameof(FileName)}: {FileName}, {nameof(ContentType)}: {ContentType}, {nameof(ContentSha1)}: {ContentSha1}}}"; }
        }
    }
}
