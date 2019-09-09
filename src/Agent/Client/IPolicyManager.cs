using System;
using System.Threading.Tasks;
using Polly;

namespace Bytewizer.Backblaze.Client
{
    public interface IPolicyManager
    {
        Func<Task> ConnectAsync { get; set; }
        IAsyncPolicy DownloadPolicy { get; }
        IAsyncPolicy InvokePolicy { get; }
        IAsyncPolicy UploadPolicy { get; }
    }
}