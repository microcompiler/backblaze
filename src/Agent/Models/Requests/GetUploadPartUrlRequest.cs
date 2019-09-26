using System;
using System.Collections.Generic;
using System.Diagnostics;

using Newtonsoft.Json;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains information to create a get <see cref="GetUploadPartUrlRequest"/>.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class GetUploadPartUrlRequest : IEquatable<GetUploadPartUrlRequest>, IRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetUploadPartUrlRequest"/> class.
        /// </summary>
        /// <param name="fileId">The large file id whose parts you want to upload.</param>
        public GetUploadPartUrlRequest(string fileId)
        {
            // Validate required arguments
            if (string.IsNullOrWhiteSpace(fileId))
                throw new ArgumentException("Argument can not be null, empty, or consist only of white-space characters.", nameof(fileId));

            // Initialize and set required properties
            FileId = fileId;
        }

        /// <summary>
        /// The large file id whose parts you want to upload.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string FileId { get; private set; }

        ///	<summary>
        ///	Debugger display for this object.
        ///	</summary>
        [JsonIgnore]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{{{nameof(FileId)}: {FileId}}}"; }
        }

        #region IEquatable

        /// <summary>
        /// Determines whether this object is equal <paramref name="obj"/>.
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as GetUploadPartUrlRequest);
        }

        /// <summary>
        /// Determines whether this object is equal <paramref name="other"/>.
        /// </summary>
        public bool Equals(GetUploadPartUrlRequest other)
        {
            return other != null &&
                   FileId == other.FileId;
        }

        /// <summary>
        /// Provides hash code for the request.
        /// </summary>
        public override int GetHashCode()
        {
            return 2053723198 + EqualityComparer<string>.Default.GetHashCode(FileId);
        }

        /// <summary>
        /// Determines whether the given <paramref name="left"/> is equal <paramref name="right"/>.
        /// </summary>
        public static bool operator ==(GetUploadPartUrlRequest left, GetUploadPartUrlRequest right)
        {
            return EqualityComparer<GetUploadPartUrlRequest>.Default.Equals(left, right);
        }

        /// <summary>
        /// Determines whether the given <paramref name="left"/> is not equal <paramref name="right"/>.
        /// </summary>
        public static bool operator !=(GetUploadPartUrlRequest left, GetUploadPartUrlRequest right)
        {
            return !(left == right);
        }

        #endregion
    }
}
