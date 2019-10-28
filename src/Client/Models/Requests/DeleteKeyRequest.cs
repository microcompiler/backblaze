using System;
using System.Diagnostics;

using Newtonsoft.Json;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains information to create a delete a key request.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class DeleteKeyRequest : IRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteKeyRequest"/> class.
        /// </summary>
        /// <param name="applicationKeyId">The application key id to delete.</param>
        public DeleteKeyRequest(string applicationKeyId)
        {
            // Validate required arguments
            if (string.IsNullOrWhiteSpace(applicationKeyId))
                throw new ArgumentException("Argument can not be null, empty, or consist only of white-space characters.", nameof(applicationKeyId));
            
            // Initialize and set required properties
            ApplicationKeyId = applicationKeyId;
        }

        /// <summary>
        /// The application key id to delete.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string ApplicationKeyId { get; private set; }

        ///	<summary>
        ///	Debugger display for this object.
        ///	</summary>
        [JsonIgnore]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{{{nameof(ApplicationKeyId)}: {ApplicationKeyId}}}"; }
        }
    }
}
