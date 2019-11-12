using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Management.Automation;

using Bytewizer.Backblaze.Models;

namespace Bytewizer.Backblaze.Cmdlet
{
    /// <summary>
    /// Gets all keys associated with an account in alphabetical order by key name.  
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "Key")]
    [OutputType(typeof(KeyItem))]
    public class GetKey : BaseCmdlet
    {
        #region Public Properties

        /// <summary>
        /// An absolute cache expiration time to live (TTL) relative to now.
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipeline = true, HelpMessage = "An absolute cache expiration time to live (TTL) relative to now.")]
        public TimeSpan CacheTTL { get; set; }

        /// <summary>
        /// When specified the result will be a list containing just this key id.
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public string ApplicationKeyId { get; set; }

        #endregion

        #region Cmdlet Processing

        /// <summary>
        /// Executes the process pipeline.
        /// </summary>
        protected override void ProcessRecord()
        {
            var accountId = Context.Client.AccountId;

            try
            {
                var request = new ListKeysRequest(accountId);

                if (string.IsNullOrWhiteSpace(ApplicationKeyId))
                {

                    Task<IEnumerable<KeyItem>> task = Context.Client.Keys.GetAsync(request, CacheTTL);
                    task.Wait();

                    foreach (var key in task.Result)
                        WriteObject(key);
                }
                else
                {
                    Task<KeyItem> task = Context.Client.Keys.FindByIdAsync(ApplicationKeyId, CacheTTL);
                    task.Wait();

                    WriteObject(task.Result);
                }
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

        #endregion
    }
}