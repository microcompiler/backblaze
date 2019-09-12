using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Bytewizer.Backblaze.Models;
using Bytewizer.Backblaze.Adapters;

namespace Bytewizer.Backblaze.Storage
{
    public partial class BackblazeStorage : IBackblazeFiles
    {
        public IBackblazeFiles Files { get { return this; } }

        public Task<List<FileItem>> ListAsync(ListFileNamesRequest request, int cacheTTL)
        {
            return Task.FromResult(new FileNameAdapter(_client, request, cacheTTL, cancellationToken).ToList());
        }

        public Task<List<FileItem>> ListAsync(ListFileVersionRequest request, int cacheTTL)
        {
            return Task.FromResult(new FileVersionAdapter(_client, request, cacheTTL, cancellationToken).ToList());
        }

        public Task<List<FileItem>> ListAsync(ListUnfinishedLargeFilesRequest request, int cacheTTL)
        {
            return Task.FromResult(new UnfinishedAdapter(_client, request, cacheTTL, cancellationToken).ToList());
        }

        async Task<IApiResults<DownloadFileResponse>> IBackblazeFiles.DownloadByIdAsync
        (string fileId, string localFilePath, IProgress<ICopyProgress> progress)
        {
            // Cancellation token overload
            return await Files.DownloadByIdAsync(fileId, localFilePath, progress, cancellationToken);
        }

        async Task<IApiResults<DownloadFileResponse>> IBackblazeFiles.DownloadByIdAsync
            (string fileId, string localFilePath, IProgress<ICopyProgress> progress, CancellationToken cancel)
        {

            if (!Directory.Exists(Path.GetDirectoryName(localFilePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(localFilePath));
            }
                using (var content = File.Create(localFilePath))
                {
                    var request = new DownloadFileByIdRequest(fileId);
                    var results = await _client.DownloadByIdAsync(request, content, progress, cancel);
                    if (results.IsSuccessStatusCode)
                    {
                        if (results.Response.FileInfo.TryGetValue("src_last_modified_millis", out string lastModified))
                        {
                            File.SetLastWriteTime(localFilePath, DateTime.Now);
                        }
                    }
                    return results;
                }
        }

        async Task<IApiResults<DownloadFileResponse>> IBackblazeFiles.DownloadAsync
            (string bucketName, string fileName, string localFilePath, IProgress<ICopyProgress> progress)
        {
            // Cancellation token overload
            return await Files.DownloadAsync(bucketName, fileName, localFilePath, progress, cancellationToken);
        }

        async Task<IApiResults<DownloadFileResponse>> IBackblazeFiles.DownloadAsync
            (string bucketName, string fileName, string localFilePath, IProgress<ICopyProgress> progress, CancellationToken cancel)
        {
            if (!Directory.Exists(Path.GetDirectoryName(localFilePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(localFilePath));
            }

                using (var content = File.Create(localFilePath))
                {
                    var request = new DownloadFileByNameRequest(bucketName, fileName);
                    var results = await _client.DownloadAsync(request, content, progress, cancel);
                    if (results.IsSuccessStatusCode)
                    {
                        if (results.Response.FileInfo.TryGetValue("src_last_modified_millis", out string lastModified))
                        {
                            File.SetLastWriteTime(localFilePath, DateTime.Now);
                        }
                    }
                    return results;
                }
        }

        async Task<IApiResults<UploadFileResponse>> IBackblazeFiles.UploadAsync
            (string bucketId, string fileName, string localFilePath, IProgress<ICopyProgress> progress)
        {
            // Cancellation token overload
            return await Files.UploadAsync(bucketId, fileName, localFilePath, progress, cancellationToken);
        }

        async Task<IApiResults<UploadFileResponse>> IBackblazeFiles.UploadAsync
            (string bucketId, string fileName, string localFilePath, IProgress<ICopyProgress> progress, CancellationToken cancel)
        {
                using (var content = File.OpenRead(localFilePath))
                {
                    var fileInfo = new Models.FileInfo();

                    // get last modified date
                    DateTime lastModified = File.GetLastWriteTime(localFilePath);

                    // check whether a file is read only
                    var isReadOnly = ((File.GetAttributes(localFilePath) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly);
                    fileInfo.Add("src_file_readonly", isReadOnly.ToString().ToLower());

                    // check whether a file is hidden
                    var isHidden = ((File.GetAttributes(localFilePath) & FileAttributes.Hidden) == FileAttributes.Hidden);
                    fileInfo.Add("src_file_hidden", isHidden.ToString().ToLower());

                    // check whether a file has archive attribute
                    var isArchive = ((File.GetAttributes(localFilePath) & FileAttributes.Archive) == FileAttributes.Archive);
                    fileInfo.Add("src_file_archive", isArchive.ToString().ToLower());

                    // check whether a file has compressed attribute
                    var isCompressed = ((File.GetAttributes(localFilePath) & FileAttributes.Compressed) == FileAttributes.Compressed);
                    fileInfo.Add("src_file_compressed", isCompressed.ToString().ToLower());

                    var request = new UploadFileByBucketIdRequest(bucketId, fileName) { LastModified = lastModified, FileInfo = fileInfo };

                    return await _client.UploadAsync(request, content, progress, cancel);

                }
        }

        async Task<IApiResults<ListFileNamesResponse>> IBackblazeFiles.GetNamesAsync
            (string bucketId, string prefix, string delimiter, string startFileName, long maxFileCount)
        {
            var request = new ListFileNamesRequest(bucketId)
            {
                Prefix = prefix,
                Delimiter = delimiter,
                StartFileName = startFileName,
                MaxFileCount = maxFileCount
            };

            return await _client.ListFileNamesAsync(request, cancellationToken);
        }

        async Task<IApiResults<ListFileVersionResponse>> IBackblazeFiles.GetVersionsAsync
            (string bucketId, string prefix, string delimiter, string startFileName, long maxFileCount)
        {
            var request = new ListFileVersionRequest(bucketId)
            {
                Prefix = prefix,
                Delimiter = delimiter,
                StartFileName = startFileName,
                MaxFileCount = maxFileCount
            };

            return await _client.ListFileVersionsAsync(request, cancellationToken);
        }

        async Task<IApiResults<GetFileInfoResponse>> IBackblazeFiles.GetInfoAsync
            (string fileId)
        {
            var request = new GetFileInfoRequest(fileId);
            return await _client.GetFileInfoAsync(request, cancellationToken);
        }

        async Task<IApiResults<HideFileResponse>> IBackblazeFiles.HideAsync
            (string bucketId, string fileName)
        {
            var request = new HideFileRequest(bucketId, fileName);
            return await _client.HideFileAsync(request, cancellationToken);
        }

        async Task<IApiResults<DeleteFileVersionResponse>> IBackblazeFiles.DeleteAsync
            (string fileId, string fileName)
        {
            var request = new DeleteFileVersionRequest(fileId, fileName);
            return await _client.DeleteFileVersionAsync(request, cancellationToken);
        }

        async Task<IApiResults<CopyFileResponse>> IBackblazeFiles.CopyAsync(CopyFileRequest request)
        {
            return await _client.CopyFileAsync(request, cancellationToken);
        }

        async Task<IApiResults<CopyFileResponse>> IBackblazeFiles.CopyAsync(string sourceFileId, string fileName)
        {
            var request = new CopyFileRequest(sourceFileId, fileName);
            return await _client.CopyFileAsync(request, cancellationToken);
        }


        //async Task IBackblazeFilesAgent.DeleteAllAsync
        //    (string bucketId)
        //{
        //    return await InvokePolicy.ExecuteAsync(async () =>
        //    {
        //        var results = await Files.GetVersionsAsync(bucketId);
        //        results.EnsureSuccessStatusCode();

        //        foreach (var file in results.Response.Files)
        //        {
        //            var request = new DeleteFileVersionRequest(file.FileId, file.FileName);
        //            await _client.DeleteFileVersionAsync(request, cancellationToken);
        //        }
        //    });
        //}
    }

    public static class BackblazeAgentExtensions
    {
        public static async Task<IApiResults<ListFileNamesResponse>> GetNamesAsync
           (this IBackblazeFiles filesAgent, string bucketId)
        {
            return await filesAgent.GetNamesAsync(bucketId, string.Empty, null, string.Empty, 0);
        }

        public static async Task<IApiResults<ListFileNamesResponse>> GetNamesAsync
            (this IBackblazeFiles filesAgent, string bucketId, string delimiter)
        {
            return await filesAgent.GetNamesAsync(bucketId, delimiter, string.Empty, string.Empty, 0);
        }

        public static async Task<IApiResults<ListFileVersionResponse>> GetVersionsAsync
            (this IBackblazeFiles filesAgent, string bucketId)
        {
            return await filesAgent.GetVersionsAsync(bucketId, string.Empty, null, string.Empty, 0);
        }

        public static async Task<IApiResults<ListFileVersionResponse>> GetVersionsAsync
            (this IBackblazeFiles filesAgent, string bucketId, string delimiter)
        {
            return await filesAgent.GetVersionsAsync(bucketId, delimiter, string.Empty, string.Empty, 0);
        }
    }
}
