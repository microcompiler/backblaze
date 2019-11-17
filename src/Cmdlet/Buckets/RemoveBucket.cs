using System;
using System.Threading.Tasks;
using System.Management.Automation;

using Bytewizer.Backblaze.Models;

namespace Bytewizer.Backblaze.Cmdlet
{
    /// <summary>
    /// Deletes the bucket id specified. Only buckets that contain no version of any files can be deleted.
    /// </summary>
    [Cmdlet(VerbsCommon.Remove, "Bucket")]
    [OutputType(typeof(DeleteBucketResponse))]
    public class RemoveBucket : BaseCmdlet
    {
        #region Public Properties

        /// <summary>
        /// The bucket id to delete.
        /// </summary>
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public string BucketId { get; set; }

        #endregion

        #region Cmdlet Processing

        /// <summary>
        /// Executes the process pipeline.
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                Task<IApiResults<DeleteBucketResponse>> task = Context.Client.Buckets.DeleteAsync(BucketId);

                task.Wait();
                task.Result.EnsureSuccessStatusCode();

                WriteObject(task.Result.Response);
            }
            catch (ApiException ex)
            {
                WriteError(new ErrorRecord(ex, null, ErrorCategory.InvalidResult, Context.Client));
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, null, ErrorCategory.InvalidOperation, Context.Client));
            }
        }
    }

    #endregion
}