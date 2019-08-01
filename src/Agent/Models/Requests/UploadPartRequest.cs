using System;
using System.IO;
using System.Diagnostics;

using Bytewizer.Backblaze.Extensions;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains information to create a upload part request.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class UploadPartRequest : IRequest
    {
        /// <summary>
        /// Minimum numbers of large file parts.
        /// </summary>
        public const int MinimumPartNumber = 1;

        /// <summary>
        /// Maximum numbers of large file parts.
        /// </summary>
        public const int MaximumPartNumber = 10000;

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadPartRequest"/> class.
        /// </summary>
        /// <param name="uploadUrl">The url used to upload this file.</param>
        /// <param name="bytes">The part payload.</param>
        /// <param name="partNumber">The part number of the file.</param>
        /// <param name="authorizationToken">The authorization token that must be used when uploading files.</param>
        public UploadPartRequest(Uri uploadUrl, Stream content, int partNumber, string authorizationToken)
        {
            // Validate required arguments
            if (uploadUrl == null)
                throw new ArgumentNullException("Argument can not be null", nameof(uploadUrl));

            if (content == null)
                throw new ArgumentNullException("Argument can not be null", nameof(content));

            if (partNumber < MinimumPartNumber || partNumber > MaximumPartNumber)
                throw new ArgumentOutOfRangeException($"Argument must be a minimum of {MinimumPartNumber} and a maximum of {MaximumPartNumber}.", nameof(partNumber));

            if (string.IsNullOrWhiteSpace(authorizationToken))
                throw new ArgumentException("Argument can not be null, empty, or consist only of white-space characters.", nameof(authorizationToken));
            
            // Initialize and set required properties
            UploadUrl = uploadUrl;
            ContentStream = content;
            PartNumber = partNumber;
            AuthorizationToken = authorizationToken;
        }

        /// <summary>
        /// The url used to upload this file.  
        /// </summary>
        public Uri UploadUrl { get; private set; }

        /// <summary>
        /// The content stream payload.
        /// </summary>
        public Stream ContentStream { get; private set; }

        /// <summary>
        /// A number from 1 to 10000. The parts uploaded for one file must have contiguous numbers starting with 1.
        /// </summary>
        public int PartNumber { get; private set; } = 1;

        /// <summary>
        /// The authorization token that must be used when uploading files with this URL. This token is
        /// valid for 24 hours or until the uploadUrl endpoint rejects an upload, see b2_upload_part. 
        /// </summary>
        public string AuthorizationToken { get; private set; }

        /// <summary>
        /// The number of bytes in the file being uploaded.
        /// </summary>
        public long ContentLength
        {
            get { return ContentStream.Length; }
        }

        /// <summary>
        /// The SHA1 checksum of the content of the file. B2 will check this when the file is uploaded to make sure
        /// that the file arrived correctly.  
        /// </summary>
        public string ContentSha1
        {
            get
            {
                var hash = ContentStream.ToSha1();
                ContentStream.Position = 0;
                return hash;
            }
        }

        ///	<summary>
        ///	Debugger display for this object.
        ///	</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{{{nameof(UploadUrl)}: {UploadUrl.AbsolutePath}, {nameof(PartNumber)}: {PartNumber}}}"; }
        }
    }
}
