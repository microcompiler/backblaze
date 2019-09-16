using System;
using System.Collections.Generic;
using System.Diagnostics;

using Newtonsoft.Json;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains information to create a list unfinished large file request.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class ListUnfinishedLargeFilesRequest : IRequest, IEquatable<ListUnfinishedLargeFilesRequest>
    {
        /// <summary>
        /// Represents the default number of file names per transaction.
        /// </summary>
        public const int DefaultNamesPerTransaction = 100;

        /// <summary>
        /// Maximum number of file names per transaction.
        /// </summary>
        public const int MaximumNamesPerTransaction = 1000;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListUnfinishedLargeFilesRequest"/> class.
        /// </summary>
        /// <param name="bucketId">The bucket id to look for unfinished file names in.</param>
        public ListUnfinishedLargeFilesRequest(string bucketId)
        {
            // Validate required arguments
            if (string.IsNullOrWhiteSpace(bucketId))
                throw new ArgumentException("Argument can not be null, empty, or consist only of white-space characters.", nameof(bucketId));
            
            // Initialize and set required properties
            BucketId = bucketId;
            MaxFileCount = DefaultNamesPerTransaction;
        }

        /// <summary>
        /// The bucket id to look for unfinished file names in.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string BucketId { get; private set; }

        /// <summary>
        /// When a namePrefix is provided, only files whose names match the prefix will be returned. Whe using an application key 
        /// that is restricted to a name prefix, you must provide a prefix here that is at least as restrictive. 
        /// </summary>
        public string NamePrefix { get; set; }

        /// <summary>
        /// The first upload to return. If there is an upload with this id it will be returned in the list. If not,
        /// the first upload after this the first one after this id. 
        /// </summary>
        public string StartFileId { get; set; }

        /// <summary>
        /// The maximum number of files to return from this call. The default value is 100 and the maximum allowed is 1000. 
        /// </summary>
        public long MaxFileCount
        {
            get { return _maxFileCount; }
            set
            {
                if (value < 0 || value > MaximumNamesPerTransaction)
                    throw new ArgumentException($"Property '{nameof(MaxFileCount)}' must be a minimum of 1 and a maximum of {MaximumNamesPerTransaction} of files to return.");
                else
                    _maxFileCount = value;
            }
        }
        private long _maxFileCount;

        /// <summary>
        /// Converts the value of this instance to a memory cache key.
        /// </summary>
        public string ToCacheKey()
        {
            return $"{GetType().Name}--{GetHashCode().ToString()}";
        }

        ///	<summary>
        ///	Debugger display for this object.
        ///	</summary>
        [JsonIgnore]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{{{nameof(BucketId)}: {BucketId}}}"; }
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ListUnfinishedLargeFilesRequest);
        }

        public bool Equals(ListUnfinishedLargeFilesRequest other)
        {
            return other != null &&
                   BucketId == other.BucketId &&
                   NamePrefix == other.NamePrefix &&
                   StartFileId == other.StartFileId &&
                   MaxFileCount == other.MaxFileCount;
        }

        public override int GetHashCode()
        {
            var hashCode = -1971292434;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(BucketId);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(NamePrefix);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(StartFileId);
            hashCode = hashCode * -1521134295 + MaxFileCount.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(ListUnfinishedLargeFilesRequest left, ListUnfinishedLargeFilesRequest right)
        {
            return EqualityComparer<ListUnfinishedLargeFilesRequest>.Default.Equals(left, right);
        }

        public static bool operator !=(ListUnfinishedLargeFilesRequest left, ListUnfinishedLargeFilesRequest right)
        {
            return !(left == right);
        }
    }
}
