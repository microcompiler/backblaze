using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

using Bytewizer.Backblaze.Models;


namespace Bytewizer.Backblaze.Agent
{
    public partial class BackblazeAgent : IBackblazeDirectoriesAgent
    {
        public IBackblazeDirectoriesAgent Directories { get { return this; } }

        //async Task<IApiResults<DownloadFileResponse>> IBackblazeDirectoriesAgent.UploadAsync(string bucketId, string path, string searchPattern, SearchOption searchOpton)
        //{
        //    //var tasks = new List<Task>();
        //    //var directoryInfo = new DirectoryInfo(path);
        //    //foreach (var file in directoryInfo.EnumerateFiles(searchPattern, searchOpton))
        //    //{
        //    //    var t = _bulkheadPolicy.ExecuteAsync(async () =>
        //    //    {
        //    //        return await Files.UploadAsync(bucketId, file.FullName, file.FullName, null, cancellationToken);
        //    //    });
        //    //    tasks.Add(t);
        //    //}
        //    //await Task.WhenAll(tasks);

        //    return default;
        //}
    }
}
