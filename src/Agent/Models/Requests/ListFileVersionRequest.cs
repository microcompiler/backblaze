using System;
using System.Collections.Generic;
using System.Diagnostics;

using Newtonsoft.Json;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains information to create a list file version request.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class ListFileVersionRequest : IRequest
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
        /// Initializes a new instance of the <see cref="ListFileVersionRequest"/> class.
        /// </summary>
        /// <param name="bucketId">The bucket id to look for file names in.</param>
        public ListFileVersionRequest(string bucketId)
        {
            // Validate required arguments
            if (string.IsNullOrWhiteSpace(bucketId))
                throw new ArgumentException("Argument can not be null, empty, or consist only of white-space characters.", nameof(bucketId));

            // Initialize and set required properties
            BucketId = bucketId;
        }

        /// <summary>
        /// The bucket to look for file names in.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string BucketId { get; private set; }

        /// <summary>
        /// The first file name to return. If there is a file with this name,
        /// it will be returned in the list. If not, the first file name after this the first one after this name. 
        /// </summary>
        public string StartFileName { get; set; }

        /// <summary>
        /// The first file id to return.  <see cref="StartFileName"/> must also be provided if <see cref="StartFileId"/> is specified.
        /// </summary>
        public string StartFileId { get; set; }

        /// <summary>
        /// The maximum number of files to return from this call. 
        /// The default value is <see cref="DefaultFilesPerTransaction"/> and the maximum allowed is <see cref="MaximumFilesPerTransaction"/>.
        /// Passing in 0 means to use the default of <see cref="DefaultFilesPerTransaction"/>. NOTE: <see cref="ListFileVersionRequest"/> is a Class C transaction. 
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
        /// Files returned will be limited to those within the top folder, or any one subfolder. Defaults to <see cref="null"/>. Folder names 
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

        /// <summary>
        /// Returns a hash code for this object.
        /// </summary>
        public override int GetHashCode()
        {
            var hashCode = 766801321;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(BucketId);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(StartFileName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(StartFileId);
            hashCode = hashCode * -1521134295 + MaxFileCount.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Prefix);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Delimiter);
            return hashCode;
        }
    }
}
