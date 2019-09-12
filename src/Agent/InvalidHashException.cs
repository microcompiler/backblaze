using System;
using System.Runtime.Serialization;

namespace Bytewizer.Backblaze.Client
{
    /// <summary>
    /// The exception that is thrown when a validation hash is not valid.
    /// </summary>
    [Serializable]
    public class InvalidHashException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidHashException"/> class.
        /// </summary>
        public InvalidHashException()
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidHashException"/> class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public InvalidHashException(string message)
            : base(message)
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidHashException"/> class
        /// with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException" /> parameter
        /// is not a null reference, the current exception is raised in a catch block that handles the inner exception. </param>
        public InvalidHashException(string message, Exception innerException)
            : base(message, innerException)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializationInfo"/> class
        /// with serialized data.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data. </param>
        /// <param name="context">The contextual information about the source or destination. </param>
        protected InvalidHashException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
