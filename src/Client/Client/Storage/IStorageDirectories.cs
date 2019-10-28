using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Authentication;

using Bytewizer.Backblaze.Models;

namespace Bytewizer.Backblaze.Client
{
    /// <summary>
    /// An interface for <see cref="Storage"/>.
    /// </summary>
    public interface IStorageDirectories
    {
        /// <summary>
        /// Copies local files to a bucket that match a search pattern in a specified directory path, and optionally searches subdirectories.
        /// </summary>
        /// <param name="bucketId">The bucket id to copy files to.</param>
        /// <param name="localPath">The relative or absolute path to the directory to search. This string is not case-sensitive.</param>
        /// <param name="searchPattern">The search string to match against the names of files in path. This parameter can contain a combination of valid
        /// literal path and wildcard (* and ?) characters, but it doesn't support regular expressions.</param>
        /// <param name="searchOption">One of the enumeration values that specifies whether the search operation should include only the current directory
        /// or should include all subdirectories. The default value is <see cref="SearchOption.TopDirectoryOnly"/>.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="CapExceededExecption">Thrown when a cap is exceeded or an account in bad standing.</exception>
        /// <exception cref="InvalidHashException">Thrown when a checksum hash is not valid.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IList<UploadFileResponse>> CopyToAsync(string bucketId, string localPath, string searchPattern, SearchOption searchOption);
    }
}
