using System;
using System.Collections.Generic;
using System.Diagnostics;

using Newtonsoft.Json;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains information to create a get an upload url request.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class GetUploadUrlRequest : IEquatable<GetUploadUrlRequest>, IRequest
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="bucketId">The bucket id you want to upload to.</param>
        public GetUploadUrlRequest(string bucketId)
        {
            // Validate required arguments
            if (string.IsNullOrWhiteSpace(bucketId))
                throw new ArgumentException("Argument can not be null, empty, or consist only of white-space characters.", nameof(bucketId));

            // Initialize and set required properties
            BucketId = bucketId;
        }

        /// <summary>
        /// The bucket id that you want to upload to.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string BucketId { get; private set; }

        ///	<summary>
        ///	Debugger display for this object.
        ///	</summary>
        [JsonIgnore]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{{{nameof(BucketId)}: {BucketId}}}"; }
        }

        #region IEquatable

        /// <summary>
        /// Determines whether this object is equal <paramref name="obj"/>.
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as GetUploadUrlRequest);
        }

        /// <summary>
        /// Determines whether this object is equal <paramref name="other"/>.
        /// </summary>
        public bool Equals(GetUploadUrlRequest other)
        {
            return other != null &&
                   BucketId == other.BucketId;
        }
        /// <summary>
        /// Provides hash code for the request.
        /// </summary>
        public override int GetHashCode()
        {
            return 731021066 + EqualityComparer<string>.Default.GetHashCode(BucketId);
        }

        /// <summary>
        /// Determines whether the given <paramref name="left"/> is equal <paramref name="right"/>.
        /// </summary>
        public static bool operator ==(GetUploadUrlRequest left, GetUploadUrlRequest right)
        {
            return EqualityComparer<GetUploadUrlRequest>.Default.Equals(left, right);
        }

        /// <summary>
        /// Determines whether the given <paramref name="left"/> is not equal <paramref name="right"/>.
        /// </summary>
        public static bool operator !=(GetUploadUrlRequest left, GetUploadUrlRequest right)
        {
            return !(left == right);
        }

        #endregion
    }
}
