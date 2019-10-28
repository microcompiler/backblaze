using System;
using System.Diagnostics;

using Newtonsoft.Json;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains information to create a <see cref="DeleteBucketRequest"/>.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class DeleteBucketRequest : IRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteBucketRequest"/> class.
        /// </summary>
        /// <param name="accountId">The account id.</param>
        /// <param name="bucketId">The buckete id to delete.</param>
        public DeleteBucketRequest(string accountId, string bucketId)
        {
            // Validate required arguments
            if (string.IsNullOrWhiteSpace(accountId))
                throw new ArgumentException("Argument can not be null, empty, or consist only of white-space characters.", nameof(accountId));

            if (string.IsNullOrWhiteSpace(bucketId))
                throw new ArgumentException("Argument can not be null, empty, or consist only of white-space characters.", nameof(bucketId));

            // Initialize and set required properties
            AccountId = accountId;
            BucketId = bucketId;
        }

        /// <summary>
        /// The account id.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string AccountId { get; private set; }

        /// <summary>
        /// The id of the bucket to delete.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string BucketId { get; private set; }

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
