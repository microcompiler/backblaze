using System.Diagnostics;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains information related to request failures. Any <see cref="Status"/> other than a 200 is responed to as an error.
    /// </summary>
   
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class ErrorResponse : IResponse
    {
        /// <summary>
        /// The numeric HTTP status code. Always matches the status in the HTTP response.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// A single-identifier code that identifies the error.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// A human-readable message indicating what went wrong.
        /// </summary>
        public string Message { get; set; }

        ///	<summary>
        ///	Debugger display for this object.
        ///	</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{{{nameof(Status)}: {Status}, {nameof(Code)}: {Code}, {nameof(Message)}: {Message}}}"; }
        }
    }
}
