namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Specifies a bucket security type.
    /// </summary>
    public enum BucketType
    {
        /// <summary>
        /// The files in this bucket can be downloaded by anybody.
        /// </summary>
        AllPublic,

        /// <summary>
        /// The files in this bucket require an authorization token to download. 
        /// </summary>
		AllPrivate,

        /// <summary>
        /// The files in this bucket indicate a private bucket containing a snapshot created on the B2 website.
        /// </summary>
        Snapshot
    }
}
