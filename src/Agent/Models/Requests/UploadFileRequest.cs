using System;
using System.Diagnostics;

using Bytewizer.Backblaze.Extensions;
using System.Net.Http.Headers;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains information to create a upload file request.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class UploadFileRequest : IRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UploadFileRequest"/> class.
        /// </summary>
        /// <param name="uploadUrl">The url used to upload this file.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="authorizationToken">The authorization token that must be used when uploading files.</param>
        public UploadFileRequest(Uri uploadUrl, string fileName, string authorizationToken)
        {
            // Validate required arguments
            if (uploadUrl == null)
                throw new ArgumentNullException("Argument can not be null", nameof(uploadUrl));

            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("Argument can not be null, empty, or consist only of white-space characters.", nameof(fileName));

            if (string.IsNullOrWhiteSpace(authorizationToken))
                throw new ArgumentException("Argument can not be null, empty, or consist only of white-space characters.", nameof(authorizationToken));

            // Initialize and set required properties
            UploadUrl = uploadUrl;
            FileName = fileName;
            AuthorizationToken = authorizationToken;
        }

        /// <summary>
        /// The url used to upload file.  
        /// </summary>
        public Uri UploadUrl { get; private set; }

        /// <summary>
        /// The name of the file.
        /// </summary>
        public string FileName
        {
            get { return _FileName.AbsolutePath; }
            private set { _FileName = new Uri(value); }
        }
        private Uri _FileName;

        /// <summary>
        /// The authorizationToken that must be used when uploading files with this URL. This token is
        /// valid for 24 hours or until the upload endpoint rejects an upload. 
        /// </summary>
        public string AuthorizationToken { get; private set; }

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
        /// If this is present B2 will use it as the value of the Content-Disposition header when the file is downloaded 
        /// unless it's overridden by a value given in the download request. Parameter continuations are not supported.
        /// </summary>
        /// <remarks>
        /// Note that this file info will not be included in downloads as a x-bz-info-b2-content-disposition header. 
        /// Instead, it (or the value specified in a request) will be in the Content-Disposition
        /// </remarks>
        public ContentDispositionHeaderValue ContentDisposition
        {
            get => FileInfo.GetContentDisposition();
            set => FileInfo.SetContentDisposition(value);
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
            get { return $"{{{nameof(UploadUrl)}: {UploadUrl.AbsolutePath}, {nameof(FileName)}: {FileName}}}"; }
        }
    }
}
