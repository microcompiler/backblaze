using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Newtonsoft.Json;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains information to create a application key request.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class CreateKeyRequest : IRequest
    {
        /// <summary>
        /// Minimum numbers of characters in key name.
        /// </summary>
        public static readonly int MinimumKeyNameLength = 1;

        /// <summary>
        /// Maximum number of characters in key name.
        /// </summary>
        public static readonly int MaximumKeyNameLength = 100;

        /// <summary>
        /// Minimum number of seconds key will expire.
        /// </summary>
        public static readonly int MinimumDurationInSeconds = 1;

        /// <summary>
        /// Maximum number of seconds (1000 days in seconds) key will expire.
        /// </summary>
        public static readonly int MaximumDurationInSeconds = 86400000;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateKeyRequest"/> class.
        /// </summary>
        /// <param name="accountId">The account id.</param>
        /// <param name="keyName">The name for this key.</param>
        /// <param name="capabilities">A list of <see cref="Capability"/> each one naming a capability the new key should have.</param>
        public CreateKeyRequest(string accountId, string keyName, Capabilities capabilities)
        {
            // Validate required arguments
            if (string.IsNullOrWhiteSpace(accountId))
                throw new ArgumentException("Argument can not be null, empty, or consist only of white-space characters.", nameof(accountId));

            if (capabilities.Count <= 0)
                throw new ArgumentException("Argument must containe at least one capabilitiy.", nameof(capabilities));

            if (string.IsNullOrWhiteSpace(keyName))
                throw new ArgumentException("Argument can not be null, empty, or consist only of white-space characters.", nameof(keyName));

            if (keyName.Length < MinimumKeyNameLength || keyName.Length > MaximumKeyNameLength)
                throw new ArgumentOutOfRangeException($"Argument must be a minimum of {MinimumKeyNameLength} and a maximum of {MaximumKeyNameLength} characters long.", nameof(keyName));

            if (!Regex.IsMatch(keyName, @"^([A-Za-z0-9\-]+)$"))
                throw new ArgumentOutOfRangeException("Argument can consist of only letters, digits, and dashs.", nameof(keyName));

            // Initialize and set required properties
            AccountId = accountId;
            KeyName = keyName;
            Capabilities = capabilities;
        }

        /// <summary>
        /// The account id.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string AccountId { get; private set; }

        /// <summary>
        /// A list of strings, each one naming a capability the new key should have. Possibilities are: listKeys, writeKeys,
        /// deleteKeys, listBuckets, writeBuckets, deleteBuckets, listFiles, readFiles, shareFiles, writeFiles, and deleteFiles. 
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public Capabilities Capabilities { get; private set; }

        /// <summary>
        /// The name for this key. There is no requirement that the name be unique.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string KeyName { get; private set; }

        /// <summary>
        /// When provided the key will expire after the given number of seconds and will have expiration timestamp set. Value must be 
        /// a positive integer and must be less than 1000 days (in seconds). 
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public long ValidDurationInSeconds
        {
            get { return _validDurationInSeconds; }
            set
            {
                if (value > MinimumDurationInSeconds || value < MaximumDurationInSeconds)
                    _validDurationInSeconds = value;
                else
                    throw new ArgumentOutOfRangeException($"Argument must be a minimum of {MinimumDurationInSeconds} and a maximum of {MaximumDurationInSeconds} duration in seconds.", nameof(ValidDurationInSeconds));
            }
        }
        private long _validDurationInSeconds;

        /// <summary>
        /// When present the new key can only access this bucket. When set only these capabilities can be specified: listBuckets, listFiles,
        /// readFiles, shareFiles, writeFiles, and deleteFiles. 
        /// </summary>
        public string BucketId { get; set; }

        /// <summary>
        /// When present restricts access to files whose names start with the prefix. You must set <see cref="BucketId"/> when setting this property. 
        /// </summary>
        public string NamePrefix { get; set; }

        ///	<summary>
        ///	Debugger display for this object.
        ///	</summary>
        [JsonIgnore]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{{{nameof(AccountId)}: {AccountId}, {nameof(KeyName)}: {KeyName}}}"; }
        }
    }
}
