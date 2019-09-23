using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Authentication;

using Microsoft.Extensions.Logging;

using Bytewizer.Backblaze.Client;
using Bytewizer.Backblaze.Models;
using Bytewizer.Backblaze.Adapters;
using Bytewizer.Backblaze.Extensions;

namespace Bytewizer.Backblaze.Storage
{
    /// <summary>
    /// Represents a default implementation of the <see cref="BackblazeStorage"/> which uses <see cref="ApiClient"/> for making HTTP requests.
    /// </summary>
    public partial class BackblazeStorage : IBackblazeFiles
    {
        /// <summary>
        /// Provides methods to access file operations.
        /// </summary>
        public IBackblazeFiles Files { get { return this; } }

        #region ApiClient

        /// <summary>
        /// Creates a new file by copying from an existing file.
        /// </summary>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="CapExceededExecption">Thrown when a cap is exceeded or an account in bad standing.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<CopyFileResponse>> IBackblazeFiles.CopyAsync
            (string sourceFileId, string fileName)
        {
            var request = new CopyFileRequest(sourceFileId, fileName);
            return await _client.CopyFileAsync(request, cancellationToken);
        }

        /// <summary>
        /// Creates a new file by copying from an existing file.
        /// </summary>
        /// <param name="request">The <see cref="CopyFileRequest"/> to send.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="CapExceededExecption">Thrown when a cap is exceeded or an account in bad standing.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<CopyFileResponse>> IBackblazeFiles.CopyAsync
            (CopyFileRequest request)
        {
            return await _client.CopyFileAsync(request, cancellationToken);
        }

        /// <summary>
        /// Deletes a specific version of a file. 
        /// </summary>
        /// <param name="fileName">The name of the file to delete.</param>
        /// <param name="fileId">The id of the file to delete.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<DeleteFileVersionResponse>> IBackblazeFiles.DeleteAsync
            (string fileId, string fileName)
        {
            var request = new DeleteFileVersionRequest(fileId, fileName);
            return await _client.DeleteFileVersionAsync(request, cancellationToken);
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
        async Task<IApiResults<DownloadFileResponse>> IBackblazeFiles.DownloadAsync
            (DownloadFileByIdRequest request, Stream content, IProgress<ICopyProgress> progress)
        {
            return await _client.DownloadFileByIdAsync(request, content, progress, cancellationToken);
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
        async Task<IApiResults<DownloadFileResponse>> IBackblazeFiles.DownloadAsync
            (DownloadFileByNameRequest request, Stream content, IProgress<ICopyProgress> progress)
        {
            return await _client.DownloadFileByNameAsync(request, content, progress, cancellationToken);
        }

        /// <summary>
        /// Generate an authorization token that can be used to download files from a private bucket. 
        /// </summary>
        /// <param name="bucketId">The buckete id the download authorization token will allow access.</param>
        /// <param name="fileNamePrefix">The file name prefix of files the download authorization token will allow access.</param>
        /// <param name="validDurationInSeconds">The number of seconds before the authorization token will expire.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<GetDownloadAuthorizationResponse>> IBackblazeFiles.GetDownloadTokenAsync
            (string bucketId, string fileNamePrefix, long validDurationInSeconds)
        {
            var request = new GetDownloadAuthorizationRequest(bucketId, fileNamePrefix, validDurationInSeconds);
            return await _client.GetDownloadAuthorizationAsync(request, cancellationToken);
        }

        /// <summary>
        /// Generate an authorization token that can be used to download files from a private bucket. 
        /// </summary>
        /// <param name="request">The <see cref="GetDownloadAuthorizationRequest"/> to send.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<GetDownloadAuthorizationResponse>> IBackblazeFiles.GetDownloadTokenAsync
            (GetDownloadAuthorizationRequest request)
        {
            return await _client.GetDownloadAuthorizationAsync(request, cancellationToken);
        }

        /// <summary>
        /// Gets information about a file. 
        /// </summary>
        /// <param name="fileId">The id of the file to get information about.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<GetFileInfoResponse>> IBackblazeFiles.GetInfoAsync
            (string fileId)
        {
            var request = new GetFileInfoRequest(fileId);
            return await _client.GetFileInfoAsync(request, cancellationToken);
        }

        /// <summary>
        /// Gets a url for uploading files. 
        /// </summary>
        /// <param name="bucketId">The bucket id you want to upload to.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<GetUploadUrlResponse>> IBackblazeFiles.GetUploadUrlAsync
            (string bucketId)
        {
            var request = new GetUploadUrlRequest(bucketId);
            return await _client.GetUploadUrlAsync(request, cancellationToken);
        }

        /// <summary>
        /// Gets a url for uploading files. 
        /// </summary>
        /// <param name="bucketId">The ID of the bucket that you want to upload to.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<GetUploadUrlResponse>> IBackblazeFiles.GetUploadUrlAsync
            (string bucketId, TimeSpan cacheTTL)
        {
            var request = new GetUploadUrlRequest(bucketId);
            return await _client.GetUploadUrlAsync(request, cacheTTL, cancellationToken);
        }

        /// <summary>
        /// Hides a file so that <see cref="DownloadFileByNameAsync"/> will not find the file but previous versions of the file are still stored.   
        /// </summary>
        /// <param name="bucketId">The bucket id containing the file to hide.</param>
        /// <param name="fileName">The name of the file to hide.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<HideFileResponse>> IBackblazeFiles.HideAsync
            (string bucketId, string fileName)
        {
            var request = new HideFileRequest(bucketId, fileName);
            return await _client.HideFileAsync(request, cancellationToken);
        }

        /// <summary>
        /// List the names of files in a bucket starting at a given name. 
        /// </summary>
        /// <param name="bucketId">The bucket id to look for file names in.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<ListFileNamesResponse>> IBackblazeFiles.ListNamesAsync
            (string bucketId)
        {
            var request = new ListFileNamesRequest(bucketId);
            return await _client.ListFileNamesAsync(request, cancellationToken);
        }

        /// <summary>
        /// List the names of files in a bucket starting at a given name. 
        /// </summary>
        /// <param name="request">The list of file name request to send.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<ListFileNamesResponse>> IBackblazeFiles.ListNamesAsync
            (ListFileNamesRequest request, TimeSpan cacheTTL)
        {
            return await _client.ListFileNamesAsync(request, cacheTTL, cancellationToken);
        }

        /// <summary>
        /// List versions of the files contained in one bucket in alphabetical order by file name
        /// and by reverse of date/time uploaded for versions of files with the same name. 
        /// </summary>
        /// <param name="request">The <see cref="ListFileVersionRequest"/> to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<ListFileVersionResponse>> IBackblazeFiles.ListVersionsAsync
            (string bucketId)
        {
            var request = new ListFileVersionRequest(bucketId);
            return await _client.ListFileVersionsAsync(request, cancellationToken);
        }

        /// <summary>
        /// List versions of the files contained in one bucket in alphabetical order by file name
        /// and by reverse of date/time uploaded for versions of files with the same name. 
        /// </summary>
        /// <param name="request">The <see cref="ListFileVersionRequest"/> to send.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<ListFileVersionResponse>> IBackblazeFiles.ListVersionsAsync
            (ListFileVersionRequest request, TimeSpan cacheTTL)
        {
            return await _client.ListFileVersionsAsync(request, cacheTTL, cancellationToken);
        }

        /// <summary>
        /// List information about large file uploads that have been started but have not been finished or canceled. 
        /// </summary>
        /// <param name="bucketId">The bucket id to look for unfinished file names in.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<ListUnfinishedLargeFilesResponse>> IBackblazeFiles.ListUnfinishedAsync
            (string bucketId)
        {
            var request = new ListUnfinishedLargeFilesRequest(bucketId);
            return await _client.ListUnfinishedLargeFilesAsync(request, cancellationToken);
        }

        /// <summary>
        /// List information about large file uploads that have been started but have not been finished or canceled. 
        /// </summary>
        /// <param name="request">The <see cref="ListUnfinishedLargeFilesRequest"/> to send.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<ListUnfinishedLargeFilesResponse>> IBackblazeFiles.ListUnfinishedAsync
            (ListUnfinishedLargeFilesRequest request, TimeSpan cacheTTL)
        {
            return await _client.ListUnfinishedLargeFilesAsync(request, cacheTTL, cancellationToken);
        }

        #endregion

        /// <summary>
        /// Upload file to Backblaze B2 Cloud Storage. 
        /// </summary>
        /// <param name="bucketId">The bucket id you want to upload to.</param>
        /// <param name="localPath">The relative or absolute path to the file. This string is not case-sensitive.</param>
        /// <param name="progress">A progress action which fires every time the write buffer is cycled.</param>
        /// <param name="cancel">The cancellation token to cancel operation.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="CapExceededExecption">Thrown when a cap is exceeded or an account in bad standing.</exception>
        /// <exception cref="InvalidHashException">Thrown when a checksum hash is not valid.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<UploadFileResponse>> IBackblazeFiles.UploadAsync
            (string bucketId, string localPath, IProgress<ICopyProgress> progress, CancellationToken cancel)
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

                var request = new UploadFileByBucketIdRequest(bucketId, localPath) { LastModified = lastModified, FileInfo = fileInfo };
                var results = await _client.UploadAsync(request, content, progress, cancel);
                if (results.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Successfully uploaded '{localPath}' file to '{bucketId}' bucket id.");
                }
                else
                {
                    _logger.LogWarning($"Failed uploading '{localPath}' file with error: {results.Error?.Message}");
                }

                return results;
            }
        }

        /// <summary>
        /// Download file from Backblaze B2 Cloud Storage. 
        /// </summary>
        /// <param name="fileId">The unique id of the file to download.</param>
        /// <param name="localPath">The relative or absolute path to the file. This string is not case-sensitive.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="InvalidHashException">Thrown when a checksum hash is not valid.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<DownloadFileResponse>> IBackblazeFiles.DownloadByIdAsync
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
        /// Gets the names of all files in a bucket.
        /// </summary>
        /// <param name="request">The list of file name request to send.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IEnumerable<FileItem>> IBackblazeFiles.GetAsync
            (ListFileNamesRequest request, TimeSpan cacheTTL)
        {
            var adapter = new FileNameAdapter(_client, _logger, request, cacheTTL, cancellationToken) as IEnumerable<FileItem>;
            return await Task.FromResult(adapter);
        }

        /// <summary>
        /// Gets all versions of the files contained in one bucket in alphabetical order by file name
        /// and by reverse of date/time uploaded for versions of files with the same name. 
        /// </summary>
        /// <param name="request">The <see cref="ListFileVersionRequest"/> to send.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IEnumerable<FileItem>> IBackblazeFiles.GetAsync
            (ListFileVersionRequest request, TimeSpan cacheTTL)
        {
            var adapter = new FileVersionAdapter(_client, _logger, request, cacheTTL, cancellationToken) as IEnumerable<FileItem>;
            return await Task.FromResult(adapter);
        }

        /// <summary>
        /// Gets all large file uploads that have been started but have not been finished or canceled. 
        /// </summary>
        /// <param name="request">The <see cref="ListUnfinishedLargeFilesRequest"/> to send.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IEnumerable<FileItem>> IBackblazeFiles.GetAsync
            (ListUnfinishedLargeFilesRequest request, TimeSpan cacheTTL)
        {
            var adapter = new UnfinishedAdapter(_client, _logger, request, cacheTTL, cancellationToken) as IEnumerable<FileItem>;
            return await Task.FromResult(adapter);
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
        async Task<FileItem> IBackblazeFiles.FirstAsync
            (ListFileNamesRequest request, Func<FileItem, bool> predicate, TimeSpan cacheTTL)
        {
            return await Task.Run(() =>
            {
                var adapter = new FileNameAdapter(_client, _logger, request, cacheTTL, cancellationToken) as IEnumerable<FileItem>;
                return adapter.First(predicate);
            });
        }

        /// <summary>
        /// Deletes all files contained in bucket. 
        /// </summary>
        /// <param name="request">The <see cref="ListFileVersionRequest"/> to send.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IList<DeleteFileVersionResponse>> IBackblazeFiles.DeleteAllAsync
            (ListFileVersionRequest request)
        {
            var response = new List<DeleteFileVersionResponse>();
            var parallelTasks = new List<Task>();

            var files = await Files.GetAsync(request);
            foreach (var file in files)
            {
                parallelTasks.Add(Task.Run(async () =>
                {
                    var deleteRequest = new DeleteFileVersionRequest(file.FileId, file.FileName);
                    var results = await _client.DeleteFileVersionAsync(deleteRequest, cancellationToken);
                    if (results.IsSuccessStatusCode)
                    {
                        _logger.LogInformation($"Successfully deleted '{file.FileName}' file from '{request.BucketId}' bucket id.");
                        response.Add(results.Response);
                    }
                    else
                    {
                        _logger.LogWarning($"Failed deleting '{file.FileName}' file with error: {results.Error.Message}");
                    }
                }));
            }
            await Task.WhenAll(parallelTasks);

            return response;
        }
    }
}