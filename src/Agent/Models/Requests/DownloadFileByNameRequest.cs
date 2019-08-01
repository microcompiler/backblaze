using System;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains information to create a download file by name request.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class DownloadFileByNameRequest : IRequest
    {
        /// <summary>
        /// Initializes a new instance of the class for downloading files from <see cref="BucketType.AllPublic"/> buckets.
        /// </summary>
        /// <param name="bucketName">The unique name of the bucket the file is in.</param>
        /// <param name="fileName">The name of the remote file.</param>
        public DownloadFileByNameRequest(string bucketName, string fileName)
        {
            // Validate required arguments
            if (string.IsNullOrWhiteSpace(bucketName))
                throw new ArgumentException($"Argument '{nameof(bucketName)}' can not be null, empty, or consist only of white-space characters.", nameof(bucketName));
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException($"Argument '{nameof(fileName)}' can not be null, empty, or consist only of white-space characters.", nameof(fileName));
           
            // Initialize and set required properties
            BucketName = bucketName;
            FileName = fileName;
        }

        /// <summary>
        /// Initializes a new instance of the class for downloading files from <see cref="BucketType.AllPrivate"/> buckets.
        /// </summary>
        /// <param name="bucketName">The unique name of the bucket the file is in.</param>
        /// <param name="fileName">The name of the remote file.</param>
        /// <param name="authorizationToken">The authorization token must be used when downloading files from <see cref="BucketType.AllPrivate"/> buckets.</param>

        public DownloadFileByNameRequest(string bucketName, string fileName, string authorizationToken)
        {
            if (string.IsNullOrWhiteSpace(bucketName))
                throw new ArgumentException("Argument can not be null, empty, or consist only of white-space characters.", nameof(bucketName));
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("Argument can not be null, empty, or consist only of white-space characters.", nameof(fileName));
            if (string.IsNullOrWhiteSpace(authorizationToken))
                throw new ArgumentException("Argument can not be null, empty, or consist only of white-space characters.", nameof(authorizationToken));

            // Initialize and set required properties
            BucketName = bucketName;
            FileName = fileName;
            AuthorizationToken = authorizationToken;
        }

        /// <summary>
        /// The unique name of the bucket.
        /// </summary>
        public string BucketName { get; private set; }

        /// <summary>
        /// The remote name of the file.
        /// </summary>
        public string FileName
        {
            get { return _RemoteFileName.AbsolutePath; }
            private set { _RemoteFileName = new Uri(value); }
        }
        private Uri _RemoteFileName;

        /// <summary>
        /// The authorization token that must be used when uploading files.  
        /// </summary>
        public string AuthorizationToken { get; private set; }

        /// <summary>
        /// A standard byte-range request which will return just part of the stored file. 
        /// </summary>
        public RangeHeaderValue Range { get; set; }

        /// <summary>
        /// If this is present, B2 will use it as the value of the Content-Disposition header, overriding any 
        /// 'b2-content-disposition' specified when the file was uploaded. Parameter continuations are not supported.
        /// </summary>
        /// <remarks>
        /// Note that this file info will not be included in downloads as a x-bz-info-b2-content-disposition header. 
        /// Instead, it (or the value specified in a request) will be in the Content-Disposition
        /// </remarks>
        public ContentDispositionHeaderValue ContentDisposition { get; set; }

        ///	<summary>
        ///	Debugger display for this object.
        ///	</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{{{nameof(BucketName)}: {BucketName}, {nameof(FileName)}: {FileName}}}"; }
        }
    }
}
