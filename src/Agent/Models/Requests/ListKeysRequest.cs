using System;
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
        public string AccountId { get; private set; }

        /// <summary>
        /// The maximum number of keys to return in the response. Default is 100, maximum is 10000. 
        /// </summary>
        /// <remarks>
        /// NOTE: b2_list_keys is a Class C transaction. The maximum number of keys returned per transaction is 1000. If you set
        /// maxKeyCount to more than 1000 and more than 1000 are returned, the call will be billed as multiple transactions,
        /// as if you had made requests in a loop asking for 1000 at a time. For example: if you set maxKeyCount to 10000 and
        /// 3123 keys are returned, you will be billed for 4 Class C transactions. 
        /// </remarks>
        public long? MaxKeyCount { get; set; }

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
    }
}
