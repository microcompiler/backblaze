using System;
using System.Diagnostics;

using Newtonsoft.Json;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains the results of authorize account request operation.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class AuthorizeAccountResponse : IResponse
    {
        /// <summary>
        /// The identifier for the account.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string AccountId { get; internal set; }

        /// <summary>
        /// An authorization token to use with all calls other than authorize account that need an authorization header.
        /// This authorization token is valid for at most 24 hours.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string AuthorizationToken { get; internal set; }

        /// <summary>
        /// The capabilities of this auth token and any restrictions on using it.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public Allowed Allowed { get; internal set; }

        /// <summary>
        /// The base url to use for all API calls except for uploading and downloading files.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public Uri ApiUrl { get; internal set; }

        /// <summary>
        /// The base url to use for downloading files.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public Uri DownloadUrl { get; internal set; }

        /// <summary>
        /// The recommended size for each part of a large file. We recommend using this part size for optimal upload performance.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public long RecommendedPartSize { get; internal set; }

        /// <summary>
        /// The smallest possible size of a part of a large file (except the last one). This is smaller than the <see cref="RecommendedPartSize"/>. 
        /// If you use it you may find that it takes longer overall to upload a large file.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public long AbsoluteMinimumPartSize { get; internal set; }

        /// <summary>
        /// OBSOLETE: This field will always have the same value as <see cref="RecommendedPartSize"/>.
        /// </summary>
        [Obsolete("This field will always have the same value as 'RecommendedPartSize'.")]
        public long MinimumPartSize { get; internal set; }

        ///	<summary>
        ///	Debugger display for this object.
        ///	</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{{{nameof(AccountId)}: {AccountId}, {nameof(AuthorizationToken)}: {AuthorizationToken}}}"; }
        }
    }
}
