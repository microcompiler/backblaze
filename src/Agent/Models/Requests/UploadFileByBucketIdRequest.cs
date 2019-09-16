using System;
using System.Diagnostics;
using System.Collections.Generic;

using Bytewizer.Backblaze.Extensions;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains information to create a large file upload request.
    /// </summary>
    public class UploadFileByBucketIdRequest : IRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UploadFileRequest"/> class.
        /// </summary>
        /// <param name="bucketId">The bucket id you want to upload to.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="authorizationToken">The authorization token that must be used when uploading files.</param>
        public UploadFileByBucketIdRequest(string bucketId, string fileName)
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
        public string BucketId { get; private set; }

        /// <summary>
        /// The name of the file.
        /// </summary>
        public string FileName
        {
            get { return _FileName.ToPath(); }
            private set { _FileName = new Uri(value, UriKind.RelativeOrAbsolute); }
        }
        private Uri _FileName;

        /// <summary>
        /// The MIME type of the content of the file, which will be returned in the Content-Type header when
        /// downloading the file. Use the Content-Type b2/x-auto to automatically set the stored Content-Type
        /// post upload. In the case where a file extension is absent or the lookup fails, the Content-Type is
        /// set to application/octet-stream.
        /// </summary>
        public string ContentType { get; set; } = "b2/x-auto";

        /// <summary>
        /// The content has a last modified time concept.   
        /// </summary>
        public DateTime LastModified
        {
            get => FileInfo.GetLastModified();
            set => FileInfo.SetLastModified(value);
        }

        /// <summary>
        /// The custom information that will be uploaded with the file. Up to 10 of these headers may be present.
        /// </summary>
        /// <remarks>
        /// The * part of the header name is replaced with the name of a custom field in the file information stored with the file
        /// and the value is an arbitrary UTF-8 string, percent-encoded. The same info headers sent with the upload will be returned
        /// with the download. The header name is case insensitive.
        /// </remarks>
        public FileInfo FileInfo { get; set; } = new FileInfo();

        ///	<summary>
        ///	Debugger display for this object.
        ///	</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{{{nameof(BucketId)}: {BucketId}, {nameof(FileName)}: {FileName}}}"; }
        }
    }
}
