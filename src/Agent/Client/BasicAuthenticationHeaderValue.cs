using System.Text;
using System.Net.Http.Headers;

namespace System.Net.Http
{
    /// <summary>
    /// HTTP Basic Authentication authorization header.
    /// </summary>
    /// <seealso cref="AuthenticationHeaderValue" />
    public class BasicAuthenticationHeaderValue : AuthenticationHeaderValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasicAuthenticationHeaderValue"/> class.
        /// </summary>
        /// <param name="id">The identifier for the key.</param>
        /// <param name="key">The secret part of the key.</param>
        public BasicAuthenticationHeaderValue(string id, string key)
            : base("Basic", EncodeCredential(id, key))
        { }

        /// <summary>
        /// Encodes the credential.
        /// </summary>
        /// <param name="id">The identifier for the key.</param>
        /// <param name="key">The secret part of the key.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">userName</exception>
        public static string EncodeCredential(string id, string key)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));

            Encoding encoding = Encoding.UTF8;
            string credential = string.Format("{0}:{1}", id, key);

            return Convert.ToBase64String(encoding.GetBytes(credential));
        }
    }
}
