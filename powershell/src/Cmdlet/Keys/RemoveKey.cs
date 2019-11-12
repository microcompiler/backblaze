using System;
using System.Threading.Tasks;
using System.Management.Automation;

using Bytewizer.Backblaze.Models;

namespace Bytewizer.Backblaze.Cmdlet
{
    /// <summary>
    /// Deletes the application key id specified.
    /// </summary>
    [Cmdlet(VerbsCommon.Remove, "Key")]
    [OutputType(typeof(DeleteKeyResponse))]
    public class RemoveKey : BaseCmdlet
    {
        #region Public Properties

        /// <summary>
        /// The application key id to delete.
        /// </summary>
        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public string ApplicationKeyId { get; set; }

        #endregion

        #region Cmdlet Processing

        /// <summary>
        /// Executes the process pipeline.
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                Task<IApiResults<DeleteKeyResponse>> task = Context.Client.Keys.DeleteAsync(ApplicationKeyId);

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