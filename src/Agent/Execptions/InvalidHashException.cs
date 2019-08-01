using System;

namespace Bytewizer.Backblaze.Client
{
    /// <summary>
    /// The exception that is thrown when a file hash is not valid.
    /// </summary>
    public class InvalidHashException : Exception
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public InvalidHashException()
        {
        }

        /// <summary>
        /// Initializes exception message and inner exception.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public InvalidHashException(string message)
            : base(message)
        {
        }
    }
}
