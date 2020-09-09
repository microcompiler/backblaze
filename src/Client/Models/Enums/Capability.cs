namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Specifies capabilities associated with an appication key.
    /// </summary>
    public enum Capability
    {
        /// <summary>
        /// Allows the client to enumerate the application keys in an account.
        /// </summary>
        ListKeys,

        /// <summary>
        /// Allows the client to create new application keys and does not impose any restrictions on what those keys can do.
        /// </summary>
        WriteKeys,

        /// <summary>
        /// Allows the client to delete any application key that belongs to the account.
        /// </summary>
        DeleteKeys,

        /// <summary>
        /// Allows the client to list the buckets in the account.
        /// </summary>
        ListBuckets,
        
        /// <summary>
        /// Allows the client to read bucket information
        /// </summary>
        ReadBuckets,

        /// <summary>
        /// Allows the client to create new buckets in the account and update the bucket type, bucket info, and lifecycle rules.
        /// </summary>
        WriteBuckets,

        /// <summary>
        /// Allows the client to delete any bucket in the account.
        /// </summary>
        DeleteBuckets,

        /// <summary>
        /// Allows the client to list files and their metadata.
        /// </summary>
        ListFiles,

        /// <summary>
        /// Allows the to client see the metadata for files, and download their contents.
        /// </summary>
        ReadFiles,

        /// <summary>
        /// Allows the client to create authorization tokens for downloading files.
        /// </summary>
        ShareFiles,

        /// <summary>
        /// Allows client to upload files including both regular files and large files.
        /// </summary>
        WriteFiles,

        /// <summary>
        /// Allows client to delete files.
        /// </summary>
        DeleteFiles
    }
}
