using System;
using System.Diagnostics;
using System.Collections.Generic;
using Bytewizer.Backblaze.Extensions;

using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains information to create a <see cref="StartLargeFileRequest"/>.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class StartLargeFileRequest : IRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StartLargeFileRequest"/> class.
        /// </summary>
        /// <param name="bucketId">The bucket id the file will go in.</param>
        /// <param name="fileName">The name of the large file.</param>
        public StartLargeFileRequest(string bucketId, string fileName)
        {
            // Validate required arguments
            if (string.IsNullOrWhiteSpace(bucketId))
                throw new ArgumentException("Argument can not be null, empty, or consist only of white-space characters.", nameof(bucketId));

            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("Argument can not be null, empty, or consist only of white-space characters.", nameof(fileName));

            // Initialize and set required properties
            BucketId = bucketId;
            FileName = fileName;
        }

        /// <summary>
        /// The bucket id the file will go in.  
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string BucketId { get; private set; }

        /// <summary>
        /// The name of the file. See Files for requirements on file names.   
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string FileName { get; private set; }

        /// <summary>
        /// The MIME type of the content of the file, which will be returned in the Content-Type header when downloading the file.
        /// Use the Content-Type b2/x-auto to automatically set the stored Content-Type post upload. In the case where a file
        /// extension is absent or the lookup fails, the Content-Type is set to application/octet-stream. The Content-Type mappings
        /// can be perused here.   
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string ContentType { get; set; } = "b2/x-auto";

        /// <summary>
        /// The content has a last modified time concept.   
        /// </summary>
        [JsonIgnore]
        public DateTime LastModified
        {
            get => FileInfo.GetLastModified();
            set => FileInfo.SetLastModified(value);
        }

        /// <summary>
        /// If this is present B2 will use it as the value of the Content-Disposition header when the file is downloaded 
        /// unless it's overridden by a value given in the download request. Parameter continuations are not supported.
        /// </summary>
        /// <remarks>
        /// Note that this file info will not be included in downloads as a x-bz-info-b2-content-disposition header. 
        /// Instead, it (or the value specified in a request) will be in the Content-Disposition
        /// </remarks>
        [JsonIgnore]
        public ContentDispositionHeaderValue ContentDisposition
        {
            get => FileInfo.GetContentDisposition();
            set => FileInfo.SetContentDisposition(value);
        }

        /// <summary>
        /// The name/value pairs for the custom file info.
        /// </summary>
        public FileInfo FileInfo { get; set; }

        ///	<summary>
        ///	Debugger display for this object.
        ///	</summary>
        [JsonIgnore]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{{{nameof(BucketId)}: {BucketId}, {nameof(FileName)}: {FileName}}}"; }
        }
    }
}
