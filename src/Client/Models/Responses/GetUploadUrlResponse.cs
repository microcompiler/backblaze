using System;
using System.Diagnostics;

using Newtonsoft.Json;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains the results of a <see cref="GetUploadUrlRequest"/> operation.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class GetUploadUrlResponse : IResponse
    {
        /// <summary>
        /// The unique id of the bucket.  
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string BucketId { get; internal set; }

        /// <summary>
        /// The url that can be used to upload parts of this file. see <see cref="UploadPartRequest"/>.  
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public Uri UploadUrl { get; internal set; }

        /// <summary>
        /// The authorization token that must be used when uploading files with this URL. This token is
        /// valid for 24 hours or until the upload url endpoint rejects an upload. 
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string AuthorizationToken { get; internal set; }

        ///	<summary>
        ///	Debugger display for this object.
        ///	</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{{{nameof(BucketId)}: {BucketId}, {nameof(UploadUrl)}: {UploadUrl.ToString()}, {nameof(AuthorizationToken)}: {AuthorizationToken}}}"; }
        }
    }
}
