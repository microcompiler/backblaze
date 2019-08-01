using System;
using System.Diagnostics;

using Newtonsoft.Json;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains information to create a list unfinished large file request.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class ListUnfinishedLargeFilesRequest : IRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListUnfinishedLargeFilesRequest"/> class.
        /// </summary>
        /// <param name="bucketId">The bucket id to look for file names in.</param>
        public ListUnfinishedLargeFilesRequest(string bucketId)
        {
            // Validate required arguments
            if (string.IsNullOrWhiteSpace(bucketId))
                throw new ArgumentException("Argument can not be null, empty, or consist only of white-space characters.", nameof(bucketId));
            
            // Initialize and set required properties
            BucketId = bucketId;
            MaxFileCount = 100;
        }

        /// <summary>
        /// The bucket id to look for file names in.
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
        /// The maximum number of files to return from this call. The default value is 100 and the maximum allowed is 100. 
        /// </summary>
        public long MaxFileCount
        {
            get { return _maxFileCount; }
            set
            {
                if (value < 0 || value > 100)
                    throw new ArgumentException($"Property '{nameof(MaxFileCount)}' must be a minimum of 1 and a maximum of 100 of files to return.");
                else
                    _maxFileCount = value;
            }
        }
        private long _maxFileCount;

        ///	<summary>
        ///	Debugger display for this object.
        ///	</summary>
        [JsonIgnore]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{{{nameof(BucketId)}: {BucketId}}}"; }
        }
    }
}
