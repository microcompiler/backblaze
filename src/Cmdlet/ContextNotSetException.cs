using System;
using System.Collections.Generic;
using System.Text;

namespace Bytewizer.Backblaze
{
    /// <summary>
    /// The exception thrown when an error occurs during client operations.
    /// </summary>
    public class ContextNotSetException : InvalidOperationException
    {
        private const string msg = "The context is not set! {0}.  Run the \"Connect-Instance\" cmdlet first.";

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextNotSetException"/> class.
        /// </summary>
        public ContextNotSetException()
            : base(string.Format(msg, string.Empty))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextNotSetException"/> class.
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// </summary>
        public ContextNotSetException(string message)
            : base(string.Format(msg, message))
        { }
    }
}
