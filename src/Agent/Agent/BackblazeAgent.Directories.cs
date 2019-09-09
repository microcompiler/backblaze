using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

using Bytewizer.Backblaze.Models;

namespace Bytewizer.Backblaze.Agent
{
    public partial class BackblazeAgent : IBackblazeDirectoriesAgent
    {
        public IBackblazeDirectoriesAgent Directories { get { return this; } }

        public async Task CopyToAsync(IEnumerable<System.IO.FileInfo> files, string bucketId)
        {
            var parallelTasks = new List<Task>();
            foreach (var file in files)
            {
                parallelTasks.Add(Task.Run(async () =>
                {
                    _logger.LogTrace($"Copying: {file.FullName} -> {file.FullName}");
                    await Files.UploadAsync(bucketId, file.FullName, file.FullName, null);
                }));
            }
            
            await Task.WhenAll(parallelTasks);

            return;
        }

        public async Task CopyFromAsync(string bucketId, string prefix)
        {
            var parallelTasks = new List<Task>();

            var request = new ListFileNamesRequest(bucketId)
            {
                Prefix = prefix
            };
            var filelist = Files.ListAsync(request, 0).GetAwaiter().GetResult();

            foreach (var file in filelist)
            {
                parallelTasks.Add(Task.Run(async () =>
                {
                    await Files.DownloadAsync("e6b1db7e-9749-4686-testbucket", file.FileName, file.FileName, null);
                }));
            }
            await Task.WhenAll(parallelTasks);
        }
            public async Task Tester()
        {
            var parallelTasks = new List<Task>();

            var results = await Files.GetNamesAsync("0ef1249e2c2faf746cc20d1b", "C:/TestSrc", null, null, 10000);
            results.EnsureSuccessStatusCode();
            
            foreach (var file in results.Response.Files)
            {
                parallelTasks.Add(Task.Run(async () =>
                {
                    await Files.DownloadAsync("e6b1db7e-9749-4686-testbucket", file.FileName, file.FileName, null);
                }));
            }
            await Task.WhenAll(parallelTasks);
        }
    }
}
