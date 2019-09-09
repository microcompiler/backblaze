using System;
using System.Collections.Generic;
using System.Diagnostics;

using Newtonsoft.Json;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains information to create a list file names request.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class ListFileNamesRequest : IEquatable<ListFileNamesRequest>, IRequest
    {
        /// <summary>
        /// Represents the default number of files per transaction.
        /// </summary>
        public const int DefaultFilesPerTransaction = 100;

        /// <summary>
        /// Maximum number of files per transaction.
        /// </summary>
        public const int MaximumFilesPerTransaction = 10000;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListFileNamesRequest"/> class.
        /// </summary>
        /// <param name="bucketId">The bucket id to look for file names in.</param>
        public ListFileNamesRequest(string bucketId)
        {
            // Validate required arguments
            if (string.IsNullOrWhiteSpace(bucketId))
                throw new ArgumentException("Argument can not be null, empty, or consist only of white-space characters.", nameof(bucketId));
            
            // Initialize and set required properties
            BucketId = bucketId;
        }

        /// <summary>
        /// The bucket id to look for file names in.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string BucketId { get; private set; }

        /// <summary>
        /// The first file name to return. If there is a file with this name it will be returned in the list.
        /// If not the first file name after this the first one after this name. 
        /// </summary>
        public string StartFileName { get; set; }

        /// <summary>
        /// The maximum number of files to return from this call. 
        /// The default value is <see cref="DefaultFilesPerTransaction"/> and the maximum allowed is <see cref="MaximumFilesPerTransaction"/>.
        /// Passing in 0 means to use the default of <see cref="DefaultFilesPerTransaction"/>. NOTE: <see cref="ListFileNamesRequest"/> is a Class C transaction. 
        /// If you set <see cref="MaxFileCount"/> to more than 1000 and more than 1000 are returned, the call will be billed as multiple transactions,
        /// as if you had made requests in a loop asking for 1000 at a time. For example: if you set <see cref="MaxFileCount"/> to 10000 and 3123 items are returned,
        /// you will be billed for 4 Class C transactions. 
        /// </summary>
        public long MaxFileCount
        {
            get { return _maxFileCount; }
            set
            {
                if (value < 0 || value > MaximumFilesPerTransaction)
                    throw new ArgumentException($"Property '{nameof(MaxFileCount)}' must be a minimum of 1 and a maximum of {MaximumFilesPerTransaction} of files to return.");
                else
                    _maxFileCount = value;
            }
        }
        private long _maxFileCount = DefaultFilesPerTransaction;


        /// <summary>
        /// Files returned will be limited to those with the given prefix. Defaults to <see cref="string.Empty"/> which matches all files. 
        /// </summary>
        public string Prefix { get; set; } = string.Empty;

        /// <summary>
        /// Files returned will be limited to those within the top folder or any one subfolder. Defaults to <see cref="null"/>. Folder names 
        /// will also be returned. The delimiter character will be used to "break" file names into folders. 
        /// </summary>
        public string Delimiter { get; set; } = null;

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

        #region IEquatable

        /// <summary>
        /// Determines whether this object is equal <paramref name="obj"/>.
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as ListFileNamesRequest);
        }

        /// <summary>
        /// Determines whether this object is equal <paramref name="other"/>.
        /// </summary>
        public bool Equals(ListFileNamesRequest other)
        {
            return other != null &&
                   BucketId == other.BucketId &&
                   StartFileName == other.StartFileName &&
                   MaxFileCount == other.MaxFileCount &&
                   Prefix == other.Prefix &&
                   Delimiter == other.Delimiter;
        }

        /// <summary>
        /// Provides hash code for the request.
        /// </summary>
        public override int GetHashCode()
        {
            var hashCode = -429382267;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(BucketId);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(StartFileName);
            hashCode = hashCode * -1521134295 + MaxFileCount.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Prefix);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Delimiter);
            return hashCode;
        }

        /// <summary>
        /// Determines whether the given <paramref name="left"/> is equal <paramref name="right"/>.
        /// </summary>
        public static bool operator ==(ListFileNamesRequest left, ListFileNamesRequest right)
        {
            return EqualityComparer<ListFileNamesRequest>.Default.Equals(left, right);
        }

        /// <summary>
        /// Determines whether the given <paramref name="left"/> is not equal <paramref name="right"/>.
        /// </summary>
        public static bool operator !=(ListFileNamesRequest left, ListFileNamesRequest right)
        {
            return !(left == right);
        }

        #endregion
    }
}
