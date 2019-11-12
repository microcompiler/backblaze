using System;
using System.Threading.Tasks;
using System.Management.Automation;

using Bytewizer.Backblaze.Models;

namespace Bytewizer.Backblaze.Cmdlet
{
    /// <summary>
    /// Creates a new bucket. 
    /// </summary>
    [Cmdlet(VerbsCommon.New, "Bucket")]
    [OutputType(typeof(CreateBucketResponse))]
    public class NewBucket : BaseCmdlet
    {
        #region Public Properties

        /// <summary>
        /// The name to give the new bucket. Bucket names must be a minimum of 6 and a maximum of 50 characters long,
        /// and must be globally unique; two different B2 accounts cannot have buckets with the same name. Bucket names
        /// can consist of: letters, digits, and "-". Bucket names cannot start with "b2-" as these are reserved for
        /// internal Backblaze use. 
        /// </summary>
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty]
        public string BucketName { get; set; }

        /// <summary>
        /// The bucket secuirty authorization type. The <see cref="BucketType.AllPublic"/> indicates that files in this bucket can be downloaded by anybody
        /// or <see cref="BucketType.AllPrivate"/> requires that you need a bucket authorization token to download the files. 
        /// </summary>
        [Parameter(Mandatory = true, Position = 1, ValueFromPipeline = true)]
        public BucketType BucketType { get; set; }

        /// <summary>
        /// User defined information stored with the bucket limted to 10 items.
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        public BucketInfo BucketInfo { get; set; } = new BucketInfo();

        /// <summary>
        /// Cors rules for this bucket limited to 100 rules. 
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        public CorsRules CorsRules { get; set; } = new CorsRules();

        /// <summary>
        /// Lifecycle rules for this bucket limited to 100 rules. 
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        public LifecycleRules LifecycleRules { get; set; } = new LifecycleRules();

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
                var request = new CreateBucketRequest(accountId, BucketName, BucketType)
                {
                    BucketInfo = BucketInfo,
                    CorsRules = CorsRules,
                    LifecycleRules = LifecycleRules
                };

                Task<IApiResults<CreateBucketResponse>> task = Context.Client.Buckets.CreateAsync(request);

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