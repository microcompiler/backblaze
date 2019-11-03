using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;

using Bytewizer.Backblaze.Models;
using Bytewizer.Backblaze.Extensions;

namespace Bytewizer.Backblaze.Client
{
    /// <summary>
    /// Represents a base implementation which uses <see cref="HttpClient"/> for making HTTP requests.
    /// </summary>
    public abstract partial class ApiRest : DisposableObject
    {
        #region Constants

        /// <summary>
        /// Represents the default cache time to live (TTL) in seconds.
        /// </summary>
        public readonly TimeSpan DefaultCacheTTL = TimeSpan.FromSeconds(3600);

        #endregion

        #region Lifetime

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiClient"/> class.
        /// </summary>
        public ApiRest(HttpClient httpClient, IClientOptions options, ILogger<ApiRest> logger, IMemoryCache cache)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cache = new CacheManager(cache, logger);
            _policy = new PolicyManager(options, logger);

            Options = options as ClientOptions;
        }

        #region IDisposable

        /// <summary>
        /// Frees resources owned by this instance.
        /// </summary>
        /// <param name="disposing">
        /// True when called via <see cref="IDisposable.Dispose()"/>, false when called from the finalizer.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            // Only managed resources to dispose
            if (!disposing)
                return;

            // Dispose owned objects
            _httpClient?.Dispose();
        }

        #endregion IDisposable

        #endregion

        #region Private Fields

        /// <summary>
        /// The <see cref="HttpClient"/> used for making HTTP requests.
        /// </summary>
        private readonly HttpClient _httpClient;

        /// <summary>
        /// The <see cref="ILogger"/> used for application logging.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// The <see cref="ICacheManager"/> used for application caching.
        /// </summary>
        private readonly ICacheManager _cache;

        /// <summary>
        /// The <see cref="IPolicyManager"/> used for application resilience.
        /// </summary>
        private readonly IPolicyManager _policy;

        #endregion

        #region Public Properties

        /// <summary>
        /// Json serializer.
        /// </summary>
        internal JsonSerializer JsonSerializer { get; } = new JsonSerializer();

        /// <summary>
        /// Client configuration options.
        /// </summary>
        public ClientOptions Options { get; set; } = new ClientOptions();

        /// <summary>
        /// The account information returned from Backblaze B2 Cloud Storage.
        /// </summary>
        public AccountInfo AccountInfo { get; } = new AccountInfo();

        /// <summary>
        /// The authorization token to use with all calls other than <see cref="AuthorizeAccountAync"/>. This authorization token is valid for at most 24 hours.
        /// </summary>
        public AuthToken AuthToken { get; private set; }

        #endregion

        #region Public Methods

        #region Authorize Account

        /// <summary>
        /// Connect to Backblaze B2 Cloud Storage and initialize <see cref="AccountInfo"/>.
        /// </summary>
        public void Connect()
        {
            ConnectAsync(Options.KeyId, Options.ApplicationKey).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Connect to Backblaze B2 Cloud Storage and initialize <see cref="AccountInfo"/>.
        /// </summary>
        /// <param name="keyId">The identifier for the key.</param>
        /// <param name="applicationKey">The secret part of the key. You can use either the master application key or a normal application key.</param>
        public void Connect(string keyId, string applicationKey)
        {
            ConnectAsync(keyId, applicationKey).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Connect to Backblaze B2 Cloud Storage and initialize <see cref="AccountInfo"/>.
        /// </summary>
        public async Task ConnectAsync()
        {
            await ConnectAsync(Options.KeyId, Options.ApplicationKey).ConfigureAwait(false);
        }

        /// <summary>
        /// Connect to Backblaze B2 Cloud Storage and initialize <see cref="AccountInfo"/>.
        /// </summary>
        /// <param name="keyId">The identifier for the key.</param>
        /// <param name="applicationKey">The secret part of the key. You can use either the master application key or a normal application key.</param>
        public async Task ConnectAsync(string keyId, string applicationKey)
        {
            Options.KeyId = keyId;
            Options.ApplicationKey = applicationKey;
            Options.Validate();

            _policy.ConnectAsync = () => ConnectAsync(keyId, applicationKey);
            _cache.Clear();

            var results = await AuthorizeAccountAync(keyId, applicationKey, CancellationToken.None).ConfigureAwait(false);
            if (results.IsSuccessStatusCode)
            {
                AuthToken = new AuthToken(results.Response.AuthorizationToken)
                {
                    Allowed = results.Response.Allowed
                };

                AccountInfo.ApiUrl = new Uri($"{results.Response.ApiUrl}b2api/v2/");
                AccountInfo.DownloadUrl = new Uri($"{results.Response.DownloadUrl}");
                AccountInfo.AccountId = results.Response.AccountId;

                if (Options.AutoSetPartSize)
                {
                    Options.UploadPartSize = results.Response.RecommendedPartSize;
                    Options.UploadCutoffSize = results.Response.RecommendedPartSize;
                    Options.DownloadPartSize = results.Response.RecommendedPartSize;
                    Options.DownloadCutoffSize = results.Response.RecommendedPartSize;
                }

                _logger.LogInformation("Client successfully authenticated to Backblaze B2 Cloud Storage.");
            }
        }

        #endregion

        #region Upload Stream

        /// <summary>
        /// Uploads content by bucket id. 
        /// </summary>
        /// <param name="request">The upload file request content to send.</param>
        /// <param name="content">The upload content to receive.</param>
        /// <param name="progress">A progress action which fires every time the write buffer is cycled.</param>
        /// <param name="cancel">The cancellation token to cancel operation.</param>
        public async Task<IApiResults<UploadFileResponse>> UploadAsync
            (UploadFileByBucketIdRequest request, Stream content, IProgress<ICopyProgress> progress, CancellationToken cancel)
        {
            return await _policy.InvokeUpload.ExecuteAsync(async () =>
            {
                if (content.Length < Options.UploadCutoffSize)
                {
                    var urlRequest = new GetUploadUrlRequest(request.BucketId);
                    var urlResults = await GetUploadUrlAsync(urlRequest, cancel).ConfigureAwait(false);

                    if (urlResults.IsSuccessStatusCode)
                    {
                        var response = urlResults.Response;
                        var fileRequest = new UploadFileRequest(response.UploadUrl, request.FileName, response.AuthorizationToken)
                        {
                            ContentType = request.ContentType,
                            FileInfo = request.FileInfo
                        };

                        return await UploadFileAsync(fileRequest, content, progress, cancel).ConfigureAwait(false);
                    }

                    return new ApiResults<UploadFileResponse>(urlResults.HttpResponse, urlResults.Error);
                }
                else
                {
                    var largeFileRequest = new UploadLargeFileRequest(request.BucketId, request.FileName, content)
                    {
                        ContentType = request.ContentType,
                        FileInfo = request.FileInfo
                    };

                    return await UploadLargeFileAsync(largeFileRequest, progress, cancel).ConfigureAwait(false);
                }
            });
        }

        #endregion

        #region Download Stream

        /// <summary>
        /// Downloads the most recent version of content by bucket and file name. 
        /// </summary>
        /// <param name="request">The <see cref="DownloadFileByIdRequest"/> to send.</param>
        /// <param name="content">The download content to receive.</param>
        /// <param name="progress">A progress action which fires every time the write buffer is cycled.</param>
        /// <param name="cancel">The cancellation token to cancel operation.</param>
        public async Task<IApiResults<DownloadFileResponse>> DownloadAsync
            (DownloadFileByNameRequest request, Stream content, IProgress<ICopyProgress> progress, CancellationToken cancel)
        {
            return await _policy.InvokeDownload.ExecuteAsync(async () =>
            {
                var fileRequest = new DownloadFileByNameRequest(request.BucketName, request.FileName);
                var fileResults = await DownloadFileByNameAsync(fileRequest, null, null, cancel);

                if (fileResults.IsSuccessStatusCode)
                {
                    if (fileResults.Response.ContentLength < Options.DownloadCutoffSize)
                    {
                        return await DownloadFileByNameAsync(request, content, progress, cancel).ConfigureAwait(false);
                    }
                    else
                    {
                        return await DownloadLargeFileAsync(fileRequest, fileResults, content, progress, cancel).ConfigureAwait(false);
                    }
                }

                return fileResults;
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Downloads a specific version of content by file id. 
        /// </summary>
        /// <param name="request">The <see cref="DownloadFileByIdRequest"/> to send.</param>
        /// <param name="content">The download content to receive.</param>
        /// <param name="progress">A progress action which fires every time the write buffer is cycled.</param>
        /// <param name="cancel">The cancellation token to cancel operation.</param>
        public async Task<IApiResults<DownloadFileResponse>> DownloadByIdAsync
            (DownloadFileByIdRequest request, Stream content, IProgress<ICopyProgress> progress, CancellationToken cancel)
        {
            return await _policy.InvokeDownload.ExecuteAsync(async () =>
            {
                var fileRequest = new DownloadFileByIdRequest(request.FileId);
                var fileResults = await DownloadFileByIdAsync(fileRequest, cancel).ConfigureAwait(false);
                if (fileResults.IsSuccessStatusCode)
                {
                    if (fileResults.Response.ContentLength < Options.DownloadCutoffSize)
                    {
                        return await DownloadFileByIdAsync(request, content, progress, cancel).ConfigureAwait(false);
                    }
                    else
                    {
                        return await DownloadLargeFileAsync(fileRequest, fileResults, content, progress, cancel).ConfigureAwait(false);
                    }
                }

                return fileResults;
            }).ConfigureAwait(false);
        }

        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        /// Downloads the most recent version of a large file in chunked parts. 
        /// </summary>
        /// <param name="request">The <see cref="DownloadFileByIdRequest"/> content to send.</param>
        /// <param name="results">The <see cref="DownloadFileResponse"/> results.</param>
        /// <param name="content">The download content to receive.</param>
        /// <param name="progress">A progress action which fires every time the write buffer is cycled.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        private async Task<IApiResults<DownloadFileResponse>> DownloadLargeFileAsync
            (DownloadFileByIdRequest request, IApiResults<DownloadFileResponse> results, Stream content, IProgress<ICopyProgress> progress, CancellationToken cancellationToken)
        {
            var parts = GetContentParts(results.Response.ContentLength, Options.DownloadPartSize);

            foreach (var part in parts)
            {
                var mmultiStream = new MultiStream(content, part.Position, part.Length);
                var partReqeust = new DownloadFileByIdRequest(request.FileId)
                {
                    Range = new RangeHeaderValue(part.Position, part.Position + part.Length - 1)
                };

                var partResults = await DownloadFileByIdAsync(partReqeust, mmultiStream, progress, cancellationToken).ConfigureAwait(false);
                if (!partResults.IsSuccessStatusCode)
                {
                    return new ApiResults<DownloadFileResponse>(partResults.HttpResponse, partResults.Error);
                }
            }

            return results;
        }

        /// <summary>
        /// Downloads the most recent version of a large file in chunked parts. 
        /// </summary>
        /// <param name="request">The <see cref="DownloadFileByIdRequest"/> content to send.</param>
        /// <param name="results">The <see cref="DownloadFileResponse"/> results.</param>
        /// <param name="content">The download content to receive.</param>
        /// <param name="progress">A progress action which fires every time the write buffer is cycled.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        private async Task<IApiResults<DownloadFileResponse>> DownloadLargeFileAsync
            (DownloadFileByNameRequest request, IApiResults<DownloadFileResponse> results, Stream content, IProgress<ICopyProgress> progress, CancellationToken cancellationToken)
        {
            var parts = GetContentParts(results.Response.ContentLength, Options.DownloadPartSize);

            foreach (var part in parts)
            {
                var mmultiStream = new MultiStream(content, part.Position, part.Length);
                var partReqeust = new DownloadFileByNameRequest(request.BucketName, request.FileName)
                {
                    Range = part.RangeHeader
                };

                var partResults = await DownloadFileByNameAsync(partReqeust, mmultiStream, progress, cancellationToken).ConfigureAwait(false);
                if (!partResults.IsSuccessStatusCode)
                {
                    return new ApiResults<DownloadFileResponse>(partResults.HttpResponse, partResults.Error);
                }
            }

            return results;
        }

        /// <summary>
        /// Uploads a large file in chunked parts. 
        /// </summary>
        /// <param name="request">The <see cref="UploadFileRequest"/> content to send.</param>
        /// <param name="progress">A progress action which fires every time the write buffer is cycled.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        private async Task<IApiResults<UploadFileResponse>> UploadLargeFileAsync
            (UploadLargeFileRequest request, IProgress<ICopyProgress> progress, CancellationToken cancellationToken)
        {
            if (request.ContentLength < ClientOptions.MinimumPartSize)
                throw new ArgumentOutOfRangeException($"Argument must be a minimum of {ClientOptions.MinimumPartSize} bytes long.", nameof(request.ContentLength));

            List<string> sha1Hash = new List<string>();

            var parts = GetStreamParts(request.ContentStream, Options.UploadPartSize);
            if (parts.Count == 0)
                throw new ApiException($"The number of large file parts could not be determined from stream.");

            var fileRequest = new StartLargeFileRequest(request.BucketId, request.FileName)
            {
                ContentType = request.ContentType,
                FileInfo = request.FileInfo
            };

            fileRequest.FileInfo.SetLargeFileSha1(request.ContentStream.ToSha1());

            var fileResults = await StartLargeFileAsync(fileRequest, cancellationToken).ConfigureAwait(false);
            if (fileResults.IsSuccessStatusCode)
            {
                var urlRequest = new GetUploadPartUrlRequest(fileResults.Response.FileId);
                var urlResults = await GetUploadPartUrlAsync(urlRequest, default, cancellationToken).ConfigureAwait(false);
                if (fileResults.IsSuccessStatusCode)
                {
                    foreach (var part in parts)
                    {
                        var partStream = new PartialStream(request.ContentStream, part.Position, part.Length);
                        var partReqeust = new UploadPartRequest(urlResults.Response.UploadUrl, part.PartNumber, urlResults.Response.AuthorizationToken);

                        var partResults = await UploadPartAsync(partReqeust, partStream, progress, cancellationToken).ConfigureAwait(false);
                        if (partResults.IsSuccessStatusCode)
                        {
                            sha1Hash.Add(partResults.Response.ContentSha1);
                        }
                        else
                        {
                            return new ApiResults<UploadFileResponse>(partResults.HttpResponse, partResults.Error);
                        }
                    }
                }
                else
                {
                    return new ApiResults<UploadFileResponse>(fileResults.HttpResponse, fileResults.Error);
                }

                var finishRequest = new FinishLargeFileRequest(fileResults.Response.FileId, sha1Hash);
                var finishResults = await FinishLargeFileAsync(finishRequest, cancellationToken).ConfigureAwait(false);
                if (finishResults.IsSuccessStatusCode)
                {
                    var infoRequest = new GetFileInfoRequest(fileResults.Response.FileId);
                    var infoResults = await GetFileInfoAsync(infoRequest, cancellationToken).ConfigureAwait(false);
                    if (infoResults.IsSuccessStatusCode)
                    {
                        return finishResults;
                    }
                    else
                    {
                        return new ApiResults<UploadFileResponse>(infoResults.HttpResponse, infoResults.Error);
                    }
                }
                return new ApiResults<UploadFileResponse>(finishResults.HttpResponse, finishResults.Error);
            }
            return new ApiResults<UploadFileResponse>(fileResults.HttpResponse, fileResults.Error);
        }

        /// <summary>
        /// Gets the file parts to upload.
        /// </summary>
        /// <param name="content">The upload content stream.</param>
        /// <param name="partSize">The part size in bits.</param>
        private static HashSet<FileParts> GetStreamParts(Stream content, long partSize)
        {
            HashSet<FileParts> hashSet = new HashSet<FileParts>();

            long streamLength = (content.CanSeek ? content.Length : -1);

            if (streamLength == -1 || streamLength <= partSize)
                return hashSet;

            var parts = Math.Ceiling((double)streamLength / partSize);

            for (int i = 0; i < parts; i++)
            {
                hashSet.Add(
                    new FileParts()
                    {
                        PartNumber = i + 1,
                        Position = i * partSize,
                        Length = Math.Min(streamLength - (i * partSize), partSize)
                    }
                );
            }

            return hashSet;
        }

        /// <summary>
        /// Gets the content parts to download.
        /// </summary>
        /// <param name="contentLength">The download content length.</param>
        /// <param name="partSize">The part size in bits.</param>
        private static HashSet<FileParts> GetContentParts(long contentLength, long partSize)
        {
            HashSet<FileParts> hashSet = new HashSet<FileParts>();

            if (contentLength == -1 || contentLength <= partSize)
                return hashSet;

            var parts = Math.Ceiling((double)contentLength / partSize);

            for (int i = 0; i < parts; i++)
            {
                var partNumber = i + 1;
                var position = i * partSize;
                var length = Math.Min(contentLength - position, partSize);
                var rangeHeader = new RangeHeaderValue(position, position + length - 1);

                hashSet.Add(
                    new FileParts()
                    {
                        PartNumber = partNumber,
                        Position = position,
                        Length = length,
                        RangeHeader = rangeHeader
                    }
                );
            }

            return hashSet;
        }

        #endregion
    }
}