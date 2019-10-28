using System;

using Bytewizer.Backblaze.Client;

namespace Bytewizer.Backblaze.Client
{
    /// <summary>
    /// Client options for Backblaze B2 Cloud Storage interface.
    /// </summary>
    public interface IClientOptions 
    {
        /// <summary>
        /// The identifier for the key.
        /// </summary>
        string KeyId { get; set; }

        /// <summary>
        /// The secret part of the key. You can use either the master application key or a normal application key.
        /// </summary>
        string ApplicationKey { get; set; }

        /// <summary>
        /// The base authentication url of Backblaze B2 Cloud Storage.
        /// </summary>
        Uri AuthUrl { get; set; }

        /// <summary>
        /// The time in seconds that the message handler instance can be reused.
        /// </summary>
        double HandlerLifetime { get; set; }

        /// <summary>
        /// The time in seconds to wait before the client request times out.
        /// </summary>
        double Timeout { get; set; }

        /// <summary>
        /// The number of times the client will retry failed requests before timing out.
        /// </summary>
        int RetryCount { get; set; }

        /// <summary>
        /// The maximum number of parallel request connections established.
        /// </summary>
        int RequestMaxParallel { get; set; }

        /// <summary>
        /// The maximum number of parallel download connections established.
        /// </summary>
        int DownloadMaxParallel { get; set; }

        /// <summary>
        /// Download cutoff size for switching to chunked parts in bytes.
        /// </summary>
        long DownloadCutoffSize { get; set; }

        /// <summary>
        /// Download part size of chunked parts in bytes.
        /// </summary>
        long DownloadPartSize { get; set; }

        /// <summary>
        /// The maximum number of parallel upload connections established.
        /// </summary>
        int UploadMaxParallel { get; set; }

        /// <summary>
        /// Upload cutoff size for switching to chunked parts in bytes.
        /// </summary>
        long UploadCutoffSize { get; set; }

        /// <summary>
        /// Upload part size of chunked parts in bytes.
        /// </summary>
        long UploadPartSize { get; set; }

        /// <summary>
        /// Use the recommended part size returned by Backblaze B2 Cloud Storage.
        /// </summary>
        bool AutoSetPartSize { get; set; }

        /// <summary>
        /// This is for testing use only and not recomended for production environments. 
        /// </summary>
        bool ChecksumDisabled { get; set; }

        /// <summary>
        /// This is for testing use only and not recomended for production environments. 
        /// </summary>
        string TestMode { get; set; }

        /// <summary>
        /// Validate required values and initialize default values.
        /// </summary>
        void Validate();
    }
}