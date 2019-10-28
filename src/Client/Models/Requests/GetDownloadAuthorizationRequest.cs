using System;
using System.Diagnostics;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains information to create a get <see cref="GetDownloadAuthorizationRequest"/>.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class GetDownloadAuthorizationRequest : IRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetDownloadAuthorizationRequest"/> class.
        /// </summary>
        /// <param name="bucketId">The buckete id the download authorization token will allow access.</param>
        /// <param name="fileNamePrefix">The file name prefix of files the download authorization token will allow access.</param>
        /// <param name="validDurationInSeconds">The number of seconds before the authorization token will expire.</param>
        public GetDownloadAuthorizationRequest(string bucketId, string fileNamePrefix, long validDurationInSeconds = 3600)
        {
            // Validate required arguments
            if (string.IsNullOrWhiteSpace(bucketId))
                throw new ArgumentException("Argument can not be null, empty, or consist only of white-space characters.", nameof(bucketId));

            if (string.IsNullOrWhiteSpace(fileNamePrefix))
                throw new ArgumentException("Argument can not be null, empty, or consist only of white-space characters.", nameof(fileNamePrefix));

            if (validDurationInSeconds < 1 || validDurationInSeconds > 604800)
                throw new ArgumentOutOfRangeException($"Argument must be a minimum of 1 second and a maximum of 604800 seconds (1000 days)", nameof(validDurationInSeconds));

            // Initialize and set required properties
            BucketId = bucketId;
            FileNamePrefix = fileNamePrefix;
            ValidDurationInSeconds = validDurationInSeconds;
        }

        /// <summary>
        /// The bucket id.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string BucketId { get; private set; }

        /// <summary>
        /// The file name prefix of files the download authorization token will allow to access.
        /// </summary>
        /// <remarks>
        /// For example, if you have a private bucket named "photos" and generate a download authorization token for the
        /// fileNamePrefix "pets/" you will be able to use the download authorization token 
        /// to access: https://f345.backblazeb2.com/file/photos/pets/kitten.jpg 
        /// but not: https://f345.backblazeb2.com/file/photos/vacation.jpg. 
        /// </remarks>
        [JsonProperty(Required = Required.Always)]
        public string FileNamePrefix { get; private set; }

        /// <summary>
        /// The number of seconds before the authorization token will expire. The minimum value is 1 second.
        /// The maximum value is 604800 which is one week in seconds. 
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public long ValidDurationInSeconds { get; private set; } = 3600;

        /// <summary>
        /// If this is present B2 will use it as the value of the Content-Disposition header when the file is downloaded 
        /// unless it's overridden by a value given in the download request. Parameter continuations (contain an '*') are not supported.
        /// </summary>
        /// <remarks>
        /// Note that this file info will not be included in downloads as a x-bz-info-b2-content-disposition header. 
        /// Instead, it (or the value specified in a request) will be in the Content-Disposition
        /// </remarks>
        [JsonIgnore]
        public ContentDispositionHeaderValue ContentDisposition
        {
            get { return ContentDispositionHeaderValue.Parse(B2ContentDisposition); }
            set { B2ContentDisposition = value.ToString(); }
        }

        [JsonProperty]
        private string B2ContentDisposition { get; set; }


        ///	<summary>
        ///	Debugger display for this object.
        ///	</summary>
        [JsonIgnore]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{{{nameof(BucketId)}: {BucketId}}}"; }
        }
    }
}
