using System.Diagnostics;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains response information related to an allowed authorization.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class Allowed
    {
        /// <summary>
        /// A list of capabilities the key has. 
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public List<Capability> Capabilities { get; set; }

        /// <summary>
        /// When present access is restricted to one bucket.
        /// </summary>
        public string BucketId { get; set; }

        /// <summary>
        /// When bucket id is set and it is a valid bucket that has not been deleted this field is set to the name of the
        /// bucket. It's possible that bucket id is set to a bucket that no longer exists in which case this field will be
        /// null. It's also null when bucket id is null.
        /// </summary>
        public string BucketName { get; set; }

        /// <summary>
        /// When present access is restricted to files whose names start with the prefix.
        /// </summary>
        public string NamePrefix { get; set; }

        ///	<summary>
        ///	Debugger display for this object.
        ///	</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{{{nameof(Capabilities)}: {string.Join(", ", Capabilities)}, {nameof(NamePrefix)}: {NamePrefix}}}"; }
        }
    }
}
