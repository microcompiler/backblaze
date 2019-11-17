using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using Bytewizer.Backblaze.Models;
using Bytewizer.Backblaze.Progress;

namespace Bytewizer.Backblaze.Cmdlet
{
    /// <summary>
    /// Downloads file content from Backblaze B2 Cloud Storage by file id.
    /// </summary>
    [Cmdlet(VerbsCommon.New, "FileDownload", ConfirmImpact = ConfirmImpact.None)]
    [OutputType(typeof(DownloadFileResponse))]
    public class InvokeFileDownload : BaseCmdlet
    {
        #region Public Properties

        /// <summary>
        /// Remote file id to download.
        /// </summary>
        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public string RemoteFileId { get; set; }

        /// <summary>
        /// The local path where to save the file.
        /// </summary>
        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = true, Position = 1, ValueFromPipeline = true)]
        public string LocalFile { get; set; }

        /// <summary>
        /// If the local file exists overwrite it.
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public SwitchParameter Overwrite { get; set; }

        /// <summary>
        /// Supress progress bar.
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public SwitchParameter NoProgress { get; set; }

        #endregion

        #region Cmdlet Processing

        /// <summary>
        /// Executes the process pipeline.
        /// </summary>
        protected override void ProcessRecord()
        {
            var pathinfo = GetResolvedProviderPathFromPSPath(LocalFile, out ProviderInfo provider);
            var localfullPath = pathinfo[0];

            if (Directory.Exists(@localfullPath))
            {
                var filename = Path.GetFileName(RemoteFileId);
                var localfilefullpath = localfullPath + "/" + filename;
                var present = File.Exists(localfilefullpath);

                if ((present & Overwrite) || (!present))
                {
                    var localStream = File.Create(@localfilefullpath);
                    try
                    {
                        var request = new DownloadFileByIdRequest(RemoteFileId);
                        Task<IApiResults<DownloadFileResponse>> task;
                        if (NoProgress)
                        {
                            task = Context.Client.DownloadByIdAsync(request, localStream, null, Context.Client.CancellationToken);
                        }
                        else
                        {
                            task = Context.Client.DownloadByIdAsync(request, localStream, null, Context.Client.CancellationToken);
                        }

                        if (task.Result.IsSuccessStatusCode)
                        {
                            WriteObject(task.Result.Response);

                        }
                        localStream.Close();
                    }
                    catch (ApiException ex)
                    {
                        localStream.Close();
                        WriteError(new ErrorRecord(ex, null, ErrorCategory.InvalidResult, Context.Client));
                    }
                }
                else
                {
                    var ex = new UnauthorizedAccessException("File already present on local host.");
                    WriteError(new ErrorRecord(ex, "File already present on local host.", ErrorCategory.InvalidOperation, Context.Client));
                }
            }
            else
            {
                var ex = new FileNotFoundException($"Local path {localfullPath} was not found.");
                WriteError(new ErrorRecord(ex, $"Local path {localfullPath} was not found.", ErrorCategory.InvalidOperation, localfullPath));
            }

            #endregion
        }
    }
}
