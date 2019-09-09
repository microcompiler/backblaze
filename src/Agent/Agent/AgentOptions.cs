using System;

using Bytewizer.Backblaze.Client;

namespace Bytewizer.Backblaze.Agent
{
    /// <summary>
    /// Agent options for the Backblaze B2 Cloud Storage service.
    /// </summary>
    public class AgentOptions : ClientOptions, IAgentOptions
    {
        #region Constants

        /// <summary>
        /// The default base authentication url of the Backblaze B2 Cloud Storage service.
        /// </summary>
        public static Uri DefaultAuthUrl = new Uri("https://api.backblazeb2.com/b2api/v2/");

        /// <summary>
        /// The default time in seconds that the message handler instance can be reused.
        /// </summary>
        public const double DefaultHandlerLifetime = 600;

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
        /// The base authentication url of the Backblaze B2 Cloud Storage service.
        /// </summary>
        public Uri AuthUrl { get; set; } = DefaultAuthUrl;

        /// <summary>
        /// The time in seconds that the message handler instance can be reused.
        /// </summary>
        public double HandlerLifetime { get; set; } = DefaultHandlerLifetime;

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
                throw new ConfigurationException("Download cutoff size must be greater then or eaqual to part size.");

            if (HandlerLifetime <= 0) HandlerLifetime = DefaultHandlerLifetime;
        }
    }
}