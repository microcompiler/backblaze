using System;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Bytewizer.Backblaze.Extensions;

using Newtonsoft.Json;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains information to create a bucket request.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class CreateBucketRequest : IRequest
    {
        /// <summary>
        /// Minimum number of characters in bucket name.
        /// </summary>
        public const int MinimumBucketNameLength = 6;

        /// <summary>
        /// Maximum number of characters in bucket name.
        /// </summary>
        public const int MaximumBucketNameLength = 50;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateBucketRequest"/> class.
        /// </summary>
        /// <param name="accountId">The account id.</param>
        /// <param name="bucketName">The name to give the new bucket. Bucket names must be a minimum of <see cref="MinimumBucketNameLength"/> and a maximum of <see cref="MaximumBucketNameLength"/> characters long, and must be globally unique.</param>
        /// <param name="bucketType">The bucket secuirty authorization type.</param>
        public CreateBucketRequest(string accountId, string bucketName, BucketType bucketType)
        {
            // Validate required arguments
            if (string.IsNullOrWhiteSpace(accountId))
                throw new ArgumentException("Argument can not be null, empty, or consist only of white-space characters.", nameof(accountId));

            if (string.IsNullOrWhiteSpace(bucketName))
                throw new ArgumentException("Argument can not be null, empty, or consist only of white-space characters.", nameof(bucketName));

            if (bucketName.Length < MinimumBucketNameLength || bucketName.Length > MaximumBucketNameLength)
                throw new ArgumentOutOfRangeException($"Argument must be a minimum of {MinimumBucketNameLength} and a maximum of {MaximumBucketNameLength} characters long.", nameof(bucketName));

            if (!Regex.IsMatch(bucketName, @"^([A-Za-z0-9\-]+)$"))
                throw new ArgumentOutOfRangeException("Argument can consist of only letters, digits, and dashs.", nameof(bucketName));

            if (bucketName.StartsWith("b2-"))
                throw new ArgumentException("Argument cannot start with 'b2-'. Reserved for internal Backblaze use.", nameof(bucketName));

            // Initialize and set required properties
            AccountId = accountId;
            BucketName = bucketName;
            BucketType = bucketType;
        }

        /// <summary>
        /// The account id.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string AccountId { get; private set; }

        /// <summary>
        /// The name to give the new bucket. Bucket names must be a minimum of 6 and a maximum of 50 characters long,
        /// and must be globally unique; two different B2 accounts cannot have buckets with the same name. Bucket names
        /// can consist of: letters, digits, and "-". Bucket names cannot start with "b2-" as these are reserved for
        /// internal Backblaze use. 
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string BucketName { get; private set; }

        /// <summary>
        /// The bucket secuirty authorization type. The <see cref="BucketType.AllPublic"/> indicates that files in this bucket can be downloaded by anybody
        /// or <see cref="BucketType.AllPrivate"/> requires that you need a bucket authorization token to download the files. 
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public BucketType BucketType { get; set; }

        /// <summary>
        /// User defined Cache-Control policy for buckets.  
        /// </summary>
        [JsonIgnore]
        public CacheControlHeaderValue CacheControl
        {
            get => BucketInfo.GetCacheControl();
            set => BucketInfo.SetCacheControl(value);
        }

        /// <summary>
        /// User defined information stored with the bucket limted to 10 items.
        /// </summary>
        public BucketInfo BucketInfo { get; set; } = new BucketInfo();

        /// <summary>
        /// Cors rules for this bucket limited to 100 rules. 
        /// </summary>
        public CorsRules CorsRules { get; set; } = new CorsRules();

        /// <summary>
        /// Lifecycle rules for this bucket limited to 100 rules. 
        /// </summary>
        public LifecycleRules LifecycleRules { get; set; } = new LifecycleRules();

        ///	<summary>
        ///	Debugger display for this object.
        ///	</summary>
        [JsonIgnore]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{{{nameof(AccountId)}: {AccountId}, {nameof(BucketName)}: {BucketName}, {nameof(BucketType)}: {BucketType}}}"; }
        }
    }
}
