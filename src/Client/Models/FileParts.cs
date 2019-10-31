using System.Net.Http.Headers;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Represents information related to a <see cref="FileParts"/>.
    /// </summary>
    public class FileParts
    {
        /// <summary>
        /// Gets or sets the part number.  
        /// </summary>
        public int PartNumber { get; set; }

        /// <summary>
        /// Gets or sets the start position of the file part.
        /// </summary>
        public long Position { get; set; }

        /// <summary>
        /// Gets or sets the legnth of the file part.
        /// </summary>
        public long Length { get; set; }

        /// <summary>
        /// Gets or sets a range header value of the file part.
        /// </summary>
        public RangeHeaderValue RangeHeader { get; set; }
    }
}
