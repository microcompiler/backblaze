using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Authentication;

using Bytewizer.Backblaze.Models;

namespace Bytewizer.Backblaze.Cloud
{
    /// <summary>
    /// An interface for <see cref="Storage"/>.
    /// </summary>
    public interface IStorageFiles
    {
        #region ApiClient

        /// <summary>
        /// Creates a new file by copying from an existing file.
        /// </summary>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="CapExceededExecption">Thrown when a cap is exceeded or an account in bad standing.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<CopyFileResponse>> CopyAsync(string sourceFileId, string fileName);

        /// <summary>
        /// Creates a new file by copying from an existing file.
        /// </summary>
        /// <param name="request">The <see cref="CopyFileRequest"/> to send.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="CapExceededExecption">Thrown when a cap is exceeded or an account in bad standing.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<CopyFileResponse>> CopyAsync(CopyFileRequest request);

        /// <summary>
        /// Deletes a specific version of a file. 
        /// </summary>
        /// <param name="fileName">The name of the file to delete.</param>
        /// <param name="fileId">The id of the file to delete.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<DeleteFileVersionResponse>> DeleteAsync(string fileId, string fileName);

        /// <summary>
        /// Downloads a specific version of a file by file id.  
        /// </summary>
        /// <param name="request">The <see cref="DownloadFileByIdRequest"/> to send.</param>
        /// <param name="content">The download content to receive.</param>
        /// <param name="progress">A progress action which fires every time the write buffer is cycled.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="InvalidHashException">Thrown when a checksum hash is not valid.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<DownloadFileResponse>> DownloadAsync(DownloadFileByIdRequest request, Stream content, IProgress<ICopyProgress> progress);

        /// <summary>
        /// Downloads the most recent version of a file by name. 
        /// </summary>
        /// <param name="request">The <see cref="DownloadFileByNameRequest"/> to send.</param>
        /// <param name="content">The download content to receive.</param>
        /// <param name="progress">A progress action which fires every time the write buffer is cycled.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="InvalidHashException">Thrown when a checksum hash is not valid.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<DownloadFileResponse>> DownloadAsync(DownloadFileByNameRequest request, Stream content, IProgress<ICopyProgress> progress);

        /// <summary>
        /// Generate an authorization token that can be used to download files from a private bucket. 
        /// </summary>
        /// <param name="bucketId">The buckete id the download authorization token will allow access.</param>
        /// <param name="fileNamePrefix">The file name prefix of files the download authorization token will allow access.</param>
        /// <param name="validDurationInSeconds">The number of seconds before the authorization token will expire.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<GetDownloadAuthorizationResponse>> GetDownloadTokenAsync(string bucketId, string fileNamePrefix, long validDurationInSeconds = 3600);

        /// <summary>
        /// Generate an authorization token that can be used to download files from a private bucket. 
        /// </summary>
        /// <param name="request">The <see cref="GetDownloadAuthorizationRequest"/> to send.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<GetDownloadAuthorizationResponse>> GetDownloadTokenAsync(GetDownloadAuthorizationRequest request);

        /// <summary>
        /// Gets information about a file. 
        /// </summary>
        /// <param name="fileId">The id of the file to get information about.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<GetFileInfoResponse>> GetInfoAsync(string fileId);

        /// <summary>
        /// Gets a url for uploading files. 
        /// </summary>
        /// <param name="bucketId">The bucket id you want to upload to.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<GetUploadUrlResponse>> GetUploadUrlAsync(string bucketId);

        /// <summary>
        /// Gets a url for uploading files. 
        /// </summary>
        /// <param name="bucketId">The bucket id you want to upload to.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<GetUploadUrlResponse>> GetUploadUrlAsync(string bucketId, TimeSpan cacheTTL = default);

        /// <summary>
        /// Hides a file so that <see cref="DownloadAsync(DownloadFileByNameRequest, Stream, IProgress{ICopyProgress})"/> by name will not find the file but previous versions of the file are still stored.   
        /// </summary>
        /// <param name="bucketId">The bucket id containing the file to hide.</param>
        /// <param name="fileName">The name of the file to hide.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<HideFileResponse>> HideAsync(string bucketId, string fileName);

        /// <summary>
        /// List the names of files in a bucket starting at a given name. 
        /// </summary>
        /// <param name="bucketId">The bucket id to look for file names in.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<ListFileNamesResponse>> ListNamesAsync(string bucketId);

        /// <summary>
        /// List the names of files in a bucket starting at a given name. 
        /// </summary>
        /// <param name="request">The <see cref="ListFileNamesRequest"/> to send.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<ListFileNamesResponse>> ListNamesAsync(ListFileNamesRequest request, TimeSpan cacheTTL = default);

        /// <summary>
        /// List versions of the files contained in one bucket in alphabetical order by file name
        /// and by reverse of date/time uploaded for versions of files with the same name. 
        /// </summary>
        /// <param name="bucketId">The bucket id to look for file names in.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<ListFileVersionResponse>> ListVersionsAsync(string bucketId);

        /// <summary>
        /// List versions of the files contained in one bucket in alphabetical order by file name
        /// and by reverse of date/time uploaded for versions of files with the same name. 
        /// </summary>
        /// <param name="request">The <see cref="ListFileVersionRequest"/> to send.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<ListFileVersionResponse>> ListVersionsAsync(ListFileVersionRequest request, TimeSpan cacheTTL = default);

        /// <summary>
        /// List information about large file uploads that have been started but have not been finished or canceled. 
        /// </summary>
        /// <param name="bucketId">The bucket id to look for unfinished file names in.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<ListUnfinishedLargeFilesResponse>> ListUnfinishedAsync(string bucketId);

        /// <summary>
        /// List information about large file uploads that have been started but have not been finished or canceled. 
        /// </summary>
        /// <param name="request">The <see cref="ListUnfinishedLargeFilesRequest"/> to send.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<ListUnfinishedLargeFilesResponse>> ListUnfinishedAsync(ListUnfinishedLargeFilesRequest request, TimeSpan cacheTTL = default);

        #endregion

        /// <summary>
        /// Uploads a file by bucket id and file name to Backblaze B2 Cloud Storage. 
        /// </summary>
        /// <param name="bucketId">The bucket id you want to upload to.</param>
        /// <param name="fileName">The name of the file to upload.</param>
        /// <param name="localPath">The relative or absolute path to the file. This string is not case-sensitive.</param>
        /// <param name="progress">A progress action which fires every time the write buffer is cycled.</param>
        /// <param name="cancel">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="CapExceededExecption">Thrown when a cap is exceeded or an account in bad standing.</exception>
        /// <exception cref="InvalidHashException">Thrown when a checksum hash is not valid.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<UploadFileResponse>> UploadAsync(string bucketId, string fileName, string localPath, IProgress<ICopyProgress> progress, CancellationToken cancel);

        /// <summary>
        /// Downloads a file by bucket and file name from Backblaze B2 Cloud Storage. 
        /// </summary>
        /// <param name="bucketName">The name of the bucket to download from.</param>
        /// <param name="fileName">The name of the file to download.</param>
        /// <param name="localPath">The relative or absolute path to the file. This string is not case-sensitive.</param>
        /// <param name="progress">A progress action which fires every time the write buffer is cycled.</param>
        /// <param name="cancel">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="InvalidHashException">Thrown when a checksum hash is not valid.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<DownloadFileResponse>> DownloadAsync(string bucketName, string fileName, string localPath, IProgress<ICopyProgress> progress, CancellationToken cancel);

        /// <summary>
        /// Downloads a file by file id from Backblaze B2 Cloud Storage. 
        /// </summary>
        /// <param name="fileId">The unique id of the file to download.</param>
        /// <param name="localPath">The relative or absolute path to the file. This string is not case-sensitive.</param>
        /// <param name="progress">A progress action which fires every time the write buffer is cycled.</param>
        /// <param name="cancel">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="InvalidHashException">Thrown when a checksum hash is not valid.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<DownloadFileResponse>> DownloadByIdAsync(string fileId, string localPath, IProgress<ICopyProgress> progress, CancellationToken cancel);

        /// <summary>
        /// Returns an enumerable that iterates through the names of all files in a bucket.
        /// </summary>
        /// <param name="request">The list of file name request to send.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IEnumerable<FileItem>> GetEnumerableAsync(ListFileNamesRequest request, TimeSpan cacheTTL = default);

        /// <summary>
        /// Returns an enumerable that iterates through all versions of the files contained in one bucket in alphabetical order by file name
        /// and by reverse of date/time uploaded for versions of files with the same name. 
        /// </summary>
        /// <param name="request">The <see cref="ListFileVersionRequest"/> to send.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IEnumerable<FileItem>> GetEnumerableAsync(ListFileVersionRequest request, TimeSpan cacheTTL = default);

        /// <summary>
        /// Returns an enumerable that iterates through all large file uploads that have been started but have not finished or canceled. 
        /// </summary>
        /// <param name="request">The <see cref="ListUnfinishedLargeFilesRequest"/> to send.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IEnumerable<FileItem>> GetEnumerableAsync(ListUnfinishedLargeFilesRequest request, TimeSpan cacheTTL = default);

        /// <summary>
        /// Returns the first file item that satisfies a specified condition.
        /// </summary>
        /// <param name="request">The list of file name request to send.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now.</param>
        /// <exception cref="InvalidOperationException">No element satisfies the condition in predicate. -or- The source sequence is empty</exception>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<FileItem> FirstAsync(ListFileNamesRequest request, Func<FileItem, bool> predicate, TimeSpan cacheTTL = default);

        /// <summary>
        /// Deletes all of the files contained in bucket. 
        /// </summary>
        /// <param name="request">The <see cref="ListFileVersionRequest"/> to send.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IList<DeleteFileVersionResponse>> DeleteAllAsync(ListFileVersionRequest request);
    }
}
