using System;
using System.Net.Http;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;

using Bytewizer.Backblaze.Handlers;

namespace Bytewizer.Backblaze.Client
{
    /// <summary>
    /// Represents a default implementation of the <see cref="BackblazeClient"/> which uses <see cref="HttpClient"/> for making requests.
    /// </summary>
    public class BackblazeClient : Storage, IStorageClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BackblazeClient"/> class with defaults.
        /// </summary>
        public BackblazeClient()
            : base(GetHttpClient(0, null), new ClientOptions(), new NullLoggerFactory(), null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BackblazeClient"/> class.
        /// </summary>
        public BackblazeClient(IClientOptions options, ILoggerFactory logger = null, IMemoryCache cache = null)
            : base(GetHttpClient(options.RetryCount, logger), options, logger, cache)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BackblazeClient"/> class.
        /// </summary>
        public BackblazeClient(HttpClient client, IClientOptions options, ILoggerFactory logger = null, IMemoryCache cache = null)
            : base(client, options, logger, cache)
        { }

        /// <summary>
        /// Creates an initialized instance of the client connected to Backblaze B2 Cloud Storage with pre-configured defaults.
        /// </summary>
        /// <param name="keyId">The identifier for the key.</param>
        /// <param name="applicationKey">The secret part of the key. You can use either the master application key or a normal application key.</param>
        public static BackblazeClient Initialize(string keyId, string applicationKey)
        {
            var client = new BackblazeClient();
            if (client == null) throw new ArgumentNullException(nameof(client));

            client.Connect(keyId, applicationKey);

            return client;
        }

        private static HttpClient GetHttpClient(int retryCount, ILoggerFactory loggerFactory)
        {            
            if (retryCount == 0)
                retryCount = ClientOptions.DefaultRetryCount;

            if (loggerFactory == null)
                loggerFactory = new NullLoggerFactory();

            var logger = loggerFactory.CreateLogger<BackblazeClient>();
            var policy = PolicyManager.CreateRetryPolicy(retryCount, logger);
            
            return ClientFactory.Create(new DelegatingHandler[] { 
                new UserAgentHandler(), 
                new HttpErrorHandler(policy), 
                new LoggingHandler(logger) 
            });
        }
    }
}
