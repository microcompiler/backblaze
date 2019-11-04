using System;

using Bytewizer.Backblaze.Models;

namespace Bytewizer.Backblaze.Client
{
    /// <summary>
    /// Represents a <see cref="AuthToken"/> returned from Backblaze B2 Cloud Storage.
    /// </summary>
    public class AuthToken
    {
        #region Lifetime

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthToken" /> class.
        /// </summary>
        public AuthToken(string authorizationToken)
        {
            Authorization = authorizationToken ?? throw new ArgumentNullException(nameof(authorizationToken));
        }

        #endregion

        #region Public Properties

        /// <summary>
		/// The token returned by the API that can be used to authorise further requests.
		/// </summary>
        public string Authorization { get; set; }

        /// <summary>
        /// An object <see cref="Models.Allowed"/> containing the capabilities of this auth token, and any restrictions on using it.
        /// </summary>
        public Allowed Allowed { get; set; }

        /// <summary>
        /// The number of seconds for which this token is valid from the time it was issued.
        /// </summary>
        public long ExpiresIn { get; set; } = 86400;

        /// <summary>
        /// Represents the date and time at which this token was issued.
        /// </summary>
        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Returns the date and time at which this token expires, calculated from <see cref="IssuedAt"/> and <see cref="ExpiresIn"/>.
        /// </summary>
        public DateTime ExpiresAt { get { return IssuedAt.AddSeconds(ExpiresIn); } }

        /// <summary>
        /// Returns true if the token has expired based on the current system clock and the calculated <see cref="ExpiresAt"/> value.
        /// </summary>
        public bool IsExpired() { return (DateTime.Now >= IssuedAt.AddSeconds(ExpiresIn)); }

        /// <summary>
        /// Returns true if the token will expired based on the current system clock and the calculated <see cref="ExpiresAt"/> value.
        /// </summary>
        public bool IsExpiring() { return (DateTime.Now >= IssuedAt.AddSeconds(ExpiresIn / 90d)); }

        /// <summary>
        /// The token in authorization header format.
        /// </summary>
        public override string ToString() => $"{Authorization}";

        #endregion
    }
}
