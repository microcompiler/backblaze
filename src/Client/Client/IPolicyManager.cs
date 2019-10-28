using System;
using System.Net.Http;
using System.Threading.Tasks;
using Polly;

namespace Bytewizer.Backblaze.Client
{
    /// <summary>
    /// An interface for <see cref="IPolicyManager"/>.
    /// </summary>
    public interface IPolicyManager
    {
        /// <summary>
        /// Connect to Backblaze B2 Cloud Storage and initialize <see cref="AccountInfo"/>.
        /// </summary>
        Func<Task> ConnectAsync { get; set; }

        /// <summary>
        /// Retry policy used for downloading.
        /// </summary>
        IAsyncPolicy InvokeDownload { get; }

        /// <summary>
        /// Retry policy used for uploading.
        /// </summary>
        IAsyncPolicy InvokeUpload { get; }

        /// <summary>
        /// Retry policy used for invoking post requests.
        /// </summary>
        IAsyncPolicy InvokePolicy { get; }

        /// <summary>
        /// Retry policy used for invoking send requests.
        /// </summary>
        IAsyncPolicy<HttpResponseMessage> InvokeClient { get; }
    }
}