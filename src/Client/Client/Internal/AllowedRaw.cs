using System;
using System.Collections.Generic;
using System.Diagnostics;

using Newtonsoft.Json;

using Bytewizer.Backblaze.Models;

namespace Bytewizer.Backblaze.Client.Internal
{
    /// <summary>
    /// Represents information related to an allowed authorization.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    internal class AllowedRaw
    {
        /// <summary>
        /// Gets or sets a list of <see cref="Capability"/> allowed.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public List<string> Capabilities { get; set; }

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
            get { return $"{{{nameof(Capabilities)}: {string.Join(", ", Capabilities)}, {nameof(NamePrefix)}: {NamePrefix}}}"; }
        }

        /// <summary>
        /// Translates this <see cref="AllowedRaw"/> instance to an instance of <see cref="Allowed"/>,
        /// parsing capabilities into the <see cref="Allowed.Capabilities"/> and
        /// <see cref="Allowed.UnknownCapabilities"/> properties.
        /// </summary>
        /// <returns>An instance of <see cref="Allowed"/>.</returns>
        public Allowed ParseCapabilities()
        {
            Allowed parsed = new Allowed();

            parsed.BucketId = this.BucketId;
            parsed.BucketName = this.BucketName;
            parsed.NamePrefix = this.NamePrefix;

            foreach (string capabilityName in this.Capabilities)
            {
                if (Enum.TryParse<Capability>(capabilityName, out var parsedCapability))
                {
                    parsed.Capabilities.Add(parsedCapability);
                }
                else
                {
                    parsed.UnknownCapabilities ??= new List<string>();
                    parsed.UnknownCapabilities.Add(capabilityName);
                }
            }

            return parsed;
        }
    }
}
