using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Authentication;

using Microsoft.Extensions.Logging;

using Bytewizer.Backblaze.Models;
using Bytewizer.Backblaze.Extensions;
using Bytewizer.Backblaze.Enumerables;

namespace Bytewizer.Backblaze.Client
{
    /// <summary>
    /// Represents a default implementation of the <see cref="Storage"/> which uses <see cref="ApiClient"/> for making HTTP requests.
    /// </summary>
    public partial class Storage : IStorageFiles
    {
        /// <summary>
        /// Provides methods to access file operations.
        /// </summary>
        public IStorageFiles Files { get { return this; } }

        #region ApiClient

        /// <summary>
        /// Creates a new file by copying from an existing file.
        /// </summary>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="CapExceededExecption">Thrown when a cap is exceeded or an account in bad standing.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<CopyFileResponse>> IStorageFiles.CopyAsync
            (string sourceFileId, string fileName)
        {
            var request = new CopyFileRequest(sourceFileId, fileName);
            return await _client.CopyFileAsync(request, _cancellationToken);
        }

        /// <summary>
        /// Creates a new file by copying from an existing file.
        /// </summary>
        /// <param name="request">The <see cref="CopyFileRequest"/> to send.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="CapExceededExecption">Thrown when a cap is exceeded or an account in bad standing.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<CopyFileResponse>> IStorageFiles.CopyAsync
            (CopyFileRequest request)
        {
            return await _client.CopyFileAsync(request, _cancellationToken);
        }

        /// <summary>
        /// Deletes a specific version of a file. 
        /// </summary>
        /// <param name="fileName">The name of the file to delete.</param>
        /// <param name="fileId">The id of the file to delete.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<DeleteFileVersionResponse>> IStorageFiles.DeleteAsync
            (string fileId, string fileName)
        {
            var request = new DeleteFileVersionRequest(fileId, fileName);
            return await _client.DeleteFileVersionAsync(request, _cancellationToken);
        }

        /// <summary>
        /// Downloads a specific version of a file by file id.  
        /// </summary>
        /// <param name="request">The <see cref="DownloadFileByIdRequest"/> to send.</param>
        /// <param name="content">The download content to receive.</param>
        /// <param name="progress">A progress action which fires every time the write buffer is cycled.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="InvalidHashException">Thrown when a checksum hash is not valid.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<DownloadFileResponse>> IStorageFiles.DownloadAsync
            (DownloadFileByIdRequest request, Stream content, IProgress<ICopyProgress> progress)
        {
            return await _client.DownloadFileByIdAsync(request, content, progress, _cancellationToken);
        }

        /// <summary>
        /// Downloads the most recent version of a file by name. 
        /// </summary>
        /// <param name="request">The <see cref="DownloadFileByIdRequest"/> to send.</param>
        /// <param name="content">The download content to receive.</param>
        /// <param name="progress">A progress action which fires every time the write buffer is cycled.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="InvalidHashException">Thrown when a checksum hash is not valid.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<DownloadFileResponse>> IStorageFiles.DownloadAsync
            (DownloadFileByNameRequest request, Stream content, IProgress<ICopyProgress> progress)
        {
            return await _client.DownloadFileByNameAsync(request, content, progress, _cancellationToken);
        }

        /// <summary>
        /// Generate an authorization token that can be used to download files from a private bucket. 
        /// </summary>
        /// <param name="bucketId">The buckete id the download authorization token will allow access.</param>
        /// <param name="fileNamePrefix">The file name prefix of files the download authorization token will allow access.</param>
        /// <param name="validDurationInSeconds">The number of seconds before the authorization token will expire.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<GetDownloadAuthorizationResponse>> IStorageFiles.GetDownloadTokenAsync
            (string bucketId, string fileNamePrefix, long validDurationInSeconds)
        {
            var request = new GetDownloadAuthorizationRequest(bucketId, fileNamePrefix, validDurationInSeconds);
            return await _client.GetDownloadAuthorizationAsync(request, _cancellationToken);
        }

        /// <summary>
        /// Generate an authorization token that can be used to download files from a private bucket. 
        /// </summary>
        /// <param name="request">The <see cref="GetDownloadAuthorizationRequest"/> to send.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<GetDownloadAuthorizationResponse>> IStorageFiles.GetDownloadTokenAsync
            (GetDownloadAuthorizationRequest request)
        {
            return await _client.GetDownloadAuthorizationAsync(request, _cancellationToken);
        }

        /// <summary>
        /// Gets information about a file. 
        /// </summary>
        /// <param name="fileId">The id of the file to get information about.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<GetFileInfoResponse>> IStorageFiles.GetInfoAsync
            (string fileId)
        {
            var request = new GetFileInfoRequest(fileId);
            return await _client.GetFileInfoAsync(request, _cancellationToken);
        }

        /// <summary>
        /// Gets a url for uploading files. 
        /// </summary>
        /// <param name="bucketId">The bucket id you want to upload to.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<GetUploadUrlResponse>> IStorageFiles.GetUploadUrlAsync
            (string bucketId)
        {
            var request = new GetUploadUrlRequest(bucketId);
            return await _client.GetUploadUrlAsync(request, _cancellationToken);
        }

        /// <summary>
        /// Gets a url for uploading files. 
        /// </summary>
        /// <param name="bucketId">The ID of the bucket that you want to upload to.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<GetUploadUrlResponse>> IStorageFiles.GetUploadUrlAsync
            (string bucketId, TimeSpan cacheTTL)
        {
            var request = new GetUploadUrlRequest(bucketId);
            return await _client.GetUploadUrlAsync(request, cacheTTL, _cancellationToken);
        }

        /// <summary>
        /// Hides a file so that <see cref="DownloadByIdAsync(DownloadFileByIdRequest, Stream, IProgress{ICopyProgress}, CancellationToken)"/> will not find the file but previous versions of the file are still stored.   
        /// </summary>
        /// <param name="bucketId">The bucket id containing the file to hide.</param>
        /// <param name="fileName">The name of the file to hide.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<HideFileResponse>> IStorageFiles.HideAsync
            (string bucketId, string fileName)
        {
            var request = new HideFileRequest(bucketId, fileName);
            return await _client.HideFileAsync(request, _cancellationToken);
        }

        /// <summary>
        /// List the names of files in a bucket starting at a given name. 
        /// </summary>
        /// <param name="bucketId">The bucket id to look for file names in.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<ListFileNamesResponse>> IStorageFiles.ListNamesAsync
            (string bucketId)
        {
            var request = new ListFileNamesRequest(bucketId);
            return await _client.ListFileNamesAsync(request, _cancellationToken);
        }

        /// <summary>
        /// List the names of files in a bucket starting at a given name. 
        /// </summary>
        /// <param name="request">The list of file name request to send.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<ListFileNamesResponse>> IStorageFiles.ListNamesAsync
            (ListFileNamesRequest request, TimeSpan cacheTTL)
        {
            return await _client.ListFileNamesAsync(request, cacheTTL, _cancellationToken);
        }

        /// <summary>
        /// List versions of the files contained in one bucket in alphabetical order by file name
        /// and by reverse of date/time uploaded for versions of files with the same name. 
        /// </summary>
        /// <param name="bucketId">The bucket id to look for file names in.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<ListFileVersionResponse>> IStorageFiles.ListVersionsAsync
            (string bucketId)
        {
            var request = new ListFileVersionRequest(bucketId);
            return await _client.ListFileVersionsAsync(request, _cancellationToken);
        }

        /// <summary>
        /// List versions of the files contained in one bucket in alphabetical order by file name
        /// and by reverse of date/time uploaded for versions of files with the same name. 
        /// </summary>
        /// <param name="request">The <see cref="ListFileVersionRequest"/> to send.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<ListFileVersionResponse>> IStorageFiles.ListVersionsAsync
            (ListFileVersionRequest request, TimeSpan cacheTTL)
        {
            return await _client.ListFileVersionsAsync(request, cacheTTL, _cancellationToken);
        }

        /// <summary>
        /// List information about large file uploads that have been started but have not been finished or canceled. 
        /// </summary>
        /// <param name="bucketId">The bucket id to look for unfinished file names in.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<ListUnfinishedLargeFilesResponse>> IStorageFiles.ListUnfinishedAsync
            (string bucketId)
        {
            var request = new ListUnfinishedLargeFilesRequest(bucketId);
            return await _client.ListUnfinishedLargeFilesAsync(request, _cancellationToken);
        }

        /// <summary>
        /// List information about large file uploads that have been started but have not been finished or canceled. 
        /// </summary>
        /// <param name="request">The <see cref="ListUnfinishedLargeFilesRequest"/> to send.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<ListUnfinishedLargeFilesResponse>> IStorageFiles.ListUnfinishedAsync
            (ListUnfinishedLargeFilesRequest request, TimeSpan cacheTTL)
        {
            return await _client.ListUnfinishedLargeFilesAsync(request, cacheTTL, _cancellationToken);
        }

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
        async Task<IApiResults<UploadFileResponse>> IStorageFiles.UploadAsync
            (string bucketId, string fileName, string localPath, IProgress<ICopyProgress> progress, CancellationToken cancel)
        {
            using (var content = File.OpenRead(localPath))
            {
                var fileInfo = new Models.FileInfo();

                // get last modified date
                DateTime lastModified = File.GetLastWriteTime(localPath);

                // check whether a file is read only
                var isReadOnly = ((File.GetAttributes(localPath) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly);
                fileInfo.Add("src_file_readonly", isReadOnly.ToString().ToLower());

                // check whether a file is hidden
                var isHidden = ((File.GetAttributes(localPath) & FileAttributes.Hidden) == FileAttributes.Hidden);
                fileInfo.Add("src_file_hidden", isHidden.ToString().ToLower());

                // check whether a file has archive attribute
                var isArchive = ((File.GetAttributes(localPath) & FileAttributes.Archive) == FileAttributes.Archive);
                fileInfo.Add("src_file_archive", isArchive.ToString().ToLower());

                // check whether a file has compressed attribute
                var isCompressed = ((File.GetAttributes(localPath) & FileAttributes.Compressed) == FileAttributes.Compressed);
                fileInfo.Add("src_file_compressed", isCompressed.ToString().ToLower());

                var request = new UploadFileByBucketIdRequest(bucketId, fileName) { LastModified = lastModified, FileInfo = fileInfo };
                var results = await _client.UploadAsync(request, content, progress, cancel);
                if (results.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Successfully uploaded '{localPath}' file to '{bucketId}' bucket id");
                }
                else
                {
                    _logger.LogError($"Failed uploading '{localPath}' file with error: {results.Error?.Message}");
                }

                return results;
            }
        }

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
        async Task<IApiResults<DownloadFileResponse>> IStorageFiles.DownloadAsync
            (string bucketName, string fileName, string localPath, IProgress<ICopyProgress> progress, CancellationToken cancel)
        {
            if (!Directory.Exists(Path.GetDirectoryName(localPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(localPath));
            }

            using (var content = File.Create(localPath))
            {
                var request = new DownloadFileByNameRequest(bucketName, fileName);
                var results = await _client.DownloadAsync(request, content, progress, cancel);
                if (results.IsSuccessStatusCode)
                {
                    var lastModified = results.Response.FileInfo.GetLastModified();
                    if (lastModified != DateTime.MinValue)
                        File.SetLastWriteTime(localPath, lastModified);
                }

                return results;
            }
        }

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
        async Task<IApiResults<DownloadFileResponse>> IStorageFiles.DownloadByIdAsync
            (string fileId, string localPath, IProgress<ICopyProgress> progress, CancellationToken cancel)
        {
            if (!Directory.Exists(Path.GetDirectoryName(localPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(localPath));
            }

            using (var content = File.Create(localPath))
            {
                var request = new DownloadFileByIdRequest(fileId);
                var results = await _client.DownloadByIdAsync(request, content, progress, cancel);
                if (results.IsSuccessStatusCode)
                {
                    var lastModified = results.Response.FileInfo.GetLastModified();
                    if (lastModified != DateTime.MinValue)
                        File.SetLastWriteTime(localPath, lastModified);
                }

                return results;
            }
        }

        /// <summary>
        /// Returns an enumerable that iterates through the names of all files in a bucket.
        /// </summary>
        /// <param name="request">The list of file name request to send.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IEnumerable<FileItem>> IStorageFiles.GetEnumerableAsync
            (ListFileNamesRequest request, TimeSpan cacheTTL)
        {
            var enumerable = new FileNameEnumerable(_client, _logger, request, cacheTTL, _cancellationToken) as IEnumerable<FileItem>;
            return await Task.FromResult(enumerable).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns an enumerable that iterates through all versions of the files contained in one bucket in alphabetical order by file name
        /// and by reverse of date/time uploaded for versions of files with the same name. 
        /// </summary>
        /// <param name="request">The <see cref="ListFileVersionRequest"/> to send.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IEnumerable<FileItem>> IStorageFiles.GetEnumerableAsync
            (ListFileVersionRequest request, TimeSpan cacheTTL)
        {
            var enumerable = new FileVersionEnumerable(_client, _logger, request, cacheTTL, _cancellationToken) as IEnumerable<FileItem>;
            return await Task.FromResult(enumerable).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns an enumerable that iterates through all large file uploads that have been started but have not finished or canceled. 
        /// </summary>
        /// <param name="request">The <see cref="ListUnfinishedLargeFilesRequest"/> to send.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IEnumerable<FileItem>> IStorageFiles.GetEnumerableAsync
                (ListUnfinishedLargeFilesRequest request, TimeSpan cacheTTL)
        {
            var enumerable = new UnfinishedEnumerable(_client, _logger, request, cacheTTL, _cancellationToken) as IEnumerable<FileItem>;
            return await Task.FromResult(enumerable).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns the first file item that satisfies a specified condition.
        /// </summary>
        /// <param name="request">The list of file name request to send.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now.</param>
        /// <exception cref="InvalidOperationException">No element satisfies the condition in predicate or the source sequence is empty.</exception>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<FileItem> IStorageFiles.FirstAsync
            (ListFileNamesRequest request, Func<FileItem, bool> predicate, TimeSpan cacheTTL)
        {
            return await Task.Run(() =>
            {
                var adapter = new FileNameEnumerable(_client, _logger, request, cacheTTL, _cancellationToken) as IEnumerable<FileItem>;
                return adapter.First(predicate);
            });
        }

        /// <summary>
        /// Deletes all files contained in bucket. 
        /// </summary>
        /// <param name="request">The <see cref="ListFileVersionRequest"/> to send.</param>
        /// <param name="dop">The degree of parallelism. Use 0 to default to <see cref="Environment.ProcessorCount"/>.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IList<DeleteFileVersionResponse>> IStorageFiles.DeleteAllAsync
            (ListFileVersionRequest request, int dop)
        {
            var response = new List<DeleteFileVersionResponse>();
            
            var files = await Files.GetEnumerableAsync(request);      
            await files.ForEachAsync(dop, async filepath =>
            {
                var deleteRequest = new DeleteFileVersionRequest(filepath.FileId, filepath.FileName);
                var results = await _client.DeleteFileVersionAsync(deleteRequest, _cancellationToken);
                if (results.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Successfully deleted '{filepath.FileName}' file from '{request.BucketId}' bucket id");
                    response.Add(results.Response);
                }
                else
                {
                    _logger.LogWarning($"Failed deleting '{filepath.FileName}' file with error: {results.Error.Message}");
                }
            }, _cancellationToken);

            return response;
        }
    }
}