using System;
using System.Diagnostics;

using Newtonsoft.Json;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Represents information related to a <see cref="KeyItem"/>.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class KeyItem : IItem
    {
        /// <summary>
        /// Gets or sets the key name.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string KeyName { get; set; }

        /// <summary>
        /// Gets or sets the application key id.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string ApplicationKeyId { get; set; }

        /// <summary>
        /// Gets or sets a list of <see cref="Capability"/> associated with the key. 
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public Capabilities Capabilities { get; set; }

        /// <summary>
        /// Gets or sets the account id this application key associated with.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string AccountId { get; set; }

        /// <summary>
        /// Gets or sets when this key will expire.
        /// </summary>  
        public DateTime ExpirationTimestamp { get; set; }

        /// <summary>
        /// Gets or sets restricted access only to this bucket id.
        /// </summary>
        public string BucketId { get; set; }

        /// <summary>
        /// Gets or sets restricted access to files whose names start with the prefix.
        /// </summary>
        public string NamePrefix { get; set; }

        /// <summary>
        /// Debugger display for this object.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{{{nameof(KeyName)}: {KeyName}, {nameof(ExpirationTimestamp)}: {ExpirationTimestamp}, {nameof(NamePrefix)}: {NamePrefix}}}"; }
        }
    }
}
