using Bytewizer.Backblaze.Models;

namespace Bytewizer.Backblaze.Client
{
    /// <summary>
    /// Represents a <see cref="CacheKeys"/> returned from the Backblaze B2 Cloud Storage service.
    /// </summary>
    public class CacheKeys
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CacheKeys"/> class.
        /// </summary>
        private CacheKeys(string value) { Value = value; }

        /// <summary>
        /// Gets a <see cref="string" /> that represents this value.
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Represents the upload url cache key.
        /// </summary>
        public static CacheKeys UploadUrl { get { return new CacheKeys(typeof(GetUploadUrlRequest).Name); } }

        /// <summary>
        /// Represents the upload part url cache key.
        /// </summary>
        public static CacheKeys UploadPartUrl { get { return new CacheKeys(typeof(GetUploadPartUrlRequest).Name); } }

        /// <summary>
        /// Represents the list buckets cache key.
        /// </summary>
        public static CacheKeys ListBuckets { get { return new CacheKeys(typeof(ListBucketsRequest).Name); } }

        /// <summary>
        /// Represents the list file names cache key.
        /// </summary>
        public static CacheKeys ListFileNames { get { return new CacheKeys(typeof(ListFileNamesRequest).Name); } }

        /// <summary>
        /// Represents the list file names cache key.
        /// </summary>
        public static CacheKeys ListFileVersions { get { return new CacheKeys(typeof(ListFileVersionRequest).Name); } }

        /// <summary>
        /// Represents the list key cache key.
        /// </summary>
        public static CacheKeys ListKeys { get { return new CacheKeys(typeof(ListKeysRequest).Name); } }

        /// <summary>
        /// Represents the list parts cache key.
        /// </summary>
        public static CacheKeys ListParts { get { return new CacheKeys(typeof(ListPartsRequest).Name); } }

        /// <summary>
        /// Represents the list unfinished large files cache key.
        /// </summary>
        public static CacheKeys ListUnfinished { get { return new CacheKeys(typeof(ListUnfinishedLargeFilesRequest).Name); } }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        public override string ToString()
        {
            return this.Value;
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        public static implicit operator string(CacheKeys key) { return key.ToString(); }
    }
}
