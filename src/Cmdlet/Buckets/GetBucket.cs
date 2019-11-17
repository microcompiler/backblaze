using System;
using System.Threading.Tasks;
using System.Management.Automation;

using Bytewizer.Backblaze.Models;

namespace Bytewizer.Backblaze.Cmdlet
{
    /// <summary>
    /// List buckets associated with an account in alphabetical order by bucket name. When using an authorization token
    /// that is restricted to a bucket you must include the <see cref="BucketId"/>
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "Bucket")]
    [OutputType(typeof(BucketItem))]
    public class GetBucket : BaseCmdlet
    {
        #region Public Properties

        /// <summary>
        /// An absolute cache expiration time to live (TTL) relative to now.
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipeline = true, HelpMessage = "An absolute cache expiration time to live (TTL) relative to now.")]
        public TimeSpan CacheTTL { get; set; }

        /// <summary>
        /// When specified the result will be a list containing just this bucket id.
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public string BucketId { get; set; }

        /// <summary>
        /// When specified the result will be a list containing just this bucket name.  
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public string BucketName { get; set; }

        /// <summary>
        /// When specified the result will be filter for by bucket types. If not present, only buckets with 
        /// bucket types <see cref="BucketType.AllPublic"/>, <see cref="BucketType.AllPrivate"/> and <see cref="BucketType.Snapshot"/> will be returned.
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public BucketTypes BucketTypes { get; set; }

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
                var request = new ListBucketsRequest(accountId)
                {
                    BucketId = BucketId,
                    BucketName = BucketName,
                    BucketTypes = BucketTypes,
                };

                Task<IApiResults<ListBucketsResponse>> task = Context.Client.Buckets.ListAsync(request, CacheTTL);

                task.Wait();
                task.Result.EnsureSuccessStatusCode();

                foreach (var bucket in task.Result.Response.Buckets)
                    WriteObject(bucket);
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