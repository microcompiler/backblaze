using System;
using System.Threading.Tasks;
using System.Management.Automation;

using Bytewizer.Backblaze.Models;

namespace Bytewizer.Backblaze.Cmdlet
{
    /// <summary>
    /// List buckets associated with an account in alphabetical order by bucket name. When using an authorization token
    /// that is restricted to a bucket you must include the <see cref="ListBucketsRequest.BucketId"/>
    /// </summary>
    [Cmdlet(VerbsCommon.Set, "Bucket")]
    [OutputType(typeof(UpdateBucketResponse))]
    public class SetBucket : BaseCmdlet
    {
        #region Public Properties

        /// <summary>
        /// The id of the bucket to delete.
        /// </summary>
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty]
        public string BucketId { get; set; }

        /// <summary>
        /// The bucket secuirty authorization type. The <see cref="BucketType.AllPublic"/> indicates that files in this bucket can be downloaded by anybody
        /// or <see cref="BucketType.AllPrivate"/> requires that you need a bucket authorization token to download the files. 
        /// </summary>
        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = true, Position = 1, ValueFromPipeline = true)]
        public BucketType BucketType { get; set; }

        /// <summary>
        /// User defined information stored with the bucket limted to 10 items.
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        public BucketInfo BucketInfo { get; set; }

        /// <summary>
        /// Cors rules for this bucket limited to 100 rules. 
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        public CorsRules CorsRules { get; set; }

        /// <summary>
        /// Lifecycle rules for this bucket limited to 100 rules. 
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        public LifecycleRules LifecycleRules { get; set; }

        #endregion

        #region Cmdlet Processing

        /// <summary>
        /// Executes the process pipeline.
        /// </summary>
        protected override void ProcessRecord()
        {
            var accountId = Context.Client.AccountId;
            var request = new UpdateBucketRequest(accountId, BucketId, BucketType)
            {
                BucketInfo = BucketInfo,
                CorsRules = CorsRules,
                LifecycleRules = LifecycleRules,
            };

            Task<IApiResults<UpdateBucketResponse>> task = Context.Client.Buckets.UpdateAsync(request);

            task.Wait();
            task.Result.EnsureSuccessStatusCode();

            WriteObject(task.Result.Response);
        }

        #endregion
    }
}