using System.Net;

using Bytewizer.Backblaze.Models;

namespace Bytewizer.Backblaze
{
    /// <summary>
    /// The exception that is thrown when an authentication attempt is not valid.
    /// </summary>
    public class AuthException : ApiException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthException"/> class.
        /// </summary>
        public AuthException()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public AuthException(string message)
            : base(message)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthException"/> class with a specified http status code.
        /// </summary>
        /// <param name="statusCode">The http status code.</param>
        public AuthException(HttpStatusCode statusCode)
            : base(statusCode)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthException"/> class with a specified http status code and error message.
        /// </summary>
        /// <param name="statusCode">The http status code.</param>
        /// <param name="error">The server error that explains the reason for the exception.</param>
        public AuthException(HttpStatusCode statusCode, ErrorResponse error)
            : base (statusCode, error)
        { }
    }
}
