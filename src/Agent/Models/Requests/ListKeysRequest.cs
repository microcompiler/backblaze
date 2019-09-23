using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains information to create a list keys request.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class ListKeysRequest : IRequest
    {
        /// <summary>
        /// Represents the default number of keys per transaction.
        /// </summary>
        public const int DefaultKeysPerTransaction = 100;

        /// <summary>
        /// Maximum number of keys per transaction.
        /// </summary>
        public const int MaximumKeysPerTransaction = 10000;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListKeysRequest"/> class.
        /// </summary>
        /// <param name="accountId">The id of your account.</param>
        public ListKeysRequest(string accountId)
        {
            // Validate required arguments
            if (string.IsNullOrWhiteSpace(accountId))
                throw new ArgumentException("Argument can not be null, empty, or consist only of white-space characters.", nameof(accountId));

            // Initialize and set required properties
            AccountId = accountId;
        }

        /// <summary>
        /// The account id.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string AccountId { get; set; }

        /// <summary>
        /// The maximum number of files to return from this call. 
        /// The default value is <see cref="DefaultKeysPerTransaction"/> and the maximum allowed is <see cref="MaximumKeysPerTransaction"/>.
        /// Passing in 0 means to use the default of <see cref="DefaultKeysPerTransaction"/>. NOTE: <see cref="ListKeysRequest"/> is a Class C transaction. 
        /// If you set <see cref="MaxKeyCount"/> to more than 1000 and more than 1000 are returned, the call will be billed as multiple transactions,
        /// as if you had made requests in a loop asking for 1000 at a time. For example: if you set <see cref="MaxKeyCount"/> to 10000 and 3123 items are returned,
        /// you will be billed for 4 Class C transactions. 
        /// </summary>
        public long MaxKeyCount
        {
            get { return _maxKeyCount; }
            set
            {
                if (value < 0 || value > MaximumKeysPerTransaction)
                    throw new ArgumentException($"Property '{nameof(MaxKeyCount)}' must be a minimum of 1 and a maximum of {MaximumKeysPerTransaction} of files to return.");
                else
                    _maxKeyCount = value;
            }
        }
        private long _maxKeyCount = DefaultKeysPerTransaction;

        /// <summary>
        /// The first key to return. Used when a query hits the maxKeyCount, and you want to get more. Set to the value returned
        /// as the nextApplicationKeyId in the previous query. 
        /// </summary>
        public string StartApplicationKeyId { get; set; }

        ///	<summary>
        ///	Debugger display for this object.
        ///	</summary>
        [JsonIgnore]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{{{nameof(AccountId)}: {AccountId}}}"; }
        }

        /// <summary>
        /// Returns a hash code for this object.
        /// </summary>
        public override int GetHashCode()
        {
            var hashCode = 743484779;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(AccountId);
            hashCode = hashCode * -1521134295 + EqualityComparer<long?>.Default.GetHashCode(MaxKeyCount);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(StartApplicationKeyId);
            return hashCode;
        }
    }
}
