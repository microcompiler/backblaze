using System;

namespace Bytewizer.Backblaze
{
    /// <summary>
    /// The exception that is thrown when a cap is exceeded or an account in bad standing.
    /// </summary>
    public class CapExceededExecption : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CapExceededExecption"/> class.
        /// </summary>
        public CapExceededExecption()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CapExceededExecption"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public CapExceededExecption(string message)
            : base(message)
        { }
    }
}
