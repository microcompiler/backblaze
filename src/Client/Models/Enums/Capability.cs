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
        /// Allows the client to read bucket information.
        /// </summary>
        ReadBuckets,

        /// <summary>
        /// Allows the client to list the buckets in the account.
        /// </summary>
        ListBuckets,

        /// <summary>
        /// Lets the client list the names and IDs of all buckets in the account, even with application keys that are restricted to a bucket.
        /// There are not currently any APIs that list just bucket names and IDs.
        /// </summary>
        ListAllBucketNames,

        /// <summary>
        /// Allows the client to create new buckets in the account and update the bucket type, bucket info, and lifecycle rules.
        /// </summary>
        WriteBuckets,

        /// <summary>
        /// Allows the client to delete any bucket in the account.
        /// </summary>
        DeleteBuckets,

        /// <summary>
        /// Lets clients see whether a bucket has File Lock enabled.
        /// Also allows clients to see the default lock mode and period (if configured) on a bucket that has File Lock enabled.
        /// </summary>
        ReadBucketRetentions,

        /// <summary>
        /// Lets clients create a bucket that has File Lock enabled.
        /// Also allows clients to update the default lock mode and period on a bucket that has File Lock enabled.
        /// </summary>
        WriteBucketRetentions,

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
        DeleteFiles,

        /// <summary>
        /// Lets clients see the File Lock settings (mode and expiration) on a file.
        /// These files must be located in a bucket that has File Lock enabled.
        /// </summary>
        ReadFileRetentions,

        /// <summary>
        /// Lets clients update the File Lock settings (mode and expiration) on a file.
        /// These files must be located in a bucket that has File Lock enabled.
        /// </summary>
        WriteFileRetentions,

        /// <summary>
        /// Lets clients delete governance mode-locked files. Also allows governance mode expiration to be shortened and to switch governance mode to compliance mode.
        /// Used in these APIs:
        /// </summary>
        BypassGovernance,

        /// <summary>
        /// Lets client read file legal holds.
        /// </summary>
        ReadFileLegalHolds,

        /// <summary>
        /// Lets client write file legal holds.
        /// </summary>
        WriteFileLegalHolds,

        /// <summary>
        /// Permission to read default bucket encryption settings.
        /// </summary>
        ReadBucketEncryption,

        /// <summary>
        /// Permission to write default bucket encryption settings.
        /// </summary>
        WriteBucketEncryption,
    }
}
