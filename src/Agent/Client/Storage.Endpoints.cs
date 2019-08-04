using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Security.Authentication;

using Bytewizer.Backblaze.Models;
using Bytewizer.Backblaze.Extensions;

namespace Bytewizer.Backblaze.Client
{
    public abstract partial class Storage : DisposableObject
    {
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

            using (var results = await _httpClient.SendAsync(httpRequest, cancellationToken))
            {
                return await HandleResultsAsync<AuthorizeAccountResponse>(results);
            }
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
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await _authPolicy.ExecuteAsync(async () =>
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
                        if (results.StatusCode == HttpStatusCode.Unauthorized)
                            throw new AuthenticationException("Authentication failed: Invalid key id or application key.");

                        if (results.StatusCode == HttpStatusCode.Forbidden)
                            throw new CapExceededExecption("Cap exceeded: Account cap exceeded or in bad standing.");

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
            });
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
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await _authPolicy.ExecuteAsync(async () =>
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
                        if (results.StatusCode == HttpStatusCode.Unauthorized)
                            throw new AuthenticationException("Authentication failed: Invalid key id or application key.");

                        if (results.StatusCode == HttpStatusCode.Forbidden)
                            throw new CapExceededExecption("Cap exceeded: Account cap exceeded or in bad standing.");

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
            });
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

            return await _authPolicy.ExecuteAsync(async () =>
            {

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
                        if (results.StatusCode == HttpStatusCode.Unauthorized)
                            throw new AuthenticationException("Authentication failed: Invalid key id or application key.");

                        if (results.StatusCode == HttpStatusCode.Forbidden)
                            throw new CapExceededExecption("Cap exceeded: Account cap exceeded or in bad standing.");

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
            });
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

            return await _authPolicy.ExecuteAsync(async () =>
            {
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
                        if (results.StatusCode == HttpStatusCode.Unauthorized)
                            throw new AuthenticationException("Authentication failed: Invalid key id or application key.");

                        if (results.StatusCode == HttpStatusCode.Forbidden)
                            throw new CapExceededExecption("Cap exceeded: Account cap exceeded or in bad standing.");

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
            });
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

            return await _authPolicy.ExecuteAsync(async () =>
            {
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
            });
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

            return await _authPolicy.ExecuteAsync(async () =>
            {
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
            });
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

            return await _authPolicy.ExecuteAsync(async () =>
            {
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
            });
        }

        #endregion

        #endregion
    }
}
