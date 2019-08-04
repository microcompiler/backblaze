using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Security.Authentication;

using Polly;

using Bytewizer.Backblaze.Models;
using Bytewizer.Backblaze.Extensions;
using Microsoft.Extensions.Logging;

namespace Bytewizer.Backblaze.Client
{
    /// <summary>
    /// Represents a base implementation which uses <see cref="HttpClient"/> for making HTTP requests.
    /// </summary>
    public abstract partial class Storage : DisposableObject
    {
        //TODO: Multithreading uploads/download for large file parts.

        #region Constants

        /// <summary>
        /// Kilobyte
        /// </summary>
        public const long KB = 0x400;

        /// <summary>
        /// Megabyte
        /// </summary>
        public const long MB = 0x100000;

        /// <summary>
        /// Gigabyte
        /// </summary>
        public const long GB = 0x40000000;

        /// <summary>
        /// Terabyte
        /// </summary>
        public const ulong TB = 0x10000000000;

        /// <summary>
        /// Minimum large file size in bits.
        /// </summary>
        public const long MinimumLargeFileSize = (5 * MB);

        /// <summary>
        /// Maximum large file size in bits.
        /// </summary>
        public const long MaximumLargeFileSize = (long)(10 * TB);

        /// <summary>
        /// Minimum large file part size in bits.
        /// </summary>
        public const long MinimumLargeFilePartSize = (5 * MB);

        /// <summary>
        /// Maximum large file part size in bits.
        /// </summary>
        public const long MaximumLargeFilePartSize = (5 * GB);

        /// <summary>
        /// Recommended large file part size in bits.
        /// </summary>
        public const long RecommendedLargeFilePartSize = (100 * MB);

        /// <summary>
        /// Default download cutoff size for switching to chunked parts in bits.
        /// </summary>
        public const long DefaultDownloadCutoffSize = (200 * MB);

        /// <summary>
        /// Default download part size in bits.
        /// </summary>
        public const long DefaultDownloadPartSize = (200 * MB);

        /// <summary>
        /// Default upload cutoff size for switching to chunked parts in bits.
        /// </summary>
        public const long DefaultUploadCutoffSize = (200 * MB);

        /// <summary>
        /// Default upload part size in bits.
        /// </summary>
        public const long DefaultUploadPartSize = (200 * MB);

        #endregion

        #region Lifetime

        /// <summary>
        /// Initializes a new instance of the <see cref="Storage"/> class.
        /// </summary>
        public Storage() : this(null, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Storage"/> class.
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient"/> used for making requests.</param>
        public Storage(HttpClient httpClient, ILogger<Storage> logger)
        {
            _httpClient = httpClient ?? new HttpClient();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

        #region Protected Fields

        /// <summary>
        /// <see cref="HttpClient"/> for making HTTP requests.
        /// </summary>
        protected readonly HttpClient _httpClient;

        /// <summary>
        /// Application logging
        /// </summary>
        protected readonly ILogger _logger;

        /// <summary>
        /// Authentication retry policy used when making HTTP requests.
        /// </summary>
        protected Policy _authPolicy;

        #endregion

        #region Public Properties

        /// <summary>
        /// Json serializer.
        /// </summary>
        internal JsonSerializer JsonSerializer { get; } = new JsonSerializer();

        /// <summary>
        /// The account information returned from the Backblaze B2 server.
        /// </summary>
        public AccountInfo AccountInfo { get; } = new AccountInfo();

        /// <summary>
        /// The authorization token to use with all calls other than <see cref="AuthorizeAccountAync"/>. 
        /// This authorization token is valid for at most 24 hours.
        /// </summary>
        public AuthToken AuthToken { get; private set; }

        /// <summary>
        /// This is for testing use only and not recomended for production environments. Sets "X-Bx-Test-Mode" headers used for debugging and testing.  
        /// Setting it to "fail_some_uploads", "expire_some_account_authorization_tokens" or "force_cap exceeded" will cause the
        /// server to return specific errors used for testing.
        /// </summary>
        public string TestMode { get; set; } = string.Empty;

        /// <summary>
        /// The number of times the client will retry failed authentication requests before timing out.
        /// </summary>
        public int RetryCount { get; set; } = 3;

        /// <summary>
        /// Upload cutoff size for switching to chunked parts in bits.
        /// </summary>
        public long UploadCutoffSize { get; set; } = DefaultUploadCutoffSize;

        /// <summary>
        /// Upload part size of chunked parts in bits.
        /// </summary>
        public long UploadPartSize { get; set; } = DefaultUploadPartSize;

        /// <summary>
        /// Download cutoff size for switching to chunked parts in bits.
        /// </summary>
        public long DownloadCutoffSize { get; set; } = DefaultDownloadCutoffSize;

        /// <summary>
        /// Download part size of chunked parts in bits.
        /// </summary>
        public long DownloadPartSize { get; set; } = DefaultDownloadPartSize;

        #endregion

        #region Public Methods

        #region Authorize Account

        /// <summary>
        /// Connect to Backblaze B2 Cloud storage and initialize account settings.
        /// </summary>
        /// <param name="keyId">The identifier for the key.</param>
        /// <param name="applicationKey">The secret part of the key.</param>
        public void Connect(string keyId, string applicationKey)
        {
            ConnectAsync(keyId, applicationKey).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Connect to Backblaze B2 Cloud storage and initialize account settings.
        /// </summary>
        /// <param name="keyId">The identifier for the key.</param>
        /// <param name="applicationKey">The secret part of the key.</param>
        public async Task ConnectAsync(string keyId, string applicationKey)
        {
            _authPolicy = Policy
                        .Handle<AuthenticationException>()
                        .WaitAndRetryAsync(RetryCount,
                            retryAttempt => GetSleepDuration(retryAttempt),
                            (exception, retryCount) => ConnectAsync(keyId, applicationKey));

            var results = await AuthorizeAccountAync(keyId, applicationKey, CancellationToken.None);
            if (results.IsSuccessStatusCode)
            {
                AuthToken = new AuthToken(results.Response.AuthorizationToken)
                {
                    Allowed = results.Response.Allowed
                };

                AccountInfo.ApiUrl = new Uri($"{results.Response.ApiUrl}b2api/v2/");
                AccountInfo.DownloadUrl = new Uri($"{results.Response.DownloadUrl}");
                AccountInfo.AccountId = results.Response.AccountId;
                AccountInfo.AbsoluteMinimumPartSize = results.Response.AbsoluteMinimumPartSize;
                AccountInfo.RecommendedPartSize = results.Response.RecommendedPartSize;
            }
        }

        #endregion

        #region Upload Stream

        /// <summary>
        /// Uploads file content by bucket id. 
        /// </summary>
        /// <param name="request">The upload file request content to send.</param>
        /// <param name="content">The upload content to receive.</param>
        /// <param name="progress">A progress action which fires every time the write buffer is cycled.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        public async Task<IApiResults<UploadFileResponse>> UploadAsync
            (UploadFileByBucketIdRequest request, Stream content, IProgress<ICopyProgress> progress, CancellationToken cancel)
        {
            if (content.Length < GetCutoffSize(UploadCutoffSize, UploadPartSize))
            {
                var urlRequest = new GetUploadUrlRequest(request.BucketId);
                var urlResults = await GetUploadUrlAsync(urlRequest, cancel);
                if (urlResults.IsSuccessStatusCode)
                {
                    var response = urlResults.Response;
                    var fileRequest = new UploadFileRequest(response.UploadUrl, content, request.FileName, response.AuthorizationToken)
                    {
                        ContentType = request.ContentType,
                        FileInfo = request.FileInfo
                    };

                    return await UploadFileAsync(fileRequest, progress, cancel);
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

                return await UploadLargeFileAsync(largeFileRequest, progress, cancel);
            }
        }

        #endregion

        #region Download Stream

        /// <summary>
        /// Downloads a specific version of content by file id. 
        /// </summary>
        /// <param name="request">The download file request to send.</param>
        /// <param name="content">The download content to receive.</param>
        /// <param name="progress">A progress action which fires every time the write buffer is cycled.</param>
        /// <param name="cancel">The cancellation token to cancel operation.</param>
        public async Task<IApiResults<DownloadFileResponse>> DownloadAsync
            (DownloadFileByIdRequest request, Stream content, IProgress<ICopyProgress> progress, CancellationToken cancel)
        {
            var fileRequest = new DownloadFileByIdRequest(request.FileId);
            var fileResults = await DownloadFileByIdAsync(fileRequest, cancel);
            if (fileResults.IsSuccessStatusCode)
            {
                if (fileResults.Response.ContentLength < DownloadCutoffSize)
                {
                    var results = await DownloadFileByIdAsync(request, content, progress, cancel);

                    if (results.IsSuccessStatusCode)
                    {
                        ValidateSha1Hash(results.Response, content);
                    }

                    return results;
                }
                else
                {
                    return await DownloadLargeFileAsync(fileRequest, fileResults, content, progress, cancel);
                }
            }

            return fileResults;
        }

        /// <summary>
        /// Downloads the most recent version of content by bucket and file name. 
        /// </summary>
        /// <param name="request">The download file request to send.</param>
        /// <param name="content">The download content to receive.</param>
        /// <param name="progress">A progress action which fires every time the write buffer is cycled.</param>
        /// <param name="cancel">The cancellation token to cancel operation.</param>
        public async Task<IApiResults<DownloadFileResponse>> DownloadAsync
            (DownloadFileByNameRequest request, Stream content, IProgress<ICopyProgress> progress, CancellationToken cancel)
        {
            var fileRequest = new DownloadFileByNameRequest(request.BucketName, request.FileName);
            var fileResults = await DownloadFileByNameAsync(fileRequest, cancel);
            if (fileResults.IsSuccessStatusCode)
            {
                if (fileResults.Response.ContentLength < GetCutoffSize(DownloadCutoffSize, DownloadPartSize))
                {
                    var results = await DownloadFileByNameAsync(request, content, progress, cancel);

                    if (results.IsSuccessStatusCode)
                    {
                        ValidateSha1Hash(results.Response, content);
                    }

                    return results;
                }
                else
                {
                    return await DownloadLargeFileAsync(fileRequest, fileResults, content, progress, cancel);
                }
            }

            return fileResults;
        }

        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        /// Invokes a HTTP POST operation on the Backblaze B2 server.
        /// </summary>
        /// <typeparam name="TRequest">Request resource type.</typeparam>
        /// <typeparam name="TResponse">Response resource type.</typeparam>
        /// <param name="content">Resource content to send.</param>
        /// <param name="url">Relative url to the resource.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A results object returned from the server.</returns>
        private async Task<IApiResults<TResponse>> InvokePostAsync<TRequest, TResponse>
        (TRequest content, string url, CancellationToken cancellationToken)
        where TRequest : IRequest
        where TResponse : IResponse
        {
            if (content == null) throw new ArgumentNullException(nameof(content));

            return await _authPolicy.ExecuteAsync(async () =>
            {
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = CreateJsonContent(content)
                };

                httpRequest.Headers.SetAuthorization(AuthToken.Authorization);
                httpRequest.Headers.SetTestMode(TestMode);

                using (var results = await _httpClient.SendAsync(httpRequest, cancellationToken))
                {
                    return await HandleResultsAsync<TResponse>(results);
                }
            });
        }

        /// <summary>
        /// Handel a HTTP response operation from the Backblaze B2 server.
        /// </summary>
        /// <typeparam name="TResponse">Response resource type.</typeparam>
        /// <param name="content">A instance implementing <see cref="HttpResponseMessage"/>.</param>
        /// <returns></returns>
        private async Task<IApiResults<TResponse>> HandleResultsAsync<TResponse>(HttpResponseMessage response)
            where TResponse : IResponse
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            if (response.IsSuccessStatusCode)
            {
                var results = await ReadAsJsonAsync<TResponse>(response);
                return new ApiResults<TResponse>(response, results);
            }
            else
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new AuthenticationException("Authentication failed: Invalid key id or application key.");

                if (response.StatusCode == HttpStatusCode.Forbidden)
                    throw new CapExceededExecption("Cap exceeded: Account cap exceeded or in bad standing.");

                var error = await ReadAsJsonAsync<ErrorResponse>(response);
                return new ApiResults<TResponse>(response, error);
            }
        }

        /// <summary>
        /// Creates HTTP content from serialized json object. 
        /// </summary>
        /// <typeparam name="T">Type to serialize.</typeparam>
        /// <param name="value">The string value (payload) to include.</param>
        private HttpContent CreateJsonContent<T>(T value)
        {
            var json = JsonSerializer.SerializeObject(value).ToString();
            _logger.LogTrace($"Sending B2 API Request: {json}");
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        /// <summary>
        /// Gets the serialized json object from HTTP content with a media type of "application/json"
        /// </summary>
        /// <typeparam name="T">Type to return.</typeparam>
        /// <param name="content">A instance implementing <see cref="HttpResponseMessage"/>.</param>
        private async Task<T> ReadAsJsonAsync<T>(HttpResponseMessage response)
        {
            string json = await ValidateAndGetString(response.Content);
            _logger.LogTrace($"Received B2 API Response: {json}");
            return JsonSerializer.DeserializeObject<T>(json);
        }

        /// <summary>
        /// Validate content type and read string.
        /// </summary>
        /// <param name="content">A instance implementing <see cref="HttpContent"/>.</param>
        private static Task<string> ValidateAndGetString(HttpContent content)
        {
            var mediaType = content.Headers.ContentType?.MediaType;
            if (!string.Equals(mediaType, "application/json", StringComparison.OrdinalIgnoreCase))
            {
                throw new ApiException($"Invalid content type: Content header '{mediaType}' is an invalid media type.");
            }
            return content.ReadAsStringAsync();
        }

        /// <summary>
        /// Validate Sha1 hash to stream.
        /// </summary>
        /// <param name="response">Downloaded response.</param>
        /// <param name="stream">Downloaded stream </param>
        private static void ValidateSha1Hash(DownloadFileResponse response, Stream stream)
        {
            var fileHash = response.ContentSha1;
            var streamHash = stream.ToSha1();

            if (string.Equals(fileHash, streamHash))
                return;

            if (string.Equals(fileHash, "none"))
            {
                var largeFileHash = response.FileInfo.GetLargeFileSha1();
                if (string.Equals(largeFileHash, streamHash))
                    return;
            }

            throw new InvalidHashException($"Checksum failed: Hash compare on file id: '{response.FileId}' failed.");
        }

        /// <summary>
        /// Downloads the most recent version of a large file in chunked parts. 
        /// </summary>
        /// <param name="request">The download file request content to send.</param>
        /// <param name="content">The download content to receive.</param>
        /// <param name="progress">A progress action which fires every time the write buffer is cycled.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        private async Task<IApiResults<DownloadFileResponse>> DownloadLargeFileAsync
            (DownloadFileByIdRequest request, IApiResults<DownloadFileResponse> results, Stream content, IProgress<ICopyProgress> progress, CancellationToken cancellationToken)
        {
            var parts = GetContentParts(results.Response.ContentLength, DownloadPartSize);

            foreach (var part in parts)
            {
                var mmultiStream = new MultiStream(content, part.Position, part.Length);
                var partReqeust = new DownloadFileByIdRequest(request.FileId)
                {
                    Range = new RangeHeaderValue(part.Position, part.Position + part.Length - 1)
                };

                var partResults = await DownloadFileByIdAsync(partReqeust, mmultiStream, progress, cancellationToken);
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
        /// <param name="request">The download file request content to send.</param>
        /// <param name="content">The download content to receive.</param>
        /// <param name="progress">A progress action which fires every time the write buffer is cycled.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        private async Task<IApiResults<DownloadFileResponse>> DownloadLargeFileAsync
            (DownloadFileByNameRequest request, IApiResults<DownloadFileResponse> results, Stream content, IProgress<ICopyProgress> progress, CancellationToken cancellationToken)
        {
            var parts = GetContentParts(results.Response.ContentLength, DownloadPartSize);

            foreach (var part in parts)
            {
                var mmultiStream = new MultiStream(content, part.Position, part.Length);
                var partReqeust = new DownloadFileByNameRequest(request.BucketName, request.FileName)
                {
                    Range = new RangeHeaderValue(part.Position, part.Position + part.Length - 1)
                };

                var partResults = await DownloadFileByNameAsync(partReqeust, mmultiStream, progress, cancellationToken);
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
        /// <param name="request">The upload file request content to send.</param>
        /// <param name="progress">A progress action which fires every time the write buffer is cycled.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        private async Task<IApiResults<UploadFileResponse>> UploadLargeFileAsync
            (UploadLargeFileRequest request, IProgress<ICopyProgress> progress, CancellationToken cancellationToken)
        {

            if (request.ContentLength < AccountInfo.AbsoluteMinimumPartSize)
                throw new ArgumentOutOfRangeException($"Argument must be a minimum of {AccountInfo.AbsoluteMinimumPartSize} bits long.", nameof(request.ContentLength));

            List<string> sha1Hash = new List<string>();

            var parts = GetStreamParts(request.ContentStream, GetPartSize(UploadPartSize));
            if (parts.Count == 0)
                throw new ApiException($"The number of large file parts could not be determined from stream.");

            var fileRequest = new StartLargeFileRequest(request.BucketId, request.FileName)
            {
                ContentType = request.ContentType,
                FileInfo = request.FileInfo
            };

            fileRequest.FileInfo.SetLargeFileSha1(request.ContentStream.ToSha1());

            var fileResults = await StartLargeFileAsync(fileRequest, cancellationToken);
            if (fileResults.IsSuccessStatusCode)
            {
                var urlRequest = new GetUploadPartUrlRequest(fileResults.Response.FileId);
                var urlResults = await GetUploadPartUrlAsync(urlRequest, cancellationToken);
                if (fileResults.IsSuccessStatusCode)
                {
                    foreach (var part in parts)
                    {
                        var partStream = new PartialStream(request.ContentStream, part.Position, part.Length);
                        var partReqeust = new UploadPartRequest(urlResults.Response.UploadUrl, partStream, part.PartNumber, urlResults.Response.AuthorizationToken);

                        var partResults = await UploadPartAsync(partReqeust, progress, cancellationToken);
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
                var finishResults = await FinishLargeFileAsync(finishRequest, cancellationToken);
                if (finishResults.IsSuccessStatusCode)
                {
                    var infoRequest = new GetFileInfoRequest(fileResults.Response.FileId);
                    var infoResults = await GetFileInfoAsync(infoRequest, cancellationToken);
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

            long parts = streamLength / partSize;

            for (int i = 0; i <= parts; i++)
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

            long parts = contentLength / partSize;

            for (int i = 0; i <= parts; i++)
            {
                hashSet.Add(
                    new FileParts()
                    {
                        PartNumber = i + 1,
                        Position = i * partSize,
                        Length = Math.Min(contentLength - (i * partSize), partSize)
                    }
                );
            }

            return hashSet;
        }

        /// <summary>
        /// Gets cutoff size in bits for switching to chunked parts upload.
        /// </summary>
        private double GetCutoffSize(long cutoff, long part)
        {
            double cutoffSize;
            if (cutoff == 0)
            {
                cutoffSize = GetPartSize(part);
            }
            else
            {
                if (cutoff <= AccountInfo.AbsoluteMinimumPartSize)
                {
                    cutoffSize = AccountInfo.AbsoluteMinimumPartSize;
                }
                else
                {
                    cutoffSize = cutoff;
                }
            }
            return cutoffSize;
        }

        /// <summary>
        /// Gets part size in bits of large file chunked parts.
        /// </summary>
        private long GetPartSize(long part)
        {
            long partSize;
            if (part == 0)
            {
                partSize = AccountInfo.RecommendedPartSize;
            }
            else
            {
                if (part < AccountInfo.AbsoluteMinimumPartSize)
                {
                    partSize = AccountInfo.AbsoluteMinimumPartSize;
                }
                else
                {
                    partSize = part;
                }
            }
            return partSize;
        }

        /// <summary>
        /// Get the duration to wait (exponential backoff) allowing an exponentially increasing wait time.
        /// </summary>
        /// <param name="retryAttempt">The retry attempt count.</param>
        public static TimeSpan GetSleepDuration(int retryAttempt)
        {
            Random jitterer = new Random();

            return TimeSpan.FromSeconds(Math.Pow(1, retryAttempt))
                    + TimeSpan.FromMilliseconds(jitterer.Next(0, 100));
        }

        #endregion
    }
}