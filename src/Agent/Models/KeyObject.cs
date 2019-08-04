using System;
using System.Diagnostics;

using Newtonsoft.Json;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains response information related to a key.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class KeyObject
    {
        /// <summary>
        /// The name assigned when the key was created.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string KeyName { get; internal set; }

        /// <summary>
        /// The application key id.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string ApplicationKeyId { get; internal set; }

        /// <summary>
        /// A list of strings, each one naming a capability the key has. Possibilities are: listKeys, writeKeys, 
        /// deleteKeys, listBuckets, writeBuckets, deleteBuckets, listFiles, readFiles, shareFiles, writeFiles, and deleteFiles. 
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string[] Capabilities { get; internal set; }

        /// <summary>
        /// The account id that this application key is for.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string AccountId { get; internal set; }

        /// <summary>
        /// When present indicates when this key will expire.
        /// </summary>  
        public DateTime ExpirationTimestamp { get; internal set; }

        /// <summary>
        /// Restricts access to this bucket.
        /// </summary>
        public string BucketId { get; internal set; }

        /// <summary>
        /// Restricts access to files whose names start with the prefix.
        /// </summary>
        public string NamePrefix { get; internal set; }

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
