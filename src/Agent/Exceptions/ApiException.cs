using System;
using System.Net;
using System.Runtime.Serialization;

using Bytewizer.Backblaze.Models;

namespace Bytewizer.Backblaze
{
    /// <summary>
    /// The exception thrown when an error occurs during client operations.
    /// </summary>
    [Serializable]
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
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiException"/> class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public ApiException(string message)
            : base(message)
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiException"/> class
        /// with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException" /> parameter
        /// is not a null reference, the current exception is raised in a catch block that handles the inner exception. </param>
        public ApiException(string message, Exception innerException)
            : base(message, innerException)
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiException"/> class
        /// with a specified http status code.
        /// </summary>
        /// <param name="statusCode">The http status code.</param>
        public ApiException(HttpStatusCode statusCode)
            : base($"Backblaze B2 Cloud Storage.ervice responded with the following: httpcode={statusCode.ToString().ToLower()}")
        {
            StatusCode = statusCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiException"/> class
        /// with a specified http status code and error message.
        /// </summary>
        /// <param name="statusCode">The http status code.</param>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public ApiException(HttpStatusCode statusCode, string message)
            : base($"Backblaze API server responded with the following: httpcode={statusCode.ToString().ToLower()}, message:{message}")
        {
            StatusCode = statusCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiException"/> class 
        /// with a specified http status code and error message.
        /// </summary>
        /// <param name="statusCode">The http status code.</param>
        /// <param name="error">The server error that explains the reason for the exception.</param>
        public ApiException(HttpStatusCode statusCode, ErrorResponse error)
            : base($"Backblaze API server responded with the following error: httpcode={statusCode.ToString().ToLower()}, status={error.Status.ToLower()}, code={error.Code.ToLower()}, message={error.Message.ToLower()}")
        {
            StatusCode = statusCode;
            Error = error;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiException"/> class
        /// with serialized data.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data. </param>
        /// <param name="context">The contextual information about the source or destination. </param>
        protected ApiException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {}
    }
}
