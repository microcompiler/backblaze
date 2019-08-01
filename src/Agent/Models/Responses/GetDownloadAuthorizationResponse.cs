using System.Diagnostics;

using Newtonsoft.Json;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains the results of a <see cref="GetDownloadAuthorizationRequest"/> operation.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class GetDownloadAuthorizationResponse : IResponse
    {
        /// <summary>
        /// The unique id of the bucket.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string BucketId { get; internal set; }

        /// <summary>
        /// The prefix for files the authorization token will allow <see cref="DownloadFileByNameRequest"/> to access.  
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string FileNamePrefix { get; internal set; }

        /// <summary>
        /// The authorization token that can be passed in the Authorization header or as an Authorization 
        /// parameter to <see cref="DownloadFileByNameRequest"/> to access files beginning with the file name prefix. 
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string AuthorizationToken { get; internal set; }

        ///	<summary>
        ///	Debugger display for this object.
        ///	</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{{{nameof(BucketId)}: {BucketId}, {nameof(FileNamePrefix)}: {FileNamePrefix}, {nameof(AuthorizationToken)}: {AuthorizationToken}}}"; }
        }
    }
}
