using System;

using Bytewizer.Backblaze.Models;

namespace Bytewizer.Backblaze.Agent
{
    /// <summary>
    /// Options for the Backblaze B2 Cloud Storage service.
    /// </summary>
    public class AgentOptions : IAgentOptions
    {
        #region Constants

        /// <summary>
        /// Represents the default number of times the client will retry failed requests before timing out.
        /// </summary>
        public const int DefaultRetryCount = 5;

        /// <summary>
        /// Represents the default number of parallel upload connections established.
        /// </summary>
        public const int DefaultParallelUploads = 3;

        /// <summary>
        /// Represents the default number of parallel download connections established.
        /// </summary>
        public const int DefaultParallelDownloads = 5;

        #endregion

        /// <summary>
        /// The key identifier used to authenticate to the Backblaze B2 Cloud Storage service. 
        /// </summary>
        public string KeyId { get; set; }

        /// <summary>
        /// The secret part of the key used to authenticate.
        /// </summary>
        public string ApplicationKey { get; set; }

        /// <summary>
        /// This is for testing use only and not recomended for production environments. 
        /// Sets "X-Bx-Test-Mode" headers used for debugging and testing. Setting it to "fail_some_uploads", 
        /// "expire_some_account_authorization_tokens" or "force_cap exceeded" will cause the server to return specific errors used for testing.
        /// </summary>
        public string TestMode { get; set; }

        /// <summary>
        /// The maxium number of parallel upload connections established.
        /// </summary>
        public int UploadConnections { get; set; }

        /// <summary>
        /// Upload cutoff size for switching to chunked parts in bits.
        /// </summary>
        public FileSize UploadCutoffSize { get; set; }

        /// <summary>
        /// Upload part size in bits of chunked parts.
        /// </summary>
        public FileSize UploadPartSize { get; set; }

        /// <summary>
        /// The maxium number of parallel download connections established.
        /// </summary>
        public int DownloadConnections { get; set; }

        /// <summary>
        /// Download cutoff size for switching to chunked parts in bits.
        /// </summary>
        public FileSize DownloadCutoffSize { get; set; }

        /// <summary>
        /// Download part size in bits of chunked parts.
        /// </summary>
        public FileSize DownloadPartSize { get; set; }

        /// <summary>
        /// This is for testing use only and not recomended for production environments. 
        /// Disable checksums for large files.
        /// </summary>
        public bool DisableChecksum { get; set; }

        /// <summary>
        /// The base authentication url of the Backblaze B2 Cloud Storage service.
        /// </summary>
        public Uri AuthUrl { get; set; } = new Uri("https://api.backblazeb2.com/b2api/v2/");

        /// <summary>
        /// Customer endpoint for downloads.  This is usually set to a Cloudflare CDN Url as Backblaze offers 
        /// free egree for data downloaded through the Cloudflare network.
        /// </summary>
        public string DownloadUrl { get; set; }

        /// <summary>
        /// The time in seconds to wait before the client request times out.
        /// </summary>
        public double AgentTimeout { get; set; }

        /// <summary>
        /// The time in seconds that the message handler instance can be reused.
        /// </summary>
        public double HandlerLifetime { get; set; }
        
        /// <summary>
        /// The number of times the client will retry failed requests before timing out.
        /// </summary>
        public int AgentRetryCount { get; set; }

        /// <summary>
        /// Validate the required values
        /// </summary>
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(KeyId))
                throw new ConfigurationException("Configuration error: Key id is not defined.", nameof(KeyId));

            if (string.IsNullOrWhiteSpace(ApplicationKey))
                throw new ConfigurationException("Configuration error: Application key is not defined.", nameof(ApplicationKey));

            if (UploadCutoffSize < UploadPartSize)
                throw new ConfigurationException("Configuration error: Upload cutoff size must be greater then part size.", nameof(UploadCutoffSize));

            if (DownloadCutoffSize < DownloadPartSize)
                throw new ConfigurationException("Configuration error: Download cutoff size must be greater then part size.", nameof(UploadCutoffSize));

            if (UploadConnections <= 0) UploadConnections = DefaultParallelUploads;
            if (UploadCutoffSize <= 0) UploadCutoffSize = FileSize.DefaultUploadCutoffSize;
            if (UploadPartSize <= 0) UploadPartSize = FileSize.DefaultUploadPartSize;
            if (DownloadConnections <= 0) DownloadConnections = DefaultParallelDownloads;
            if (DownloadCutoffSize <= 0) DownloadCutoffSize = FileSize.DefaultDownloadCutoffSize;
            if (DownloadPartSize <= 0) DownloadPartSize = FileSize.DefaultUploadPartSize;
            if (AgentTimeout <= 0) AgentTimeout = 600; // 10 minutes
            if (HandlerLifetime <= 0) HandlerLifetime = 600; // 10 minutes
            if (AgentRetryCount <= 0) AgentRetryCount = DefaultRetryCount;
        }
    }
}
