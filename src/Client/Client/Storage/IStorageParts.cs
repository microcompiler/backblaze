using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Authentication;

using Bytewizer.Backblaze.Models;
using System.IO;

namespace Bytewizer.Backblaze.Client
{
    /// <summary>
    /// An interface for <see cref="Storage"/>.
    /// </summary>
    public interface IStorageParts
    {
        #region ApiClient

        /// <summary>
        /// Cancels the upload of a large file and deletes all parts that have been uploaded. 
        /// </summary>
        /// <param name="fileId">The file id to cancel.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<CancelLargeFileResponse>> CancelLargeFileAsync(string fileId);


        /// <summary>
        /// Creates a new file part by copying from an existing file and storing it as a part of a large file which has already been started.
        /// </summary>
        /// <param name="request">The <see cref="CopyPartRequest"/> to send.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="CapExceededExecption">Thrown when a cap is exceeded or an account in bad standing.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<CopyPartResponse>> CopyAsync(CopyPartRequest request);

        /// <summary>
        /// Creates a new file part by copying from an existing file and storing it as a part of a large file which has already been started.
        /// </summary>
        /// <param name="sourceFileId">The unique identifier of the source file being copied.</param>
        /// <param name="largeFileId">The unique identifier of the large file the part will belong to.</param>
        /// <param name="partNumber">The part number of the file.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="CapExceededExecption">Thrown when a cap is exceeded or an account in bad standing.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<CopyPartResponse>> CopyAsync(string sourceFileId, string largeFileId, int partNumber);

        /// <summary>
        /// Converts file parts that have been uploaded into a single file. 
        /// </summary>
        /// <param name="fileId">The file id of the large file to finish.</param>
        /// <param name="sha1Parts">A list of hex SHA1 checksums for the parts of the large file.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<UploadFileResponse>> FinishLargeFileAsync(string fileId, List<string> sha1Parts);

        /// <summary>
        /// Gets a url for uploading parts of a large file. 
        /// </summary>
        /// <param name="fileId">The large file id whose parts you want to upload.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<GetUploadPartUrlResponse>> GetUploadUrlAsync(string fileId);

        /// <summary>
        /// Gets a url for uploading parts of a large file. 
        /// </summary>
        /// <param name="fileId">The large file id whose parts you want to upload.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<GetUploadPartUrlResponse>> GetUploadUrlAsync(string fileId, TimeSpan cacheTTL = default);

        /// <summary>
        /// List parts that have been uploaded for a large file that has not been finished yet. 
        /// </summary>
        /// <param name="fileId">The large file id whose parts you want to list.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<ListPartsResponse>> ListAsync(string fileId);

        /// <summary>
        /// List parts that have been uploaded for a large file that has not been finished yet. 
        /// </summary>
        /// <param name="request">The <see cref="ListPartsRequest"/> to send.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<ListPartsResponse>> ListAsync(ListPartsRequest request, TimeSpan cacheTTL = default);

        /// <summary>
        /// Prepares for uploading parts of a large file. 
        /// </summary>
        /// <param name="bucketId">The bucket id the file will go in.</param>
        /// <param name="fileName">The name of the large file.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<StartLargeFileResponse>> StartLargeFileAsync(string bucketId, string fileName);

        /// <summary>
        /// Prepares for uploading parts of a large file. 
        /// </summary>
        /// <param name="request">The <see cref="StartLargeFileRequest"/> to send.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<StartLargeFileResponse>> StartLargeFileAsync(StartLargeFileRequest request);

        /// <summary>
        /// Uploads one part of a multi-part content stream using file id obtained from <see cref="StartLargeFileResponse"/>. 
        /// </summary>
        /// <param name="uploadUrl">The url used to upload this file.</param>
        /// <param name="partNumber">The part number of the file.</param>
        /// <param name="authorizationToken">The authorization token that must be used when uploading files.</param>
        /// <param name="content"> The content stream of the content payload.</param>
        /// <param name="progress">A progress action which fires every time the write buffer is cycled.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="CapExceededExecption">Thrown when a cap is exceeded or an account in bad standing.</exception>
        /// <exception cref="InvalidHashException">Thrown when a checksum hash is not valid.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<UploadPartResponse>> UploadAsync(Uri uploadUrl, int partNumber, string authorizationToken, Stream content, IProgress<ICopyProgress> progress);

        #endregion

        /// <summary>
        /// Returns an enumerable that iterates through all parts that have been uploaded for a large file that has not been finished yet. 
        /// </summary>
        /// <param name="request">The <see cref="ListPartsRequest"/> to send.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IEnumerable<PartItem>> GetEnumerableAsync(ListPartsRequest request, TimeSpan cacheTTL = default);

        /// <summary>
        /// Gets all parts associated with a file id ordered by part number. 
        /// </summary>
        /// <param name="fileId">The large file id whose parts you want to list.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IEnumerable<PartItem>> GetAsync(string fileId);

        /// <summary>
        /// Gets all parts associated with a file id ordered by part number. 
        /// </summary>
        /// <param name="request">The <see cref="ListPartsRequest"/> to send.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IEnumerable<PartItem>> GetAsync(ListPartsRequest request, TimeSpan cacheTTL = default);
    }
}