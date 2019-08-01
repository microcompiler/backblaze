using System;
using System.Net;

using Bytewizer.Backblaze.Models;

namespace Bytewizer.Backblaze
{
    /// <summary>
    /// Represents base exception class for Backblaze client errors.
    /// </summary>
    public class ApiException : Exception
    {
        /// <summary>
        /// Get the http response status code.
        /// </summary>
        public HttpStatusCode StatusCode { get; private set; }

        /// <summary>
        /// Get error information related to request failures.
        /// </summary>
        public ErrorResponse Error { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiException"/> class.
        /// </summary>
        public ApiException()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public ApiException(string message)
            : base(message)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception. </param>
        /// <param name="inner">The exception that is the cause of the current exception. If the <paramref name="inner" /> parameter
        /// is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public ApiException(string message, Exception inner)
            : base(message, inner)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiException"/> class with a specified http status code.
        /// </summary>
        /// <param name="statusCode">The http status code.</param>
        public ApiException(HttpStatusCode statusCode)
            : base($"Backblaze API server responded with the following: httpcode={statusCode.ToString().ToLower()}")
        {
            StatusCode = statusCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiException"/> class with a specified http status code and error message.
        /// </summary>
        /// <param name="statusCode">The http status code.</param>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public ApiException(HttpStatusCode statusCode, string message)
            : base($"Backblaze API server responded with the following: httpcode={statusCode.ToString().ToLower()}, message:{message}")
        {
            StatusCode = statusCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiException"/> class with a specified http status code and error message.
        /// </summary>
        /// <param name="statusCode">The http status code.</param>
        /// <param name="error">The server error that explains the reason for the exception.</param>
        public ApiException(HttpStatusCode statusCode, ErrorResponse error)
            : base($"Backblaze API server responded with the following error: httpcode={statusCode.ToString().ToLower()}, status={error.Status.ToLower()}, code={error.Code.ToLower()}, message={error.Message.ToLower()}")
        {
            StatusCode = statusCode;
            Error = error;
        }
    }
}
