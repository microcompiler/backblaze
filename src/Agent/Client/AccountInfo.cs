using System;

namespace Bytewizer.Backblaze.Client
{
    /// <summary>
    /// Represents the <see cref="AccountInfo"/> returned from the Backblaze B2 Cloud Storage service.
    /// </summary>
    public class AccountInfo
    {
        /// <summary>
        /// The identifier for the account.
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// The base authentication address of the Backblaze B2 Cloud Storage service.
        /// </summary>
        public Uri AuthUrl { get; set; } = new Uri("https://api.backblazeb2.com/b2api/v2/");

        /// <summary>
        /// The base address of the Backblaze B2 API calls except for uploading and downloading files.
        /// </summary>
        public Uri ApiUrl { get; set; }

        /// <summary>
        /// The base download address of the Backblaze B2 Cloud Storage service.
        /// </summary>
        public Uri DownloadUrl { get; set; }

    }
}
