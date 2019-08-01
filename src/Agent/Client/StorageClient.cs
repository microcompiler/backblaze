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

using Bytewizer.Backblaze.Models;
using Bytewizer.Backblaze.Extensions;

namespace Bytewizer.Backblaze.Client
{
    /// <summary>
    /// Represents a base implementation which uses <see cref="HttpClient"/> for making HTTP requests.
    /// </summary>
    public abstract class StorageClient : DisposableObject
    {
        //TODO: Multithreading uploads for large file parts.
        //TODO: Multithreading downloads for files larger the 200MB.

        #region Constants

        /// <summary>
        /// Kilobyte
        /// </summary>
        public const ulong KB = 0x400;

        /// <summary>
        /// Megabyte
        /// </summary>
        public const ulong MB = 0x100000;

        /// <summary>
        /// Gigabyte
        /// </summary>
        public const ulong GB = 0x40000000;

        /// <summary>
        /// Terabyte
        /// </summary>
        public const ulong TB = 0x10000000000;

        /// <summary>
        /// Minimum large file size.
        /// </summary>
        public const long MinimumLargeFileSize = (long)(5 * MB);

        /// <summary>
        /// Maxium large file size.
        /// </summary>
        public const long MaximumLargeFileSize = (long)(10 * TB);

        /// <summary>
        /// Default download cutoff size for switching to chunked parts in bits.
        /// </summary>
        public const long DefaultDownloadCutoffSize = (long)(200 * MB);

        #endregion

        #region Lifetime

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageClient"/> class.
        /// </summary>
        public StorageClient() : this(null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageClient"/> class.
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient"/> used for making requests.</param>
        public StorageClient(HttpClient httpClient)
        {
            _httpClient = httpClient ?? new HttpClient();
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

        #endregion

        #region Public Methods

        #region Authorize Account

        /// <summary>
        /// Connect to the Backblaze B2 API server and initialize account settings.
        /// </summary>
        /// <param name="applicationKeyId">The identifier for the key.</param>
        /// <param name="applicationKey">The secret part of the key.</param>
        public void Connect(string applicationKeyId, string applicationKey)
        {
            ConnectAsync(applicationKeyId, applicationKey).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Connect to the Backblaze B2 API server and initialize account settings.
        /// </summary>
        /// <param name="applicationKeyId">The identifier for the key.</param>
        /// <param name="applicationKey">The secret part of the key.</param>
        public async Task ConnectAsync(string applicationKeyId, string applicationKey)
        {
            var results = await AuthorizeAccountAync(applicationKeyId, applicationKey, CancellationToken.None);
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
            else
            {
                throw new AuthException(results.StatusCode, results.Error);
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
                if (fileResults.Response.ContentLength < GetCutoffSize(DownloadCutoffSize, DownloadPartSize))
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
                var tom = GetCutoffSize(DownloadCutoffSize, DownloadPartSize);
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

        #region Client Endpoints

        #region b2_authorize_account

        /// <summary>
        /// Used to log in to the Backblaze B2 API Service. Returns an <see cref="AuthToken"/> that can be used for 
        /// account-level operations, and <see cref="ApiUrl"/> that should be used as the base url for subsequent API calls.
        /// </summary>
        /// <param name="accountId">The identifier for the key.</param>
        /// <param name="applicationKey">The secret part of the key.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        public async Task<IApiResults<AuthorizeAccountResponse>> AuthorizeAccountAync
        (string applicationKeyId, string applicationKey, CancellationToken cancellationToken)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{AccountInfo.AuthUrl}b2_authorize_account");

            httpRequest.Headers.Authorization = new BasicAuthenticationHeaderValue(applicationKeyId, applicationKey);
            httpRequest.Headers.SetTestMode(TestMode);

            var results = await _httpClient.SendAsync(httpRequest, cancellationToken);
            return await HandleResultsAsync<AuthorizeAccountResponse>(results);
        }

        #endregion

        #region b2_cancel_large_file

        /// <summary>
        /// Cancels the upload of a large file and deletes all of the parts that have been uploaded. 
        /// </summary>
        /// <param name="request">The create bucket request content to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        public async Task<IApiResults<CancelLargeFileResponse>> CancelLargeFileAsync
            (CancelLargeFileRequest request, CancellationToken cancellationToken)
        {
            return await InvokePostAsync<CancelLargeFileRequest, CancelLargeFileResponse>
                (request, $"{AccountInfo.ApiUrl}b2_cancel_large_file", cancellationToken);
        }

        #endregion

        // TODO: b2_copy_file - IN BETA
        // TODO: b2_copy_part - IN BETA

        #region b2_create_bucket

        /// <summary>
        /// Creates a new bucket belonging to the specified account. 
        /// </summary>
        /// <param name="request">The create bucket request content to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        public async Task<IApiResults<CreateBucketResponse>> CreateBucketAsync
            (CreateBucketRequest request, CancellationToken cancellationToken)
        {
            return await InvokePostAsync<CreateBucketRequest, CreateBucketResponse>
                (request, $"{AccountInfo.ApiUrl}b2_create_bucket", cancellationToken);
        }

        #endregion

        #region b2_create_key

        /// <summary>
        /// Creates a new application key. There is a limit of 100 million key creations per account.
        /// </summary>
        /// <param name="request">The create application key request content to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        public async Task<IApiResults<CreateKeyResponse>> CreateKeyAsync
            (CreateKeyRequest request, CancellationToken cancellationToken)
        {
            return await InvokePostAsync<CreateKeyRequest, CreateKeyResponse>
                (request, $"{AccountInfo.ApiUrl}b2_create_key", cancellationToken);
        }

        #endregion

        #region b2_delete_bucket

        /// <summary>
        /// Deletes the bucket specified. Only buckets that contain no version of any files can be deleted. 
        /// </summary>
        /// <param name="request">The delete bucket request content to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        public async Task<IApiResults<DeleteBucketResponse>> DeleteBucketAsync
            (DeleteBucketRequest request, CancellationToken cancellationToken)
        {
            return await InvokePostAsync<DeleteBucketRequest, DeleteBucketResponse>
                (request, $"{AccountInfo.ApiUrl}b2_delete_bucket", cancellationToken);
        }

        #endregion

        #region b2_delete_file_version

        /// <summary>
        /// Deletes a specific version of a file. 
        /// </summary>
        /// <param name="request">The delete file version request content to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        public async Task<IApiResults<DeleteFileVersionResponse>> DeleteFileVersionAsync
            (DeleteFileVersionRequest request, CancellationToken cancellationToken)
        {
            return await InvokePostAsync<DeleteFileVersionRequest, DeleteFileVersionResponse>
                (request, $"{AccountInfo.ApiUrl}b2_delete_file_version", cancellationToken);
        }

        #endregion

        #region b2_delete_key

        /// <summary>
        /// Deletes the application key specified. 
        /// </summary>
        /// <param name="request">The delete key request content to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        public async Task<IApiResults<DeleteKeyResponse>> DeleteKeyAsync
            (DeleteKeyRequest request, CancellationToken cancellationToken)
        {
            return await InvokePostAsync<DeleteKeyRequest, DeleteKeyResponse>
                (request, $"{AccountInfo.ApiUrl}b2_delete_key", cancellationToken);
        }

        #endregion

        #region b2_download_file_by_id

        /// <summary>
        /// Downloads a specific version of a file by file id without content.  
        /// </summary>
        /// <param name="request">The download file request content to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        public async Task<IApiResults<DownloadFileResponse>> DownloadFileByIdAsync
            (DownloadFileByIdRequest request, CancellationToken cancellationToken)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{AccountInfo.DownloadUrl}b2api/v2/b2_download_file_by_id")
            {
                Content = CreateJsonContent(request)
            };

            if (string.IsNullOrWhiteSpace(request.Authorization))
                httpRequest.Headers.SetAuthorization(AuthToken.Authorization);

            httpRequest.Headers.SetAuthorization(request.Authorization);
            httpRequest.Headers.SetRange(request.Range);
            httpRequest.Headers.SetTestMode(TestMode);

            //TODO: Where should his be included?
            //httpRequest.Headers.SetContentDisposition(request.ContentDisposition);
            httpRequest.Content.Headers.SetContentDisposition(request.ContentDisposition);

            using (var results = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
            {
                if (!results.IsSuccessStatusCode)
                {
                    var error = await ReadAsJsonAsync<ErrorResponse>(results);
                    return new ApiResults<DownloadFileResponse>(results, error);
                }

                var fileResponse = new DownloadFileResponse()
                {
                    CashControl = results.Headers.CacheControl,
                    ContentLength = results.Content.Headers.ContentLength.Value,
                    ContentType = results.Content.Headers.ContentType.MediaType,
                    ContentDisposition = results.Content.Headers.ContentDisposition,
                };

                results.Headers.GetBzInfo(fileResponse);

                return new ApiResults<DownloadFileResponse>(results, fileResponse);
            }
        }

        /// <summary>
        /// Downloads a specific version of a file by file id.  
        /// </summary>
        /// <param name="request">The download file request content to send.</param>
        /// <param name="content">The download content to receive.</param>
        /// <param name="progress">A progress action which fires every time the write buffer is cycled.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        public async Task<IApiResults<DownloadFileResponse>> DownloadFileByIdAsync
            (DownloadFileByIdRequest request, Stream content, IProgress<ICopyProgress> progress, CancellationToken cancellationToken)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{AccountInfo.DownloadUrl}b2api/v2/b2_download_file_by_id")
            {
                Content = CreateJsonContent(request)
            };

            if (string.IsNullOrWhiteSpace(request.Authorization))
                httpRequest.Headers.SetAuthorization(AuthToken.Authorization);

            httpRequest.Headers.SetAuthorization(request.Authorization);
            httpRequest.Headers.SetRange(request.Range);
            httpRequest.Headers.SetTestMode(TestMode);
            httpRequest.Content.Headers.SetContentDisposition(request);

            using (var results = await _httpClient.SendAsync(httpRequest, content, progress, cancellationToken))
            {

                if (!results.IsSuccessStatusCode)
                {
                    var error = await ReadAsJsonAsync<ErrorResponse>(results);
                    return new ApiResults<DownloadFileResponse>(results, error);
                }

                var fileResponse = new DownloadFileResponse()
                {
                    ContentLength = results.Content.Headers.ContentLength.Value,
                    ContentType = results.Content.Headers.ContentType.MediaType,
                    CashControl = results.Headers.CacheControl,
                    ContentDisposition = results.Content.Headers.ContentDisposition,
                };

                results.Headers.GetBzInfo(fileResponse);

                return new ApiResults<DownloadFileResponse>(results, fileResponse);
            }
        }

        #endregion

        #region b2_download_file_by_name

        /// <summary>
        /// Downloads the most recent version of a file without content. 
        /// </summary>
        /// <param name="request">The download file request content to send.</param>
        /// <param name="content">The download content to receive.</param>
        /// <param name="progress">A progress action which fires every time the write buffer is cycled.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        public async Task<IApiResults<DownloadFileResponse>> DownloadFileByNameAsync
            (DownloadFileByNameRequest request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var httpRequest = new HttpRequestMessage
               (HttpMethod.Get, $"{AccountInfo.DownloadUrl}file/{request.BucketName}/{request.FileName}");

            if (string.IsNullOrWhiteSpace(request.AuthorizationToken))
                httpRequest.Headers.SetAuthorization(AuthToken.Authorization);

            httpRequest.Headers.SetAuthorization(request.AuthorizationToken);
            httpRequest.Headers.SetRange(request.Range);
            httpRequest.Headers.SetTestMode(TestMode);

            //TODO: Not sure if this is the best way to handle
            httpRequest.Content = new StringContent(string.Empty);
            httpRequest.Content.Headers.ContentDisposition(request);

            using (var results = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
            {
                if (!results.IsSuccessStatusCode)
                {
                    var error = await ReadAsJsonAsync<ErrorResponse>(results);
                    return new ApiResults<DownloadFileResponse>(results, error);
                }

                var fileResponse = new DownloadFileResponse()
                {
                    ContentLength = results.Content.Headers.ContentLength.Value,
                    ContentType = results.Content.Headers.ContentType.MediaType,
                    CashControl = results.Headers.CacheControl,
                    ContentDisposition = results.Content.Headers.ContentDisposition,
                };

                results.Headers.GetBzInfo(fileResponse);

                return new ApiResults<DownloadFileResponse>(results, fileResponse);
            }
        }

        /// <summary>
        /// Downloads the most recent version of a file. 
        /// </summary>
        /// <param name="request">The download file request content to send.</param>
        /// <param name="content">The download content to receive.</param>
        /// <param name="progress">A progress action which fires every time the write buffer is cycled.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        public async Task<IApiResults<DownloadFileResponse>> DownloadFileByNameAsync
            (DownloadFileByNameRequest request, Stream content, IProgress<ICopyProgress> progress, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var httpRequest = new HttpRequestMessage
               (HttpMethod.Get, $"{AccountInfo.DownloadUrl}file/{request.BucketName}/{request.FileName}");

            if (string.IsNullOrWhiteSpace(request.AuthorizationToken))
                httpRequest.Headers.SetAuthorization(AuthToken.Authorization);

            httpRequest.Headers.SetAuthorization(request.AuthorizationToken);
            httpRequest.Headers.SetRange(request.Range);
            httpRequest.Headers.SetTestMode(TestMode);

            //TODO: Not sure if this is the best way to handle
            httpRequest.Content = new StringContent(string.Empty);
            httpRequest.Content.Headers.ContentDisposition(request);

            using (var results = await _httpClient.SendAsync(httpRequest, content, progress, cancellationToken))
            {
                if (!results.IsSuccessStatusCode)
                {
                    var error = await ReadAsJsonAsync<ErrorResponse>(results);
                    return new ApiResults<DownloadFileResponse>(results, error);
                }

                var fileResponse = new DownloadFileResponse()
                {
                    ContentLength = results.Content.Headers.ContentLength.Value,
                    ContentType = results.Content.Headers.ContentType.MediaType,
                    CashControl = results.Headers.CacheControl,
                    ContentDisposition = results.Content.Headers.ContentDisposition,
                };

                results.Headers.GetBzInfo(fileResponse);

                return new ApiResults<DownloadFileResponse>(results, fileResponse);
            }
        }

        #endregion

        #region b2_finish_large_file

        /// <summary>
        /// Converts the file parts that have been uploaded into a single file. 
        /// </summary>
        /// <param name="request">The delete key request content to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        public async Task<IApiResults<UploadFileResponse>> FinishLargeFileAsync
            (FinishLargeFileRequest request, CancellationToken cancellationToken)
        {
            return await InvokePostAsync<FinishLargeFileRequest, UploadFileResponse>
                (request, $"{AccountInfo.ApiUrl}b2_finish_large_file", cancellationToken);
        }

        #endregion

        #region b2_get_download_authorization

        /// <summary>
        /// Used to generate an authorization token that can be used to download files from a private bucket. 
        /// </summary>
        /// <param name="request">The get download authorization request content to send.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>An authorization token that can be passed to <see cref="DownloadFileByNameAsync"/>.</returns>
        public async Task<IApiResults<GetDownloadAuthorizationResponse>> GetDownloadAuthorizationAsync
            (GetDownloadAuthorizationRequest request, CancellationToken cancellationToken)
        {
            return await InvokePostAsync<GetDownloadAuthorizationRequest, GetDownloadAuthorizationResponse>
                (request, $"{AccountInfo.ApiUrl}b2_get_download_authorization", cancellationToken);
        }

        #endregion

        #region b2_get_file_info

        /// <summary>
        /// Gets information about a file. 
        /// </summary>
        /// <param name="request">The get file info request content to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A <see cref="IApiResults{T}.Error"/> if called on a non-existent file id or if called on an unfinished large file.</returns>
        public async Task<IApiResults<GetFileInfoResponse>> GetFileInfoAsync
            (GetFileInfoRequest request, CancellationToken cancellationToken)
        {
            return await InvokePostAsync<GetFileInfoRequest, GetFileInfoResponse>
                (request, $"{AccountInfo.ApiUrl}b2_get_file_info", cancellationToken);
        }
        ///<returns>An error if called on a non-existent file id or if called on an unfinished large file.</returns>

        #endregion

        #region b2_get_upload_part_url

        /// <summary>
        /// Gets a url to use for uploading multi-parts of a large file. 
        /// </summary>
        /// <param name="request">The get upload part url request content to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        public async Task<IApiResults<GetUploadPartUrlResponse>> GetUploadPartUrlAsync
            (GetUploadPartUrlRequest request, CancellationToken cancellationToken)
        {
            return await InvokePostAsync<GetUploadPartUrlRequest, GetUploadPartUrlResponse>
                (request, $"{AccountInfo.ApiUrl}b2_get_upload_part_url", cancellationToken);
        }

        #endregion

        #region b2_get_upload_url

        /// <summary>
        /// Gets an url to use for uploading files.  
        /// </summary>
        /// <param name="request">The get upload Url request content to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        public async Task<IApiResults<GetUploadUrlResponse>> GetUploadUrlAsync
        (GetUploadUrlRequest request, CancellationToken cancellationToken)
        {
            return await InvokePostAsync<GetUploadUrlRequest, GetUploadUrlResponse>
                (request, $"{AccountInfo.ApiUrl}b2_get_upload_url", cancellationToken);
        }

        #endregion

        #region b2_hide_file

        /// <summary>
        /// Hides a file so that downloading by name will not find the file but previous versions of the file are still stored.   
        /// </summary>
        /// <param name="request">The hide file request content to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        public async Task<IApiResults<HideFileResponse>> HideFileAsync
            (HideFileRequest request, CancellationToken cancellationToken)
        {
            return await InvokePostAsync<HideFileRequest, HideFileResponse>
                (request, $"{AccountInfo.ApiUrl}b2_hide_file", cancellationToken);
        }

        #endregion

        #region b2_list_buckets

        /// <summary>
        /// Lists buckets associated with an account in alphabetical order by bucket name. When using an authorization token
        /// that is restricted to a bucket you must include the <see cref="ListBucketsRequest.BucketId"/>
        /// or <see cref="ListBucketsRequest.BucketName"/> of that bucket in the request or the request will be denied. 
        /// </summary>
        /// <param name="request">The list buckets request content to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        public async Task<IApiResults<ListBucketsResponse>> ListBucketsAsync
            (ListBucketsRequest request, CancellationToken cancellationToken)
        {
            return await InvokePostAsync<ListBucketsRequest, ListBucketsResponse>
                (request, $"{AccountInfo.ApiUrl}b2_list_buckets", cancellationToken);
        }

        #endregion

        #region b2_list_file_names

        /// <summary>
        /// Lists the names of all files in a bucket starting at a given name. 
        /// </summary>
        /// <param name="request">The list file name request content to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        public async Task<IApiResults<ListFileNamesResponse>> ListFileNamesAsync
            (ListFileNamesRequest request, CancellationToken cancellationToken)
        {
            return await InvokePostAsync<ListFileNamesRequest, ListFileNamesResponse>
                (request, $"{AccountInfo.ApiUrl}b2_list_file_names", cancellationToken);
        }

        #endregion

        #region b2_list_file_versions

        /// <summary>
        /// Lists all of the versions of all of the files contained in one bucket, in alphabetical order by file name,
        /// and by reverse of date/time uploaded for versions of files with the same name. 
        /// </summary>
        /// <param name="request">The list buckets request content to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        public async Task<IApiResults<ListFileVersionResponse>> ListFileVersionsAsync
            (ListFileVersionRequest request, CancellationToken cancellationToken)
        {
            return await InvokePostAsync<ListFileVersionRequest, ListFileVersionResponse>
                (request, $"{AccountInfo.ApiUrl}b2_list_file_versions", cancellationToken);
        }

        #endregion

        #region b2_list_keys

        /// <summary>
        /// Lists application keys associated with an account. 
        /// </summary>
        /// <param name="request">The list application keys request content to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        public async Task<IApiResults<ListKeysResponse>> ListKeysAsync
            (ListKeysRequest request, CancellationToken cancellationToken)
        {
            return await InvokePostAsync<ListKeysRequest, ListKeysResponse>
                (request, $"{AccountInfo.ApiUrl}b2_list_keys", cancellationToken);
        }

        #endregion

        #region b2_list_parts

        /// <summary>
        /// Lists the parts that have been uploaded for a large file that has not been finished yet. This call returns at 
        /// most 1000 entries but it can be called repeatedly to scan through all of the parts for an upload. 
        /// </summary>
        /// <param name="request">The list parts request content to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        public async Task<IApiResults<ListPartsResponse>> ListPartsAsync
            (ListPartsRequest request, CancellationToken cancellationToken)
        {
            return await InvokePostAsync<ListPartsRequest, ListPartsResponse>
                (request, $"{AccountInfo.ApiUrl}b2_list_parts", cancellationToken);
        }

        #endregion

        #region b2_list_unfinished_large_files

        /// <summary>
        /// Lists information about large file uploads that have been started but have not been finished or canceled. 
        /// </summary>
        /// <param name="request">The list unfinished large files request content to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        public async Task<IApiResults<ListUnfinishedLargeFilesResponse>> ListUnfinishedLargeFilesAsync
            (ListUnfinishedLargeFilesRequest request, CancellationToken cancellationToken)
        {
            return await InvokePostAsync<ListUnfinishedLargeFilesRequest, ListUnfinishedLargeFilesResponse>
                (request, $"{AccountInfo.ApiUrl}b2_list_unfinished_large_files", cancellationToken);
        }

        #endregion

        #region b2_start_large_file

        /// <summary>
        /// Prepares for uploading the multi-parts of a large file. 
        /// </summary>
        /// <param name="request">The start large file request content to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        public async Task<IApiResults<StartLargeFileResponse>> StartLargeFileAsync
            (StartLargeFileRequest request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{AccountInfo.ApiUrl}b2_start_large_file")
            {
                Content = CreateJsonContent(request)
            };

            httpRequest.Headers.SetAuthorization(AuthToken.Authorization);
            httpRequest.Headers.SetBzInfo(request.FileInfo);
            httpRequest.Headers.SetTestMode(TestMode);

            using (var results = await _httpClient.SendAsync(httpRequest, cancellationToken))
            {
                return await HandleResultsAsync<StartLargeFileResponse>(results);
            }
        }

        #endregion

        #region b2_update_bucket

        /// <summary>
        /// Update an existing bucket. 
        /// </summary>
        /// <param name="request">The update bucket request content to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        public async Task<IApiResults<UpdateBucketResponse>> UpdateBucketAsync
            (UpdateBucketRequest request, CancellationToken cancellationToken)
        {
            return await InvokePostAsync<UpdateBucketRequest, UpdateBucketResponse>
                (request, $"{AccountInfo.ApiUrl}b2_update_bucket", cancellationToken);
        }

        #endregion

        #region b2_upload_file

        /// <summary>
        /// Uploads a file. 
        /// </summary>
        /// <param name="request">The upload file request content to send.</param>
        /// <param name="progress">A progress action which fires every time the write buffer is cycled.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        public async Task<IApiResults<UploadFileResponse>> UploadFileAsync
            (UploadFileRequest request, IProgress<ICopyProgress> progress, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, request.UploadUrl);

            httpRequest.Headers.SetAuthorization(request.AuthorizationToken);
            httpRequest.Headers.SetBzInfo(request.FileInfo);
            httpRequest.Headers.SetTestMode(TestMode);

            using (var httpContent = new ProgressStreamContent(request.ContentStream, progress, false))
            {
                httpRequest.Content = httpContent;
                httpRequest.Content.Headers.ContentLength = request.ContentLength;
                httpRequest.Content.Headers.ContentSha1(request.ContentSha1);
                httpRequest.Content.Headers.ContentType = new MediaTypeHeaderValue(request.ContentType);
                httpRequest.Content.Headers.SetContentFileName(request.FileName);

                using (var results = await _httpClient.SendAsync(httpRequest, cancellationToken))
                {
                    return await HandleResultsAsync<UploadFileResponse>(results);
                }
            }
        }

        #endregion

        #region b2_upload_part

        /// <summary>
        /// Uploads one part of a multi-part file using a file id obtained from <see cref="StartLargeFileAsync"/>. 
        /// </summary>
        /// <param name="request">The upload file request content to send.</param>
        /// <param name="progress">A progress action which fires every time the write buffer is cycled.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        public async Task<IApiResults<UploadPartResponse>> UploadPartAsync
            (UploadPartRequest request, IProgress<ICopyProgress> progress, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, request.UploadUrl);

            httpRequest.Headers.SetAuthorization(request.AuthorizationToken);
            httpRequest.Headers.SetPartNumber(request.PartNumber);
            httpRequest.Headers.SetTestMode(TestMode);

            using (var httpContent = new ProgressStreamContent(request.ContentStream, progress, false))
            {
                httpRequest.Content = httpContent;
                httpRequest.Content.Headers.ContentLength = request.ContentLength;
                httpRequest.Content.Headers.ContentSha1(request.ContentSha1);
                using (var results = await _httpClient.SendAsync(httpRequest, cancellationToken))
                {
                    return await HandleResultsAsync<UploadPartResponse>(results);
                }
            }
        }

        #endregion

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
            if (response == null) throw new ArgumentNullException(nameof(response));

            if (response.IsSuccessStatusCode)
            {
                var results = await ReadAsJsonAsync<TResponse>(response);
                return new ApiResults<TResponse>(response, results);
            }
            else
            {
                var error = await ReadAsJsonAsync<ErrorResponse>(response);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new AuthException(response.StatusCode, error);
                }

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
            var json = JsonSerializer.SerializeObject(value);
            Debug.WriteLine($"Request: {json}");
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
            Debug.WriteLine($"Response: {json}");
            var value = JsonSerializer.DeserializeObject<T>(json);
            return value;
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
                throw new ApiException($"Content header '{mediaType}' is an invalid media type.");
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
                response.FileInfo.TryGetValue("large_file_sha1", out string largeFileHash);
                if (string.Equals(largeFileHash, streamHash))
                    return;
            }

            throw new InvalidHashException($"Checksum failed on file id: '{response.FileId}'.");
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
            var parts = GetContentParts(results.Response.ContentLength, GetPartSize(DownloadPartSize));

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
            var parts = GetContentParts(results.Response.ContentLength, GetPartSize(DownloadPartSize));

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
            //IApiResults<UploadFileResponse> results = default;

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
        /// Gets the file parts to upload
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
        /// Gets the file parts to download
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

        #endregion
    }
}
