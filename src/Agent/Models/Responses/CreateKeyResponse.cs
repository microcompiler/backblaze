using System.Diagnostics;

using Newtonsoft.Json;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains the results of a <see cref="CreateKeyRequest"/> operation.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class CreateKeyResponse : KeyItem, IResponse
    {   
        /// <summary>
        /// The secret part of the key. This is the only time it will be returned so you need to keep it. 
        /// This is not returned when you list the keys in your account. 
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string ApplicationKey { get; internal set; }

        ///	<summary>
        ///	Debugger display for this object.
        ///	</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{{{nameof(ApplicationKeyId)}: {ApplicationKeyId}, {nameof(KeyName)}: {KeyName}, {nameof(NamePrefix)}: {NamePrefix}, {nameof(ExpirationTimestamp)}: {ExpirationTimestamp}}}"; }
        }
    }
}
