﻿using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Security.Authentication;

using Microsoft.Extensions.Logging;

using Bytewizer.Backblaze.Models;
using Bytewizer.Backblaze.Extensions;

using Bytewizer.Backblaze.Client.Internal;

namespace Bytewizer.Backblaze.Client
{
    public abstract partial class ApiRest : DisposableObject
    {
        #region Client Endpoints

        #region b2_authorize_account

        /// <summary>
        /// Authenticate to Backblaze B2 Cloud Storage.
        /// </summary>
        /// <param name="keyId">The identifier for the key.</param>
        /// <param name="applicationKey">The secret part of the key. You can use either the master application key or a normal application key.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        /// <returns>The <see cref="AuthorizeAccountResponse" /> of this <see cref="IApiResults{T}.Response"/> value, or <c>null</c>, if the response was was error data.</returns>
        public async Task<IApiResults<AuthorizeAccountResponse>> AuthorizeAccountAync
        (string keyId, string applicationKey, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(keyId))
                throw new ArgumentNullException(nameof(keyId));

            if (string.IsNullOrEmpty(applicationKey))
                throw new ArgumentNullException(nameof(applicationKey));

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{AccountInfo.AuthUrl}b2_authorize_account");

            httpRequest.Headers.Authorization = new BasicAuthentication(keyId, applicationKey);
            httpRequest.Headers.SetTestMode(Options.TestMode);

            using (var results = await _policy.InvokeClient.ExecuteAsync(async () =>
                { return await _httpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false); }))
            {
                var rawResult = await HandleResponseAsync<AuthorizeAccountResponseRaw>(results).ConfigureAwait(false);

                return new ApiResults<AuthorizeAccountResponse>(rawResult.HttpResponse, rawResult.Response.ToAuthorizedAccountResponse());
            }

        }

        #endregion

        #region b2_cancel_large_file

        /// <summary>
        /// Cancels the upload of a large file and deletes all parts that have been uploaded. 
        /// </summary>
        /// <param name="request">The <see cref="CancelLargeFileRequest"/> to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        public async Task<IApiResults<CancelLargeFileResponse>> CancelLargeFileAsync
            (CancelLargeFileRequest request, CancellationToken cancellationToken)
        {
            return await InvokePostAsync<CancelLargeFileRequest, CancelLargeFileResponse>
                (request, $"{AccountInfo.ApiUrl}b2_cancel_large_file", cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region b2_copy_file

        /// <summary>
        /// Creates a new file by copying from an existing file.
        /// </summary>
        /// <param name="request">The <see cref="CopyFileRequest"/> to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="CapExceededExecption">Thrown when a cap is exceeded or an account in bad standing.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        public async Task<IApiResults<CopyFileResponse>> CopyFileAsync
            (CopyFileRequest request, CancellationToken cancellationToken)
        {
            var results = await InvokePostAsync<CopyFileRequest, CopyFileResponse>
                (request, $"{AccountInfo.ApiUrl}b2_copy_file", cancellationToken).ConfigureAwait(false);

            _cache.Clear(CacheKeys.ListFileNames);
            _cache.Clear(CacheKeys.ListFileVersions);

            return results;
        }

        #endregion

        #region b2_copy_part

        /// <summary>
        /// Creates a new file part by copying from an existing file and storing it as a part of a large file which has already been started.
        /// </summary>
        /// <param name="request">The <see cref="CopyPartRequest"/> to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="CapExceededExecption">Thrown when a cap is exceeded or an account in bad standing.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        public async Task<IApiResults<CopyPartResponse>> CopyPartAsync
            (CopyPartRequest request, CancellationToken cancellationToken)
        {
            var results = await InvokePostAsync<CopyPartRequest, CopyPartResponse>
                (request, $"{AccountInfo.ApiUrl}b2_copy_part", cancellationToken).ConfigureAwait(false);

            _cache.Clear(CacheKeys.ListFileNames);
            _cache.Clear(CacheKeys.ListFileVersions);

            return results;
        }

        #endregion

        #region b2_create_bucket

        /// <summary>
        /// Creates a new bucket belonging to the specified account. 
        /// </summary>
        /// <param name="request">The <see cref="CreateBucketRequest"/> to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        public async Task<IApiResults<CreateBucketResponse>> CreateBucketAsync
            (CreateBucketRequest request, CancellationToken cancellationToken)
        {
            var results = await InvokePostAsync<CreateBucketRequest, CreateBucketResponse>
                (request, $"{AccountInfo.ApiUrl}b2_create_bucket", cancellationToken).ConfigureAwait(false);

            _cache.Clear(CacheKeys.ListBuckets);

            return results;
        }

        #endregion

        #region b2_create_key

        /// <summary>
        /// Creates a new application key. There is a limit of 100 million key creations per account.
        /// </summary>
        /// <param name="request">The <see cref="CreateKeyRequest"/> to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        public async Task<IApiResults<CreateKeyResponse>> CreateKeyAsync
            (CreateKeyRequest request, CancellationToken cancellationToken)
        {
            var results = await InvokePostAsync<CreateKeyRequest, CreateKeyResponse>
                (request, $"{AccountInfo.ApiUrl}b2_create_key", cancellationToken).ConfigureAwait(false);

            _cache.Clear(CacheKeys.ListKeys);

            return results;
        }

        #endregion

        #region b2_delete_bucket

        /// <summary>
        /// Deletes the bucket specified. Only buckets that contain no version of any files can be deleted. 
        /// </summary>
        /// <param name="request">The <see cref="DeleteBucketRequest"/> to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        public async Task<IApiResults<DeleteBucketResponse>> DeleteBucketAsync
            (DeleteBucketRequest request, CancellationToken cancellationToken)
        {
            var results = await InvokePostAsync<DeleteBucketRequest, DeleteBucketResponse>
                 (request, $"{AccountInfo.ApiUrl}b2_delete_bucket", cancellationToken).ConfigureAwait(false);

            _cache.Clear(CacheKeys.ListBuckets);

            return results;
        }

        #endregion

        #region b2_delete_file_version

        /// <summary>
        /// Deletes a specific version of a file. 
        /// </summary>
        /// <param name="request">The <see cref="DeleteFileVersionRequest"/> to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        public async Task<IApiResults<DeleteFileVersionResponse>> DeleteFileVersionAsync
            (DeleteFileVersionRequest request, CancellationToken cancellationToken)
        {
            var results = await InvokePostAsync<DeleteFileVersionRequest, DeleteFileVersionResponse>
                (request, $"{AccountInfo.ApiUrl}b2_delete_file_version", cancellationToken).ConfigureAwait(false);

            _cache.Clear(CacheKeys.ListFileVersions);

            return results;
        }

        #endregion

        #region b2_delete_key

        /// <summary>
        /// Deletes the application key specified. 
        /// </summary>
        /// <param name="request">The <see cref="DeleteKeyRequest"/> to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        public async Task<IApiResults<DeleteKeyResponse>> DeleteKeyAsync
            (DeleteKeyRequest request, CancellationToken cancellationToken)
        {
            var results = await InvokePostAsync<DeleteKeyRequest, DeleteKeyResponse>
                (request, $"{AccountInfo.ApiUrl}b2_delete_key", cancellationToken).ConfigureAwait(false);

            _cache.Clear(CacheKeys.ListKeys);

            return results;
        }

        #endregion

        #region b2_download_file_by_id

        /// <summary>
        /// Downloads a specific version of a file by file id without content.  
        /// </summary>
        /// <param name="request">The <see cref="DownloadFileByIdRequest"/> to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        public async Task<IApiResults<DownloadFileResponse>> DownloadFileByIdAsync
            (DownloadFileByIdRequest request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await DownloadFileByIdAsync(request, null, null, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Downloads a specific version of a file by file id.  
        /// </summary>
        /// <param name="request">The <see cref="DownloadFileByIdRequest"/> to send.</param>
        /// <param name="content">The download content to receive.</param>
        /// <param name="progress">A progress action which fires every time the write buffer is cycled.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="InvalidHashException">Thrown when a checksum hash is not valid.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        public async Task<IApiResults<DownloadFileResponse>> DownloadFileByIdAsync
            (DownloadFileByIdRequest request, Stream content, IProgress<ICopyProgress> progress, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{AccountInfo.DownloadUrl}b2api/v2/b2_download_file_by_id")
            {
                Content = CreateJsonContent(request)
            };

            httpRequest.Headers.SetAuthorization(request.AuthorizationToken, AuthToken.Authorization);
            httpRequest.Headers.SetRange(request.Range);
            httpRequest.Headers.SetTestMode(Options.TestMode);

            httpRequest.Content.Headers.SetContentDisposition(request);

            if (content == null)
            {
                using (var response = await _policy.InvokeClient.ExecuteAsync(async () =>
                    { return await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false); }))
                {
                    return await HandleResponseAsync(response, content);
                }
            }
            else
            {
                using (var response = await _policy.InvokeClient.ExecuteAsync(async () =>
                    { return await _httpClient.SendAsync(httpRequest, content, progress, cancellationToken).ConfigureAwait(false); }))
                {
                    return await HandleResponseAsync(response, content);
                }
            }
        }

        #endregion

        #region b2_download_file_by_name

        /// <summary>
        /// Downloads the most recent version of a file by name without content.
        /// </summary>
        /// <param name="request">The <see cref="DownloadFileByNameRequest"/> content to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        public async Task<IApiResults<DownloadFileResponse>> DownloadFileByNameAsync
            (DownloadFileByNameRequest request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await DownloadFileByNameAsync(request, null, null, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Downloads the most recent version of a file by name. 
        /// </summary>
        /// <param name="request">The <see cref="DownloadFileByNameRequest"/> to send.</param>
        /// <param name="content">The download content to receive.</param>
        /// <param name="progress">A progress action which fires every time the write buffer is cycled.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="InvalidHashException">Thrown when a checksum hash is not valid.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        public async Task<IApiResults<DownloadFileResponse>> DownloadFileByNameAsync
        (DownloadFileByNameRequest request, Stream content, IProgress<ICopyProgress> progress, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var httpRequest = new HttpRequestMessage
                (HttpMethod.Get, $"{AccountInfo.DownloadUrl}file/{request.BucketName}/{request.FileName}");

            httpRequest.Headers.SetAuthorization(request.AuthorizationToken, AuthToken.Authorization);
            httpRequest.Headers.SetRange(request.Range);
            httpRequest.Headers.SetTestMode(Options.TestMode);

            //TODO: Not sure if this is the best way to handle
            httpRequest.Content = new StringContent(string.Empty);
            httpRequest.Content.Headers.ContentDisposition(request);

            if (content == null)
            {
                using (var response = await _policy.InvokeClient.ExecuteAsync(async () =>
                    { return await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false); }))
                {
                    return await HandleResponseAsync(response, null);
                }
            }
            else
            {
                using (var response = await _policy.InvokeClient.ExecuteAsync(async () =>
                    { return await _httpClient.SendAsync(httpRequest, content, progress, cancellationToken).ConfigureAwait(false); }))
                {
                    return await HandleResponseAsync(response, content);
                }
            }
        }

        #endregion

        #region b2_finish_large_file

        /// <summary>
        /// Converts file parts that have been uploaded into a single file. 
        /// </summary>
        /// <param name="request">The <see cref="FinishLargeFileRequest"/> to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        public async Task<IApiResults<UploadFileResponse>> FinishLargeFileAsync
            (FinishLargeFileRequest request, CancellationToken cancellationToken)
        {
            var results = await InvokePostAsync<FinishLargeFileRequest, UploadFileResponse>
                (request, $"{AccountInfo.ApiUrl}b2_finish_large_file", cancellationToken).ConfigureAwait(false);

            _cache.Clear(CacheKeys.ListFileNames);
            _cache.Clear(CacheKeys.ListFileVersions);
            _cache.Clear(CacheKeys.ListUnfinished);

            return results;
        }

        #endregion

        #region b2_get_download_authorization

        /// <summary>
        /// Generate an authorization token that can be used to download files from a private bucket. 
        /// </summary>
        /// <param name="request">The <see cref="GetDownloadAuthorizationRequest"/> to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        public async Task<IApiResults<GetDownloadAuthorizationResponse>> GetDownloadAuthorizationAsync
            (GetDownloadAuthorizationRequest request, CancellationToken cancellationToken)
        {
            return await InvokePostAsync<GetDownloadAuthorizationRequest, GetDownloadAuthorizationResponse>
                (request, $"{AccountInfo.ApiUrl}b2_get_download_authorization", cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region b2_get_file_info

        /// <summary>
        /// Gets information about a file. 
        /// </summary>
        /// <param name="request">The <see cref="GetFileInfoRequest"/> to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        public async Task<IApiResults<GetFileInfoResponse>> GetFileInfoAsync
            (GetFileInfoRequest request, CancellationToken cancellationToken)
        {
            return await InvokePostAsync<GetFileInfoRequest, GetFileInfoResponse>
                (request, $"{AccountInfo.ApiUrl}b2_get_file_info", cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region b2_get_upload_part_url

        /// <summary>
        /// Gets a url for uploading parts of a large file. 
        /// </summary>
        /// <param name="request">The <see cref="GetUploadPartUrlRequest"/> to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        public async Task<IApiResults<GetUploadPartUrlResponse>> GetUploadPartUrlAsync
            (GetUploadPartUrlRequest request, CancellationToken cancellationToken)
        {
            return await GetUploadPartUrlAsync(request, TimeSpan.Zero, cancellationToken);
        }

        /// <summary>
        /// Gets a url for uploading parts of a large file from memory cache. 
        /// </summary>
        /// <param name="request">The <see cref="GetUploadPartUrlRequest"/> to send.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        public async Task<IApiResults<GetUploadPartUrlResponse>> GetUploadPartUrlAsync
            (GetUploadPartUrlRequest request, TimeSpan cacheTTL, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (_cache == null)
                cacheTTL = TimeSpan.Zero;

            if (cacheTTL <= TimeSpan.Zero)
            {
                return await InvokePostAsync<GetUploadPartUrlRequest, GetUploadPartUrlResponse>
                    (request, $"{AccountInfo.ApiUrl}b2_get_upload_part_url", cancellationToken).ConfigureAwait(false);
            }
            else
            {
                return await _cache.GetOrCreateAsync(request, async (entry) =>
                {
                    entry.AbsoluteExpirationRelativeToNow = cacheTTL;
                    return await GetUploadPartUrlAsync(request, cancellationToken).ConfigureAwait(false);
                });
            }
        }

        #endregion

        #region b2_get_upload_url

        /// <summary>
        /// Gets a url for uploading files. 
        /// </summary>
        /// <param name="request">The get <see cref="GetUploadUrlRequest"/> to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        public async Task<IApiResults<GetUploadUrlResponse>> GetUploadUrlAsync
            (GetUploadUrlRequest request, CancellationToken cancellationToken)
        {
            return await GetUploadUrlAsync(request, TimeSpan.Zero, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a url for uploading files from memory cache. 
        /// </summary>
        /// <param name="request">The get <see cref="GetUploadUrlRequest"/> to send.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        public async Task<IApiResults<GetUploadUrlResponse>> GetUploadUrlAsync
            (GetUploadUrlRequest request, TimeSpan cacheTTL, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (_cache == null)
                cacheTTL = TimeSpan.Zero;

            if (cacheTTL <= TimeSpan.Zero)
            {
                return await InvokePostAsync<GetUploadUrlRequest, GetUploadUrlResponse>
                    (request, $"{AccountInfo.ApiUrl}b2_get_upload_url", cancellationToken).ConfigureAwait(false);
            }
            else
            {
                return await _cache.GetOrCreateAsync(request, async (entry) =>
                {
                    entry.AbsoluteExpirationRelativeToNow = cacheTTL;
                    return await GetUploadUrlAsync(request, cancellationToken).ConfigureAwait(false);
                });
            }
        }

        #endregion

        #region b2_hide_file

        /// <summary>
        /// Hides a file so that <see cref="DownloadFileByNameAsync(DownloadFileByNameRequest, Stream, IProgress{ICopyProgress}, CancellationToken)"/> will not find the file but previous versions of the file are still stored.   
        /// </summary>
        /// <param name="request">The <see cref="HideFileRequest"/> content to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        public async Task<IApiResults<HideFileResponse>> HideFileAsync
            (HideFileRequest request, CancellationToken cancellationToken)
        {
            var results = await InvokePostAsync<HideFileRequest, HideFileResponse>
                (request, $"{AccountInfo.ApiUrl}b2_hide_file", cancellationToken).ConfigureAwait(false);

            _cache.Clear(CacheKeys.ListFileNames);

            return results;
        }

        #endregion

        #region b2_list_buckets

        /// <summary>
        /// List buckets associated with an account in alphabetical order by bucket name. When using an authorization token
        /// that is restricted to a bucket you must include the <see cref="ListBucketsRequest.BucketId"/>
        /// or <see cref="ListBucketsRequest.BucketName"/> of that bucket in the request or the request will be denied. 
        /// </summary>
        /// <param name="request">The <see cref="ListBucketsRequest"/> to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        public async Task<IApiResults<ListBucketsResponse>> ListBucketsAsync
            (ListBucketsRequest request, CancellationToken cancellationToken)
        {
            return await ListBucketsAsync(request, TimeSpan.Zero, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// List buckets from memory cache associated with an account in alphabetical order by bucket name. When using an authorization token
        /// that is restricted to a bucket you must include the <see cref="ListBucketsRequest.BucketId"/>
        /// or <see cref="ListBucketsRequest.BucketName"/> of that bucket in the request or the request will be denied. 
        /// </summary>
        /// <param name="request">The <see cref="ListBucketsRequest"/> to send.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        public async Task<IApiResults<ListBucketsResponse>> ListBucketsAsync
            (ListBucketsRequest request, TimeSpan cacheTTL, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (_cache == null)
                cacheTTL = TimeSpan.Zero;

            if (cacheTTL <= TimeSpan.Zero)
            {
                return await InvokePostAsync<ListBucketsRequest, ListBucketsResponse>
                    (request, $"{AccountInfo.ApiUrl}b2_list_buckets", cancellationToken).ConfigureAwait(false);
            }
            else
            {
                return await _cache.GetOrCreateAsync(request, async (entry) =>
                {
                    entry.AbsoluteExpirationRelativeToNow = cacheTTL;
                    return await ListBucketsAsync(request, cancellationToken).ConfigureAwait(false);
                });
            }
        }

        #endregion

        #region b2_list_file_names

        /// <summary>
        /// List the names of all files in a bucket starting at a given name. 
        /// </summary>
        /// <param name="request">The <see cref="ListFileNamesRequest"/> to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        public async Task<IApiResults<ListFileNamesResponse>> ListFileNamesAsync
            (ListFileNamesRequest request, CancellationToken cancellationToken)
        {
            return await ListFileNamesAsync(request, TimeSpan.Zero, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// List the names of all files in a bucket starting at a given name from memory cache. 
        /// </summary>
        /// <param name="request">The <see cref="ListFileNamesRequest"/> to send.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        public async Task<IApiResults<ListFileNamesResponse>> ListFileNamesAsync
            (ListFileNamesRequest request, TimeSpan cacheTTL, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (_cache == null)
                cacheTTL = TimeSpan.Zero;

            if (cacheTTL <= TimeSpan.Zero)
            {
                return await InvokePostAsync<ListFileNamesRequest, ListFileNamesResponse>
                    (request, $"{AccountInfo.ApiUrl}b2_list_file_names", cancellationToken).ConfigureAwait(false);
            }
            else
            {
                return await _cache.GetOrCreateAsync(request, async (entry) =>
                {
                    entry.AbsoluteExpirationRelativeToNow = cacheTTL;
                    return await ListFileNamesAsync(request, cancellationToken).ConfigureAwait(false);
                }).ConfigureAwait(false);
            }
        }

        #endregion

        #region b2_list_file_versions

        /// <summary>
        /// List all versions of the files contained in one bucket in alphabetical order by file name
        /// and by reverse of date/time uploaded for versions of files with the same name. 
        /// </summary>
        /// <param name="request">The <see cref="ListFileVersionRequest"/> to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        public async Task<IApiResults<ListFileVersionResponse>> ListFileVersionsAsync
            (ListFileVersionRequest request, CancellationToken cancellationToken)
        {
            return await ListFileVersionsAsync(request, TimeSpan.Zero, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// List all versions of the files contained in one bucket in alphabetical order by file name
        /// and by reverse of date/time uploaded for versions of files with the same name. 
        /// </summary>
        /// <param name="request">The <see cref="ListFileVersionRequest"/> to send.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        public async Task<IApiResults<ListFileVersionResponse>> ListFileVersionsAsync
            (ListFileVersionRequest request, TimeSpan cacheTTL, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (_cache == null)
                cacheTTL = TimeSpan.Zero;

            if (cacheTTL <= TimeSpan.Zero)
            {
                return await InvokePostAsync<ListFileVersionRequest, ListFileVersionResponse>
                    (request, $"{AccountInfo.ApiUrl}b2_list_file_versions", cancellationToken).ConfigureAwait(false);
            }
            else
            {
                return await _cache.GetOrCreateAsync(request, async (entry) =>
                {
                    entry.AbsoluteExpirationRelativeToNow = cacheTTL;
                    return await ListFileVersionsAsync(request, cancellationToken).ConfigureAwait(false);
                }).ConfigureAwait(false);
            }
        }

        #endregion

        #region b2_list_keys

        /// <summary>
        /// List application keys associated with an account. 
        /// </summary>
        /// <param name="request">The <see cref="ListKeysRequest"/> to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        public async Task<IApiResults<ListKeysResponse>> ListKeysAsync
            (ListKeysRequest request, CancellationToken cancellationToken)
        {
            return await ListKeysAsync(request, TimeSpan.Zero, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// List application keys associated with an account from memory cache. 
        /// </summary>
        /// <param name="request">The <see cref="ListKeysRequest"/> to send.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        public async Task<IApiResults<ListKeysResponse>> ListKeysAsync
            (ListKeysRequest request, TimeSpan cacheTTL, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (_cache == null)
                cacheTTL = TimeSpan.Zero;

            if (cacheTTL <= TimeSpan.Zero)
            {
                return await _policy.InvokePolicy.ExecuteAsync(async () =>
                {
                    return await InvokePostAsync<ListKeysRequest, ListKeysResponse>
                    (request, $"{AccountInfo.ApiUrl}b2_list_keys", cancellationToken).ConfigureAwait(false);
                });
            }
            else
            {
                return await _cache.GetOrCreateAsync(request, async (entry) =>
                {
                    entry.AbsoluteExpirationRelativeToNow = cacheTTL;
                    return await ListKeysAsync(request, cancellationToken).ConfigureAwait(false);
                }).ConfigureAwait(false);
            }
        }

        #endregion

        #region b2_list_parts

        /// <summary>
        /// List parts that have been uploaded for a large file that has not been finished yet. 
        /// </summary>
        /// <param name="request">The <see cref="ListPartsRequest"/> to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        public async Task<IApiResults<ListPartsResponse>> ListPartsAsync
            (ListPartsRequest request, CancellationToken cancellationToken)
        {
            return await ListPartsAsync(request, TimeSpan.Zero, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// List parts that have been uploaded for a large file that has not been finished yet from memory cache. 
        /// </summary>
        /// <param name="request">The <see cref="ListPartsRequest"/> to send.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        public async Task<IApiResults<ListPartsResponse>> ListPartsAsync
            (ListPartsRequest request, TimeSpan cacheTTL, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (_cache == null)
                cacheTTL = TimeSpan.Zero;

            if (cacheTTL <= TimeSpan.Zero)
            {
                return await InvokePostAsync<ListPartsRequest, ListPartsResponse>
                    (request, $"{AccountInfo.ApiUrl}b2_list_parts", cancellationToken).ConfigureAwait(false);
            }
            else
            {
                return await _cache.GetOrCreateAsync(request, async (entry) =>
                {
                    entry.AbsoluteExpirationRelativeToNow = cacheTTL;
                    return await ListPartsAsync(request, cancellationToken).ConfigureAwait(false);
                }).ConfigureAwait(false);
            }
        }

        #endregion

        #region b2_list_unfinished_large_files

        /// <summary>
        /// List information about large file uploads that have been started but have not been finished or canceled.
        /// </summary>
        /// <param name="request">The <see cref="ListUnfinishedLargeFilesRequest"/> to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        public async Task<IApiResults<ListUnfinishedLargeFilesResponse>> ListUnfinishedLargeFilesAsync
            (ListUnfinishedLargeFilesRequest request, CancellationToken cancellationToken)
        {
            return await ListUnfinishedLargeFilesAsync(request, TimeSpan.Zero, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// List information about large file uploads that have been started but have not been finished or canceled from memory cache.
        /// </summary>
        /// <param name="request">The <see cref="ListUnfinishedLargeFilesRequest"/> to send.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        public async Task<IApiResults<ListUnfinishedLargeFilesResponse>> ListUnfinishedLargeFilesAsync
            (ListUnfinishedLargeFilesRequest request, TimeSpan cacheTTL, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (_cache == null)
                cacheTTL = TimeSpan.Zero;

            if (cacheTTL <= TimeSpan.Zero)
            {
                return await InvokePostAsync<ListUnfinishedLargeFilesRequest, ListUnfinishedLargeFilesResponse>
                    (request, $"{AccountInfo.ApiUrl}b2_list_unfinished_large_files", cancellationToken).ConfigureAwait(false);
            }
            else
            {
                return await _cache.GetOrCreateAsync(request, async (entry) =>
                {
                    entry.AbsoluteExpirationRelativeToNow = cacheTTL;
                    return await ListUnfinishedLargeFilesAsync(request, cancellationToken).ConfigureAwait(false);
                }).ConfigureAwait(false);
            }
        }

        #endregion

        #region b2_start_large_file

        /// <summary>
        /// Prepares for uploading parts of a large file. 
        /// </summary>
        /// <param name="request">The <see cref="StartLargeFileRequest"/> to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
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
            httpRequest.Headers.SetTestMode(Options.TestMode);

            using (var results = await _policy.InvokeClient.ExecuteAsync(async () =>
                { return await _httpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false); }))
            {
                var response = await HandleResponseAsync<StartLargeFileResponse>(results).ConfigureAwait(false);

                _cache.Clear(CacheKeys.ListUnfinished);

                return response;
            }
        }

        #endregion

        #region b2_update_bucket

        /// <summary>
        /// Update an existing bucket belonging to the specific account. 
        /// </summary>
        /// <param name="request">The <see cref="UpdateBucketRequest"/> to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        public async Task<IApiResults<UpdateBucketResponse>> UpdateBucketAsync
            (UpdateBucketRequest request, CancellationToken cancellationToken)
        {
            var results = await InvokePostAsync<UpdateBucketRequest, UpdateBucketResponse>
                (request, $"{AccountInfo.ApiUrl}b2_update_bucket", cancellationToken).ConfigureAwait(false);

            _cache.Clear(CacheKeys.ListBuckets);

            return results;
        }

        #endregion

        #region b2_upload_file

        /// <summary>
        /// Upload content stream to Backblaze B2 Cloud Storage. 
        /// </summary>
        /// <param name="request">The <see cref="UploadFileRequest"/> to send.</param>
        /// <param name="content"> The content stream of the content payload.</param>
        /// <param name="progress">A progress action which fires every time the write buffer is cycled.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="CapExceededExecption">Thrown when a cap is exceeded or an account in bad standing.</exception>
        /// <exception cref="InvalidHashException">Thrown when a checksum hash is not valid.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        public async Task<IApiResults<UploadFileResponse>> UploadFileAsync
            (UploadFileRequest request, Stream content, IProgress<ICopyProgress> progress, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var results = await _policy.InvokePolicy.ExecuteAsync(async () =>
           {
               var httpRequest = new HttpRequestMessage(HttpMethod.Post, request.UploadUrl);

               httpRequest.Headers.SetAuthorization(request.AuthorizationToken);
               httpRequest.Headers.SetBzInfo(request.FileInfo);
               httpRequest.Headers.SetTestMode(Options.TestMode);

               using (var httpContent = new ProgressStreamContent(content, progress, false))
               {
                   var hash = content.ToSha1();

                   httpRequest.Content = httpContent;
                   httpRequest.Content.Headers.ContentLength = content.Length;
                   httpRequest.Content.Headers.ContentSha1(hash);
                   httpRequest.Content.Headers.ContentType = new MediaTypeHeaderValue(request.ContentType);
                   httpRequest.Content.Headers.SetContentFileName(request.FileName);

                   using (var httpResponse = await _policy.InvokeClient.ExecuteAsync(async () =>
                       { return await _httpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false); }))
                   {
                       var response = await HandleResponseAsync<UploadFileResponse>(httpResponse).ConfigureAwait(false);

                       if (response.IsSuccessStatusCode)
                       {
                           if (hash.Equals(response.Response.ContentSha1))
                               return response;

                           if (response.Response.ContentSha1.Equals("none"))
                           {
                               var largeFileHash = response.Response.FileInfo.GetLargeFileSha1();
                               if (hash.Equals(largeFileHash))
                                   return response;
                           }

                           throw new InvalidHashException($"Response checksum failed: Hash verify on '{response.Response.FileName}' failed.");
                       }

                       return response;
                   }
               }
           }).ConfigureAwait(false);

            _cache.Clear(CacheKeys.ListFileNames);
            _cache.Clear(CacheKeys.ListFileVersions);

            return results;
        }

        #endregion

        #region b2_upload_part

        /// <summary>
        /// Uploads one part of a multi-part content stream using file id obtained from <see cref="StartLargeFileAsync"/>. 
        /// </summary>
        /// <param name="request">The <see cref="UploadPartRequest"/> to send.</param>
        /// <param name="content"> The content stream of the content payload.</param>
        /// <param name="progress">A progress action which fires every time the write buffer is cycled.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="CapExceededExecption">Thrown when a cap is exceeded or an account in bad standing.</exception>
        /// <exception cref="InvalidHashException">Thrown when a checksum hash is not valid.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        public async Task<IApiResults<UploadPartResponse>> UploadPartAsync
            (UploadPartRequest request, Stream content, IProgress<ICopyProgress> progress, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var results = await _policy.InvokePolicy.ExecuteAsync(async () =>
            {
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, request.UploadUrl);

                httpRequest.Headers.SetAuthorization(request.AuthorizationToken);
                httpRequest.Headers.SetPartNumber(request.PartNumber);
                httpRequest.Headers.SetTestMode(Options.TestMode);

                using (var httpContent = new ProgressStreamContent(content, progress, false))
                {
                    var hash = content.ToSha1();

                    httpRequest.Content = httpContent;
                    httpRequest.Content.Headers.ContentLength = content.Length;
                    httpRequest.Content.Headers.ContentSha1(hash);

                    using (var httpResponse = await _policy.InvokeClient.ExecuteAsync(async () =>
                        { return await _httpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false); }))
                    {
                        var response = await HandleResponseAsync<UploadPartResponse>(httpResponse).ConfigureAwait(false);
                        if (response.IsSuccessStatusCode)
                        {
                            if (hash.Equals(response.Response.ContentSha1))
                                return response;

                            throw new InvalidHashException($"Response checksum failed: Hash verify on part {response.Response.PartNumber} file id '{response.Response.FileId}' failed.");
                        }

                        return response;
                    }
                }
            }).ConfigureAwait(false);

            _cache.Clear(CacheKeys.ListParts);

            return results;
        }

        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        /// Invokes a HTTP POST operation on Backblaze B2 Cloud Storage.
        /// </summary>
        /// <typeparam name="TRequest">Request resource type.</typeparam>
        /// <typeparam name="TResponse">Response resource type.</typeparam>
        /// <param name="content">Resource content to send.</param>
        /// <param name="url">Relative url to the resource.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        private async Task<IApiResults<TResponse>> InvokePostAsync<TRequest, TResponse>
            (TRequest content, string url, CancellationToken cancellationToken)
        where TRequest : IRequest where TResponse : IResponse
        {
            if (content == null)
                throw new ArgumentNullException(nameof(content));

            return await _policy.InvokePolicy.ExecuteAsync(async () =>
            {
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = CreateJsonContent(content)
                };

                httpRequest.Headers.SetAuthorization(AuthToken.Authorization);
                httpRequest.Headers.SetTestMode(Options.TestMode);

                using (var results = await _policy.InvokeClient.ExecuteAsync(async () =>
                    { return await _httpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false); }))
                {
                    return await HandleResponseAsync<TResponse>(results).ConfigureAwait(false);
                }
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Handle a HTTP response operation from Backblaze B2 Cloud Storage.
        /// </summary>
        /// <typeparam name="TResponse">Response resource type.</typeparam>
        /// <param name="response">A instance implementing <see cref="HttpResponseMessage"/>.</param>
        private async Task<IApiResults<TResponse>> HandleResponseAsync<TResponse>(HttpResponseMessage response)
            where TResponse : IResponse
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            if (response.IsSuccessStatusCode)
            {
                var results = await ReadAsJsonAsync<TResponse>(response).ConfigureAwait(false);
                return new ApiResults<TResponse>(response, results);
            }

            return await HandleErrorResponseAsync<TResponse>(response).ConfigureAwait(false);
        }

        /// <summary>
        /// Handle a HTTP response operation from Backblaze B2 Cloud Storage.
        /// </summary>
        /// <param name="response">A instance implementing <see cref="HttpResponseMessage"/>.</param>
        /// <param name="content">The download content to receive.</param>
        private async Task<IApiResults<DownloadFileResponse>> HandleResponseAsync(HttpResponseMessage response, Stream content)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            if (response.IsSuccessStatusCode)
            {
                var fileResponse = new DownloadFileResponse()
                {
                    ContentLength = response.Content.Headers.ContentLength.Value,
                    ContentType = response.Content.Headers.ContentType.MediaType,
                    CashControl = response.Headers.CacheControl,
                    ContentDisposition = response.Content.Headers.ContentDisposition,
                };

                response.Headers.GetBzInfo(fileResponse);

                //TODO VerifyDownloadHash(fileResponse, content); 

                return new ApiResults<DownloadFileResponse>(response, fileResponse);
            }

            return await HandleErrorResponseAsync<DownloadFileResponse>(response).ConfigureAwait(false);
        }

        /// <summary>
        /// Verify Sha1 response hash to stream.
        /// </summary>
        /// <param name="response">Downloaded response.</param>
        /// <param name="content">Downloaded stream </param>
        private void VerifyDownloadHash(DownloadFileResponse response, Stream content)
        {
            if (content.CompareTo(string.Empty))
                return;

            if (content.CompareTo(response.ContentSha1))
                return;

            if (response.ContentSha1.Equals("none"))
            {
                var largeFileHash = response.FileInfo.GetLargeFileSha1();
                if (content.CompareTo(largeFileHash))
                    return;
            }

            throw new InvalidHashException($"Response checksum failed: Hash verify on '{response.FileName}' failed.");
        }

        /// <summary>
        /// Handle a HTTP error response operation from Backblaze B2 Cloud Storage.
        /// </summary>
        /// <param name="response">A instance implementing <see cref="HttpResponseMessage"/>.</param>
        private async Task<IApiResults<TResponse>> HandleErrorResponseAsync<TResponse>(HttpResponseMessage response)
        where TResponse : IResponse
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new AuthenticationException("Authentication failed: Invalid or expired token.");

            if (response.StatusCode == HttpStatusCode.Forbidden)
                throw new CapExceededExecption("Cap exceeded: Account cap exceeded or in bad standing.");

            var error = await ReadAsJsonAsync<ErrorResponse>(response).ConfigureAwait(false);
            _logger.LogError($"Response error code:{error.Code} status:{error.Status} message:{error.Message}");

            return new ApiResults<TResponse>(response, error);
        }

        /// <summary>
        /// Creates HTTP content from serialized json object. 
        /// </summary>
        /// <typeparam name="T">Type to serialize.</typeparam>
        /// <param name="value">The string value (payload) to include.</param>
        private HttpContent CreateJsonContent<T>(T value)
        {
            var json = JsonSerializer.SerializeObject(value).ToString();

            _logger.LogTrace($"Sending client JSON request:{Environment.NewLine}{json}");
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        /// <summary>
        /// Gets the serialized json object from HTTP content with a media type of "application/json"
        /// </summary>
        /// <typeparam name="T">Type to return.</typeparam>
        /// <param name="response">A instance implementing <see cref="HttpResponseMessage"/>.</param>
        private async Task<T> ReadAsJsonAsync<T>(HttpResponseMessage response)
        {
            var mediaType = response.Content.Headers.ContentType?.MediaType;

            if (!string.Equals(mediaType, "application/json", StringComparison.OrdinalIgnoreCase))
                throw new ApiException($"Invalid content type: Content header '{mediaType}' is an invalid media type.");

            string json = await response.Content?.ReadAsStringAsync();
            _logger.LogTrace($"Received client JSON response:{Environment.NewLine}{json}");

            return JsonSerializer.DeserializeObject<T>(json);
        }

        #endregion
    }
}