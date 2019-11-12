using System;
using System.Threading.Tasks;
using System.Management.Automation;

using Bytewizer.Backblaze.Models;

namespace Bytewizer.Backblaze.Cmdlet
{
    /// <summary>
    /// Creates a new key. 
    /// </summary>
    [Cmdlet(VerbsCommon.New, "Key")]
    [OutputType(typeof(CreateKeyResponse))]
    public class NewKey : BaseCmdlet
    {
        #region Public Properties

        /// <summary>
        /// The name for this key. 
        /// </summary>
        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public string KeyName { get; set; }

        /// <summary>
        /// A list of <see cref="Capability"/> the new key should have.
        /// </summary>
        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = true, Position = 1, ValueFromPipeline = true)]
        public Capabilities Capabilities { get; set; }

        /// <summary>
        /// When provided the key will expire after the given number of seconds and will have expiration timestamp set. Value must be 
        /// a positive integer and must be less than 1000 days (in seconds). 
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        public long ValidDurationInSeconds { get; set; }

        /// <summary>
        /// When present the new key can only access this bucket. When set only these capabilities can be specified: listBuckets, listFiles,
        /// readFiles, shareFiles, writeFiles, and deleteFiles. 
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        public string BucketId { get; set; }

        /// <summary>
        /// When present restricts access to files whose names start with the prefix. You must set <see cref="BucketId"/> when setting this property. 
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        public string NamePrefix { get; set; }

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
                var request = new CreateKeyRequest(accountId, KeyName, Capabilities)
                {
                    ValidDurationInSeconds = ValidDurationInSeconds,
                    BucketId = BucketId,
                    NamePrefix = NamePrefix,
                };

                Task<IApiResults<CreateKeyResponse>> task = Context.Client.Keys.CreateAsync(request);

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

        #endregion
    }
}