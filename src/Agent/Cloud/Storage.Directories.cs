using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Authentication;

using Bytewizer.Backblaze.Client;
using Bytewizer.Backblaze.Models;

namespace Bytewizer.Backblaze.Cloud
{
    /// <summary>
    /// Represents a default implementation of the <see cref="Storage"/> which uses <see cref="ApiClient"/> for making HTTP requests.
    /// </summary>
    public partial class Storage : IStorageDirectories
    {
        /// <summary>
        /// Provides methods to access directory operations.
        /// </summary>
        public IStorageDirectories Directories { get { return this; } }

        #region ApiClient

        #endregion

        /// <summary>
        /// Copies local files that match a search pattern in a specified directory path, and optionally searches subdirectories to a bucket.
        /// </summary>
        /// <param name="bucketId">The bucket id to copy files to.</param>
        /// <param name="localPath">The relative or absolute path to the directory to search. This string is not case-sensitive.</param>
        /// <param name="searchPattern">The search string to match against the names of files in path. This parameter can contain a combination of valid
        /// literal path and wildcard (* and ?) characters, but it doesn't support regular expressions.</param>
        /// <param name="searchOption">One of the enumeration values that specifies whether the search operation should include only the current directory
        /// or should include all subdirectories. The default value is <see cref="SearchOption.TopDirectoryOnly"/>.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="InvalidHashException">Thrown when a checksum hash is not valid.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IList<UploadFileResponse>> IStorageDirectories.CopyToAsync(string bucketId, string localPath, string searchPattern, SearchOption searchOption)
        {
            var response = new List<UploadFileResponse>();
            var parallelTasks = new List<Task>();

            foreach (var filepath in Directory.EnumerateFiles(localPath, searchPattern, searchOption))
            {
                parallelTasks.Add(Task.Run(async () =>
                {         
                    var results = await Files.UploadAsync(bucketId, filepath, filepath, null, cancellationToken);
                    if (results.IsSuccessStatusCode)
                    {
                        response.Add(results.Response);
                    }
                }));
            }
            await Task.WhenAll(parallelTasks);

            return response;
        }

        //async Task<List<DownloadFileResponse>> IBackblazeDirectories.CopyFromAsync(string bucketId, string prefix)
        //{
        //    var response = new List<DownloadFileResponse>();
        //    var parallelTasks = new List<Task>();

        //    var request = new ListFileNamesRequest(bucketId)
        //    {
        //        Prefix = prefix
        //    };

        //    var filelist = await Files.GetAsync(request, 0);
            
        //    foreach (var file in filelist)
        //    {
        //        parallelTasks.Add(Task.Run(async () =>
        //        {
        //            var downloadRequest = new DownloadFileByIdRequest(file.FileId);
        //            //await DownloadByIdAsync(request, );


        //        }));
        //    }
        //    await Task.WhenAll(parallelTasks);

        //    return response;
        //}
    }
}
