using System;
using System.Diagnostics;
using System.Net.Http.Headers;

using Bytewizer.Backblaze.Extensions;

using Newtonsoft.Json;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains information to create a file copy request.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class CopyFileRequest : IRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CopyFileRequest"/> class for copying files.
        /// </summary>
        /// <param name="sourceFileId">The unique identifier for the file to copy.</param>
        public CopyFileRequest(string sourceFileId, string fileName)
        {
            // Validate required arguments
            if (string.IsNullOrWhiteSpace(sourceFileId))
                throw new ArgumentException("Argument can not be null, empty, or consist only of white-space characters.", nameof(sourceFileId));

            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("Argument can not be null, empty, or consist only of white-space characters.", nameof(fileName));

            // Initialize and set required properties
            SourceFileId = sourceFileId;
            FileName = fileName;
        }

        /// <summary>
        /// The unique identifier of the source file being copied.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string SourceFileId { get; private set; }

        /// <summary>
        /// The unique identifier of the bucket where the copied file will be stored. If this is not set, the copied file will be added to the same bucket as the source file. 
        /// Note: The bucket containing the source file and the destination bucket must belong to the same account.
        /// </summary>
        public string SourceBucketId { get; set; }

        /// <summary>
        /// The name of the new file being created.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string FileName
        {
            get { return _FileName.ToPath(); }
            private set { _FileName = new Uri(value, UriKind.RelativeOrAbsolute); }
        }
        private Uri _FileName;

        /// <summary>
        /// A standard byte-range request to copy. If not provided the whole source file will be copied. 
        /// </summary>
        public RangeHeaderValue Range { get; set; }

        /// <summary>
        /// The strategy for how to populate metadata for the new file. If <see cref="Directive.Copy"/> is the
        /// indicated strategy then supplying the <see cref="ContentType"/> or <see cref="FileInfo"/> param is an error.
        /// </summary>
        public Directive MetadataDirective { get; set; }

        /// <summary>
        /// Must only be supplied if the <see cref="MetadataDirective"/> is <see cref="Directive.Replace"/>.
        /// The MIME type of the content of the file, which will be returned in the Content-Type header when
        /// downloading the file. Use the Content-Type b2/x-auto to automatically set the stored Content-Type
        /// post upload. In the case where a file extension is absent or the lookup fails, the Content-Type is
        /// set to application/octet-stream.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// The custom information that will be uploaded with the file. Up to 10 of these headers may be present.
        /// </summary>
        /// <remarks>
        /// The * part of the header name is replaced with the name of a custom field in the file information stored with the file
        /// and the value is an arbitrary UTF-8 string, percent-encoded. The same info headers sent with the upload will be returned
        /// with the download. The header name is case insensitive.
        /// </remarks>
        public FileInfo FileInfo { get; set; } = new FileInfo();

        /// <summary>
        /// Debugger display for this object.
        /// </summary>
        [JsonIgnore]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{{{nameof(SourceFileId)}: {SourceFileId}, {nameof(FileName)}: {FileName}}}"; }
        }
    }
}
