using Bytewizer.Backblaze.Client;
using Bytewizer.Backblaze.Models;
using System;

namespace Bytewizer.Backblaze.Agent
{
    /// <summary>
    /// Backblaze B2 Cloud Storage service agent options interface
    /// </summary>
    public interface IAgentOptions
    {
        /// <summary>
        /// The base authentication url of the Backblaze B2 Cloud Storage service.
        /// </summary>
        Uri AuthUrl { get; set; }

        /// <summary>
        /// The key identifier used to log in to the Backblaze B2 Cloud Storage service.
        /// </summary>
        string KeyId { get; set; }

        /// <summary>
        /// The secret part of the key used to log in to the Backblaze B2 Cloud Storage service.
        /// </summary>
        string ApplicationKey { get; set; }
        
        /// <summary>
        /// A flag string for X-Bx-Test-Mode headers for debugging.  This is for debugging purposes only.
        /// Setting it to "fail_some_uploads", "expire_some_account_authorization_tokens" or "force_cap exceeded" will cause
        /// server to return specific errors used during debugging.
        /// </summary>
        string TestMode { get; set; }

        /// <summary>
        /// The time in seconds to wait before the client request times out.
        /// </summary>
        double AgentTimeout { get; set; }

        /// <summary>
        /// The time in seconds that the message handler instance can be reused.
        /// </summary>
        double HandlerLifetime { get; set; }

        /// <summary>
        /// The number of times the client will retry failed requests before timing out.
        /// </summary>
        int AgentRetryCount { get; set; }

        /// <summary>
        /// The maxium number of parallel upload connections established.
        /// </summary>
        int UploadConnections { get; set; } 

        /// <summary>
        /// Cutoff size for switching to chunked upload.
        /// </summary>
        FileSize UploadCutoffSize { get; set; }

        /// <summary>
        /// Chunk size of upload.
        /// </summary>
        FileSize UploadPartSize { get; set; }

        /// <summary>
        /// The maxium number of parallel download connections established.
        /// </summary>
        int DownloadConnections { get; set; }

        /// <summary>
        /// Download cutoff size for switching to chunked parts in bits.
        /// </summary>
        FileSize DownloadCutoffSize { get; set; }

        /// <summary>
        /// Download part size in bits of chunked parts.
        /// </summary>
        FileSize DownloadPartSize { get; set; }

        /// <summary>
        /// Validate the required values.
        /// </summary>
        void Validate();
    }
}