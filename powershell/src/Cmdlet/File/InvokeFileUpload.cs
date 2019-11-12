using System;
using System.IO;
using System.Threading.Tasks;
using System.Management.Automation;

using Bytewizer.Backblaze.Models;
using Bytewizer.Backblaze.Progress;

namespace Bytewizer.Backblaze.Cmdlet
{
    /// <summary>
    /// Uploads file content to Backblaze B2 Cloud Storage by bucket id.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Invoke, "FileUpload", ConfirmImpact = ConfirmImpact.None)]
    public class InvokeFileUpload : BaseCmdlet
    {
        #region Public Properties

        /// <summary>
        /// The bucket id you want to upload the file into.
        /// </summary>
        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public string BucketId { get; set; }

        /// <summary>
        /// The relative or absolute local path to the file.
        /// </summary>
        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = true, Position = 1, ValueFromPipeline = true)]
        public string LocalFile { get; set; }

        /// <summary>
        /// The relative or absolute remote path where to save the file.
        /// </summary>
        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = false, Position = 2, ValueFromPipeline = true)]
        public string RemoteFile { get; set; }

        /// <summary>
        /// The MIME type of the content of the file which will be returned in the Content-Type header when
        /// downloading the file.
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public string ContentType { get; set; }

        /// <summary>
        /// Disable the display the progess bar when uploading the file.
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public SwitchParameter DisableProgress { get; set; }

        #endregion

        #region Cmdlet Processing

        /// <summary>
        /// Executes the process pipeline.
        /// </summary>
        protected override void ProcessRecord()
        {
            var pathinfo = GetResolvedProviderPathFromPSPath(LocalFile, out ProviderInfo provider);
            var localFullPath = pathinfo[0];
            var remoteFullPath = RemoteFile ?? localFullPath;

            if (File.Exists(localFullPath))
            {
                var content = File.OpenRead(localFullPath);
                try
                {
                    var request = new UploadFileByBucketIdRequest(BucketId, remoteFullPath)
                    {
                        ContentType = ContentType,
                        LastModified = File.GetLastWriteTime(localFullPath)
                    };

                    Task<IApiResults<UploadFileResponse>> task;

                    if (DisableProgress)
                    {
                        task = Context.Client.UploadAsync(request, content, null, Context.Client.CancellationToken);
                    }
                    else
                    {
                        var progress = new NaiveProgress<ICopyProgress>(x =>
                        {
                            if (x.PercentComplete == 100)
                                return;

                            Host.UI.WriteProgress(0, new ProgressRecord(1,
                                "Uploading file...",
                                $"Size: {new FileSize(x.ExpectedBytes).FormatUnit()}   " +
                                $"Uploaded: {new FileSize(x.BytesTransferred).FormatUnit()} " +
                                $"@ {new FileSize(x.BytesPerSecond).FormatUnit()}/s")
                            {
                                PercentComplete = (int)(x.PercentComplete * 100)
                            });
                        });

                        task = Context.Client.UploadAsync(request, content, progress, Context.Client.CancellationToken);
                    }

                    task.Result.EnsureSuccessStatusCode();

                    content.Close();
                    WriteObject(task.Result.Response);
                }
                catch (ApiException ex)
                {
                    content.Close();
                    WriteError(new ErrorRecord(ex, null, ErrorCategory.InvalidResult, Context.Client));
                }
                catch (Exception ex)
                {
                    content.Close();
                    WriteError(new ErrorRecord(ex, null, ErrorCategory.InvalidOperation, Context.Client));
                }
            }
        }

        #endregion
    }
}
