using System;
using System.Diagnostics;
using System.Net.Http.Headers;

using Newtonsoft.Json;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains information to create a download file by id request.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class DownloadFileByIdRequest : IRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadFileByIdRequest"/> class for downloading files from <see cref="BucketType.AllPublic"/> buckets.
        /// </summary>
        /// <param name="fileId">The unique identifier for the file.</param>
        public DownloadFileByIdRequest(string fileId)
        {
            // Validate required arguments
            if (string.IsNullOrWhiteSpace(fileId))
                throw new ArgumentException("Argument can not be null, empty, or consist only of white-space characters.", nameof(fileId));
 
            // Initialize and set required properties
            FileId = fileId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadFileByIdRequest"/> class for downloading files from <see cref="BucketType.AllPrivate"/> buckets.
        /// </summary>
        /// <param name="fileId">The unique identifier for the file.</param>
        /// <param name="authorizationToken">The authorization token must be used when downloading files from <see cref="BucketType.AllPrivate"/> buckets.</param>
        public DownloadFileByIdRequest(string fileId, string authorizationToken)
        {
            // Validate required arguments
            if (string.IsNullOrWhiteSpace(fileId))
                throw new ArgumentException($"value can not be null, empty, or consist only of white-space characters.", nameof(fileId));

            if (string.IsNullOrWhiteSpace(authorizationToken))
                throw new ArgumentException($"Argument '{nameof(authorizationToken)}' can not be null, empty, or consist only of white-space characters.");

            // Initialize and set required properties
            FileId = fileId;
            Authorization = authorizationToken;
        }

        /// <summary>
        /// The unique identifier for the file.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string FileId { get; private set; }

        /// <summary>
        /// A standard byte-range request which will return just part of the stored file. 
        /// </summary>
        [JsonIgnore]
        public RangeHeaderValue Range { get; set; }

        /// <summary>
        /// If this is present, B2 will use it as the value of the Content-Disposition header, overriding any 
        /// 'b2-content-disposition' specified when the file was uploaded. Parameter continuations are not supported.
        /// </summary>
        /// <remarks>
        /// Note that this file info will not be included in downloads as a x-bz-info-b2-content-disposition header. 
        /// Instead, it (or the value specified in a request) will be in the Content-Disposition
        /// </remarks>
        [JsonIgnore]
        public ContentDispositionHeaderValue ContentDisposition { get; set; }

        /// <summary>
        /// The authorization token that must be used when uploading files.  
        /// </summary>
        [JsonIgnore]
        public string Authorization { get; set; }

        ///	<summary>
        ///	Debugger display for this object.
        ///	</summary>
        [JsonIgnore]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{{{nameof(FileId)}: {FileId}, {nameof(Authorization)}: {Authorization}}}"; }
        }
    }
}
