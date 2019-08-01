using System;

namespace Bytewizer.Backblaze.Client
{
    /// <summary>
    /// Represents the account information returned from the Backblaze server.
    /// </summary>
    public class AccountInfo
    {
        /// <summary>
        /// The identifier for the account.
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// The base authentication address of the Backblaze B2 API.
        /// </summary>
        public Uri AuthUrl { get; set; } = new Uri("https://api.backblazeb2.com/b2api/v2/");

        /// <summary>
        /// The base address of the Backblaze B2 API calls except for uploading and downloading files.
        /// </summary>
        public Uri ApiUrl { get; set; }

        /// <summary>
        /// The base download address of the Backblaze B2 API.
        /// </summary>
        public Uri DownloadUrl { get; set; }

        /// <summary>
        /// The recommended size for each part of a large file. We recommend using this
        /// part size for optimal upload performance.
        /// </summary>
        public long RecommendedPartSize { get; set; }

        /// <summary>
        /// The smallest possible size of a part of a large file (except the last one). This is smaller 
        /// than the <see cref="RecommendedPartSize"/>. If you use it, you may find that it takes longer
        /// overall to upload a large file.
        /// </summary>
        public long AbsoluteMinimumPartSize { get; set; }
    }
}
