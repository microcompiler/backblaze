using System.Collections.Generic;
using System.Diagnostics;

using Newtonsoft.Json;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Represents information related to an allowed authorization.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class Allowed
    {
        /// <summary>
        /// Gets or sets a list of <see cref="Capability"/> allowed.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public Capabilities Capabilities { get; set; }

        /// <summary>
        /// Gets or sets a list of capabilities that couldn't be parsed into <see cref="Capability"/> values.
        /// </summary>
        public List<string> UnknownCapabilities { get; set; }

        /// <summary>
        /// Gets or sets restricted access only to this bucket id.
        /// </summary>
        public string BucketId { get; set; }

        /// <summary>
        /// When bucket id is set and it is a valid bucket that has not been deleted this field is set to the name of the
        /// bucket. It's possible that bucket id is set to a bucket that no longer exists in which case this field will be
        /// <c>null</c>. It's also null when bucket id is <c>null</c>.
        /// </summary>
        public string BucketName { get; set; }

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
            get
            {
                string unknownCapabilitiesString = "";

                if ((this.UnknownCapabilities != null) && (this.UnknownCapabilities.Count > 0))
                {
                    unknownCapabilitiesString = " (+ " + string.Join(", ", UnknownCapabilities) + ")";
                }

                return $"{{{nameof(Capabilities)}: {string.Join(", ", Capabilities)}{unknownCapabilitiesString}, {nameof(NamePrefix)}: {NamePrefix}}}";
            }
        }
    }
}
