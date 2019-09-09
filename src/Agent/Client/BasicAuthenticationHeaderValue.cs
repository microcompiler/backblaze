using System.Text;
using System.Net.Http.Headers;

namespace System.Net.Http
{
    /// <summary>
    /// Represents the <see cref="BasicAuthenticationHeaderValue"/> header.
    /// </summary>
    public class BasicAuthenticationHeaderValue : AuthenticationHeaderValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasicAuthenticationHeaderValue"/> class.
        /// </summary>
        /// <param name="keyId">The identifier for the key.</param>
        /// <param name="applicationKey">The secret part of the key. You can use either the master application key or a normal application key.</param>
        public BasicAuthenticationHeaderValue(string keyId, string applicationKey)
            : base("Basic", EncodeCredential(keyId, applicationKey))
        { }

        /// <summary>
        /// Encodes the credential.
        /// </summary>
        /// <param name="keyId">The identifier for the key.</param>
        /// <param name="applicationKey">The secret part of the key. You can use either the master application key or a normal application key.</param>
        public static string EncodeCredential(string keyId, string applicationKey)
        {
            if (string.IsNullOrWhiteSpace(keyId)) throw new ArgumentNullException(nameof(keyId));
            if (string.IsNullOrWhiteSpace(applicationKey)) throw new ArgumentNullException(nameof(applicationKey));

            Encoding encoding = Encoding.UTF8;
            string credential = string.Format("{0}:{1}", keyId, applicationKey);

            return Convert.ToBase64String(encoding.GetBytes(credential));
        }
    }
}
