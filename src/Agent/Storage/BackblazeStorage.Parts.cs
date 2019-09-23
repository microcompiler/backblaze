using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Authentication;

using Bytewizer.Backblaze.Client;
using Bytewizer.Backblaze.Models;
using Bytewizer.Backblaze.Adapters;

namespace Bytewizer.Backblaze.Storage
{
    /// <summary>
    /// Represents a default implementation of the <see cref="BackblazeStorage"/> which uses <see cref="ApiClient"/> for making HTTP requests.
    /// </summary>
    public partial class BackblazeStorage : IBackblazeParts
    {
        /// <summary>
        /// Provides methods to access large file part operations.
        /// </summary>
        public IBackblazeParts Parts { get { return this; } }

        #region ApiClient

        /// <summary>
        /// Cancels the upload of a large file and deletes all parts that have been uploaded. 
        /// </summary>
        /// <param name="fileId">The file id to cancel.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<CancelLargeFileResponse>> IBackblazeParts.CancelLargeFileAsync(string fileId)
        {
            var request = new CancelLargeFileRequest(fileId);
            return await _client.CancelLargeFileAsync(request, cancellationToken);
        }

        /// <summary>
        /// Creates a new file part by copying from an existing file and storing it as a part of a large file which has already been started.
        /// </summary>
        /// <param name="request">The <see cref="CopyPartRequest"/> to send.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="CapExceededExecption">Thrown when a cap is exceeded or an account in bad standing.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<CopyPartResponse>> IBackblazeParts.CopyAsync(CopyPartRequest request)
        {
            return await _client.CopyPartAsync(request, cancellationToken);
        }

        /// <summary>
        /// Creates a new file part by copying from an existing file and storing it as a part of a large file which has already been started.
        /// </summary>
        /// <param name="sourceFileId">The unique identifier of the source file being copied.</param>
        /// <param name="largeFileId">The unique identifier of the large file the part will belong to.</param>
        /// <param name="partNumber">The part number of the file.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="CapExceededExecption">Thrown when a cap is exceeded or an account in bad standing.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<CopyPartResponse>> IBackblazeParts.CopyAsync(string sourceFileId, string largeFileId, int partNumber)
        {
            var request = new CopyPartRequest(sourceFileId, largeFileId, partNumber);
            return await _client.CopyPartAsync(request, cancellationToken);
        }

        /// <summary>
        /// Converts file parts that have been uploaded into a single file. 
        /// </summary>
        /// <param name="fileId">The file id of the large file to finish.</param>
        /// <param name="sha1Parts">A list of hex SHA1 checksums for the parts of the large file.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<UploadFileResponse>> IBackblazeParts.FinishLargeFileAsync(string fileId, List<string> sha1Parts)
        {
            var request = new FinishLargeFileRequest(fileId, sha1Parts);
            return await _client.FinishLargeFileAsync(request, cancellationToken);
        }

        /// <summary>
        /// Gets a url for uploading parts of a large file. 
        /// </summary>
        /// <param name="fileId">The large file id whose parts you want to upload.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<GetUploadPartUrlResponse>> IBackblazeParts.GetUploadUrlAsync(string fileId)
        {
            var request = new GetUploadPartUrlRequest(fileId);
            return await _client.GetUploadPartUrlAsync(request, cancellationToken);
        }

        /// <summary>
        /// Gets a url for uploading parts of a large file. 
        /// </summary>
        /// <param name="fileId">The large file id whose parts you want to upload.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<GetUploadPartUrlResponse>> IBackblazeParts.GetUploadUrlAsync(string fileId, TimeSpan cacheTTL)
        {
            var request = new GetUploadPartUrlRequest(fileId);
            return await _client.GetUploadPartUrlAsync(request, cacheTTL, cancellationToken);
        }

        /// <summary>
        /// List parts that have been uploaded for a large file that has not been finished yet. 
        /// </summary>
        /// <param name="fileId">The large file id whose parts you want to list.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<ListPartsResponse>> IBackblazeParts.ListAsync
            (string fileId)
        {
            var request = new ListPartsRequest(fileId);
            return await _client.ListPartsAsync(request, cancellationToken);
        }

        /// <summary>
        /// List parts that have been uploaded for a large file that has not been finished yet. 
        /// </summary>
        /// <param name="request">The <see cref="ListPartsRequest"/> to send.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<ListPartsResponse>> IBackblazeParts.ListAsync
            (ListPartsRequest request, TimeSpan cacheTTL)
        {
            return await _client.ListPartsAsync(request, cacheTTL, cancellationToken);
        }

        /// <summary>
        /// Prepares for uploading parts of a large file. 
        /// </summary>
        /// <param name="bucketId">The bucket id the file will go in.</param>
        /// <param name="fileName">The name of the large file.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<StartLargeFileResponse>> IBackblazeParts.StartLargeFileAsync(string bucketId, string fileName)
        {
            var request = new StartLargeFileRequest(bucketId, fileName);
            return await _client.StartLargeFileAsync(request, cancellationToken);
        }

        /// <summary>
        /// Prepares for uploading parts of a large file. 
        /// </summary>
        /// <param name="request">The <see cref="StartLargeFileRequest"/> to send.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<StartLargeFileResponse>> IBackblazeParts.StartLargeFileAsync(StartLargeFileRequest request)
        {
            return await _client.StartLargeFileAsync(request, cancellationToken);
        }

        #endregion

        /// <summary>
        /// Gets all parts that have been uploaded for a large file that has not been finished yet. 
        /// </summary>
        /// <param name="request">The <see cref="ListPartsRequest"/> to send.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IEnumerable<PartItem>> IBackblazeParts.GetAsync(ListPartsRequest request, TimeSpan cacheTTL)
        {
            var adapter = new PartAdapter(_client, _logger, request, cacheTTL, cancellationToken) as IEnumerable<PartItem>;
            return await Task.FromResult(adapter);
        }
    }
}
