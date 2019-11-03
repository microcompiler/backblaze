using System;
using Bytewizer.Backblaze.Models;

namespace Bytewizer.Backblaze.Client
{
    /// <summary>
    /// Client options for Backblaze B2 Cloud Storage.
    /// </summary>
    public class ClientOptions : IClientOptions
    {
        #region Constants
        
        /// <summary>
        /// The default base authentication url of Backblaze B2 Cloud Storage.
        /// </summary>
        public static readonly Uri DefaultAuthUrl = new Uri("https://api.backblazeb2.com/b2api/v2/");

        /// <summary>
        /// The default time in seconds that the message handler instance can be reused.
        /// </summary>
        public static readonly double DefaultHandlerLifetime = 600;
        
        /// <summary>
        /// Represents the default time in seconds to wait before the client request times out.
        /// </summary>
        public static readonly double DefaultTimeout = 600;

        /// <summary>
        /// Represents the default number of times the client will retry failed requests before timing out.
        /// </summary>
        public static readonly int DefaultRetryCount = 5;

        /// <summary>
        /// Represents the default maximum number of parallel request connections established.
        /// </summary>
        public static readonly int DefaultRequestMaxParallel = 10;

        /// <summary>
        /// Represents the default maximum number of parallel download connections established.
        /// </summary>
        public static readonly int DefaultDownloadMaxParallel = 5;

        /// <summary>
        /// Represents the default download cutoff size for switching to chunked parts in bytes.
        /// </summary>
        public static readonly long DefaultDownloadCutoffSize = 100 * FileSize.MegaByte;

        /// <summary>
        /// Represents the default download part size in bytes. This field is constant.
        /// </summary>
        public static readonly long DefaultDownloadPartSize = 100 * FileSize.MegaByte;

        /// <summary>
        /// Represents the default maximum number of parallel upload connections established.
        /// </summary>
        public static readonly int DefaultUploadMaxParallel = 3;

        /// <summary>
        /// Represents the default upload cutoff size for switching to chunked parts in bytes. 
        /// </summary>
        public static readonly long DefaultUploadCutoffSize = 100 * FileSize.MegaByte;

        /// <summary>
        /// Represents the default upload part size in bytes. 
        /// </summary>
        public static readonly long DefaultUploadPartSize = 100 * FileSize.MegaByte;

        /// <summary>
        /// Represents the minimum file cutoff size in bytes. 
        /// </summary>
        public static readonly long MinimumCutoffSize = 5 * FileSize.MegaByte;

        /// <summary>
        /// Represents the maximum file size in bytes. 
        /// </summary>
        public static readonly long MaximumFileSize = 10 * FileSize.TeraByte;

        /// <summary>
        /// Represents the minimum file part size in bytes. 
        /// </summary>
        public static readonly long MinimumPartSize = 5 * FileSize.MegaByte;

        /// <summary>
        /// Represents the maximum file part size in bytes. 
        /// </summary>
        public static readonly long MaximumPartSize = 5 * FileSize.GigaByte;

        #endregion

        /// <summary>
        /// The identifier for the key.
        /// </summary>
        public string KeyId { get; set; } = string.Empty;

        /// <summary>
        /// The secret part of the key. You can use either the master application key or a normal application key.
        /// </summary>
        public string ApplicationKey { get; set; } = string.Empty;

        /// <summary>
        /// The base authentication url of Backblaze B2 Cloud Storage.
        /// </summary>
        public Uri AuthUrl { get; set; } = DefaultAuthUrl;

        /// <summary>
        /// The time in seconds that the message handler instance can be reused.
        /// </summary>
        public double HandlerLifetime
        {
            get { return _handlerLifetime; }
            set { _handlerLifetime = (value <= 0) ? DefaultHandlerLifetime : value; }
        }
        private double _handlerLifetime = DefaultHandlerLifetime;

        /// <summary>
        /// The time in seconds to wait before the client request times out.
        /// </summary>
        public double Timeout
        {
            get { return _timeout; }
            set { _timeout = (value <= 0) ? DefaultTimeout : value; }
        }
        private double _timeout = DefaultTimeout;

        /// <summary>
        /// The number of times the client will retry failed requests before timing out.
        /// </summary>
        public int RetryCount
        {
            get { return _retryCount; }
            set { _retryCount = (value <= 0) ? DefaultRetryCount : value; }
        }
        private int _retryCount = DefaultRetryCount;

        /// <summary>
        /// The maximum number of parallel request connections established.
        /// </summary>
        public int RequestMaxParallel
        {
            get { return _RequestMaxParallel; }
            set { _RequestMaxParallel = (value <= 0) ? DefaultRequestMaxParallel : value; }
        }
        private int _RequestMaxParallel = DefaultRequestMaxParallel;

        /// <summary>
        /// The maximum number of parallel download connections established.
        /// </summary>
        public int DownloadMaxParallel
        {
            get { return _downloadMaxParallel; }
            set { _downloadMaxParallel = (value <= 0) ? DefaultDownloadMaxParallel : value; }
        }
        private int _downloadMaxParallel = DefaultDownloadMaxParallel;

        /// <summary>
        /// Download cutoff size for switching to chunked parts in bytes.
        /// </summary>
        public long DownloadCutoffSize
        {
            get { return _downloadCutoffSize; }
            set
            {
                _downloadCutoffSize = value;

                if (value < MinimumCutoffSize)
                    _downloadCutoffSize = DefaultDownloadCutoffSize;

                if (value > MaximumFileSize)
                    _downloadCutoffSize = MaximumFileSize;

                if (value < _downloadPartSize)
                    _downloadPartSize = _downloadCutoffSize;
            }
        }
        private long _downloadCutoffSize = DefaultDownloadCutoffSize;

        /// <summary>
        /// Download part size of chunked parts in bytes.
        /// </summary>
        public long DownloadPartSize
        {
            get { return _downloadPartSize; }
            set
            {
                _downloadPartSize = value;

                if (value < MinimumPartSize)
                    _downloadPartSize = DefaultDownloadPartSize;

                if (value > MaximumPartSize)
                    _downloadPartSize = MaximumPartSize;

                if (value < _uploadCutoffSize)
                    _downloadCutoffSize = _downloadPartSize;
            }
        }
        private long _downloadPartSize = DefaultDownloadPartSize;

        /// <summary>
        /// The maximum number of parallel upload connections established.
        /// </summary>
        public int UploadMaxParallel
        {
            get { return _uploadMaxParallel; }
            set { _uploadMaxParallel = (value <= 0) ? DefaultUploadMaxParallel : value; }
        }
        private int _uploadMaxParallel = DefaultUploadMaxParallel;

        /// <summary>
        /// Upload cutoff size for switching to chunked parts in bytes.
        /// </summary>
        public long UploadCutoffSize
        {
            get { return _uploadCutoffSize; }
            set
            {
                _uploadCutoffSize = value;

                if (value < MinimumCutoffSize)
                    _uploadCutoffSize = DefaultUploadCutoffSize;

                if (value > MaximumFileSize)
                    _uploadCutoffSize = MaximumFileSize;

                if (value < _uploadPartSize)
                    _uploadPartSize = _uploadCutoffSize;
            }
        }
        private long _uploadCutoffSize = DefaultUploadCutoffSize;

        /// <summary>
        /// Upload part size of chunked parts in bytes.
        /// </summary>
        public long UploadPartSize
        {
            get { return _uploadPartSize; }
            set
            {
                _uploadPartSize = value;

                if (value < MinimumPartSize)
                    _uploadPartSize = DefaultUploadPartSize;

                if (value > MaximumPartSize)
                    _uploadPartSize = MaximumPartSize;

                if (value < _uploadCutoffSize)
                        _uploadCutoffSize = _uploadPartSize;
            }
        }
        private long _uploadPartSize = DefaultUploadPartSize;

        /// <summary>
        /// Use the recommended part size returned by Backblaze B2 Cloud Storage.
        /// </summary>
        public bool AutoSetPartSize { get; set; }

        /// <summary>
        /// This is for testing use only and not recomended for production environments. 
        /// </summary>
        public bool ChecksumDisabled { get; set; }

        /// <summary>
        /// This is for testing use only and not recomended for production environments. 
        /// </summary>
        public string TestMode { get; set; }

        /// <summary>
        /// Validate required values and initialize default values.
        /// </summary>
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(KeyId))
                throw new ConfigurationException("Key id is not defined.", nameof(KeyId));

            if (string.IsNullOrWhiteSpace(ApplicationKey))
                throw new ConfigurationException("Application key is not defined.", nameof(ApplicationKey));

            if (UploadCutoffSize < UploadPartSize)
                throw new ConfigurationException("Upload cutoff size must be greater then or equal to part size.");

            if (DownloadCutoffSize < DownloadPartSize)
                throw new ConfigurationException("Download cutoff size must be greater then or equal to part size.");

        }
    }
}
