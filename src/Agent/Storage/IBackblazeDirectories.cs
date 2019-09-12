using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

using Bytewizer.Backblaze.Models;

namespace Bytewizer.Backblaze.Storage
{
    public interface IBackblazeDirectories
    {
        Task CopyToAsync(IEnumerable<System.IO.FileInfo> files, string bucketId);
        Task CopyFromAsync(string bucketId, string prefix);
        Task Tester();
    }
}
