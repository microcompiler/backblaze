using Bytewizer.Backblaze.Client;
using System;
using System.Net.Http;

namespace Bytewizer.Backblaze.Agent
{
    /// <summary>
    /// Options for the Backblaze B2 Cloud Storage service.
    /// </summary>
    public class AgentOptions : IAgentOptions
    {
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
        /// Upload cutoff size for switching to chunked parts in bits.
        /// </summary>
        public long UploadCutoffSize { get; set; }

        /// <summary>
        /// Upload part size in bits of chunked parts.
        /// </summary>
        public long UploadPartSize { get; set; }

        /// <summary>
        /// Download cutoff size for switching to chunked parts in bits.
        /// </summary>
        public long DownloadCutoffSize { get; set; }

        /// <summary>
        /// Download part size in bits of chunked parts.
        /// </summary>
        public long DownloadPartSize { get; set; }

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

            if (UploadCutoffSize <= 0) UploadCutoffSize = ApiClient.DefaultUploadCutoffSize;
            if (UploadPartSize <= 0) UploadPartSize = ApiClient.DefaultUploadPartSize;
            if (DownloadCutoffSize <= 0) DownloadCutoffSize = ApiClient.DefaultDownloadCutoffSize;
            if (DownloadPartSize <= 0) DownloadPartSize = ApiClient.DefaultUploadPartSize;
            if (AgentTimeout <= 0) AgentTimeout = 600; // 10 minutes
            if (HandlerLifetime <= 0) HandlerLifetime = 600; // 10 minutes
            if (AgentRetryCount <= 0) AgentRetryCount = 3; // Retry three times before failing
        }
    }
}
