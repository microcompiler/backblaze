using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

using Bytewizer.Backblaze.Models;

namespace Bytewizer.Backblaze.Agent
{
    public interface IBackblazeDirectoriesAgent
    {
        Task CopyToAsync(IEnumerable<System.IO.FileInfo> files, string bucketId);
        Task CopyFromAsync(string bucketId, string prefix);
        Task Tester();
    }
}
