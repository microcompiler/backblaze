using System;
using System.Diagnostics;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains information to updated bucket request.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class UpdateBucketRequest : IRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateBucketRequest"/> class.
        /// </summary>
        /// <param name="accountId">The account id that the bucket is in.</param>
        /// <param name="bucketId">The buckete id to update.</param>
        /// <param name="bucketType">The bucket secuirty authorization type.</param>
        public UpdateBucketRequest(string accountId, string bucketId, BucketType bucketType)
        {
            // Validate required arguments
            if (string.IsNullOrWhiteSpace(accountId))
                throw new ArgumentException("Argument can not be null, empty, or consist only of white-space characters.", nameof(accountId));

            if (string.IsNullOrWhiteSpace(bucketId))
                throw new ArgumentException("Argument can not be null, empty, or consist only of white-space characters.", nameof(bucketId));

            // Initialize and set required properties
            AccountId = accountId;
            BucketId = bucketId;
            BucketType = bucketType;
        }

        /// <summary>
        /// The account id that the bucket is in.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string AccountId { get; private set; }

        /// <summary>
        /// The unique id of the bucket.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string BucketId { get; private set; }

        /// <summary>
        /// The bucket secuirty authorization type. The <see cref="BucketType.AllPublic"/> indicates that files in this bucket can be downloaded by anybody
        /// or <see cref="BucketType.AllPrivate"/> requires that you need a bucket authorization token to download the files. 
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public BucketType BucketType { get; private set; }

        /// <summary>
        /// User defined information stored with the bucket limited to 10 items.
        /// </summary>
        public BucketInfo BucketInfo { get; set; } = new BucketInfo();

        /// <summary>
        /// Cors rules for this bucket limited to 100 rules. 
        /// </summary>
        public LifecycleRules CorsRules { get; set; } = new LifecycleRules();

        /// <summary>
        /// Lifecycle rules for this bucket limited to 100 rules. 
        /// </summary>
        public LifecycleRules LifecycleRules { get; set; } = new LifecycleRules();

        /// <summary>
        /// When set the update will only happen if the revision number stored in the B2 service matches the one passed in. This can be used
        /// to avoid having simultaneous updates make conflicting changes. 
        /// </summary>
        public string IfRevisionIs { get; set; }

        ///	<summary>
        ///	Debugger display for this object.
        ///	</summary>
        [JsonIgnore]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{{{nameof(AccountId)}: {AccountId}, {nameof(BucketId)}: {BucketId}}}"; }
        }
    }
}
