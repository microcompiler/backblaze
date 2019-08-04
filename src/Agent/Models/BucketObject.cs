using System.Diagnostics;
using System.Collections.Generic;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains response information related to a bucket.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class BucketObject
    {
        /// <summary>
        /// The account id that this bucket key is for.
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// The unique id of the bucket.
        /// </summary>
        public string BucketId { get; set; }

        /// <summary>
        /// The unique name of the bucket.
        /// </summary>
        public string BucketName { get; set; }

        /// <summary>
        /// The bucket secuirty authorization type. The <see cref="BucketType.AllPublic"/> indicates that files in this bucket can be downloaded by anybody
        /// or <see cref="BucketType.AllPrivate"/> requires that you need a bucket authorization token to download the files. 
        /// </summary>
        public BucketType BucketType { get; set; }

        /// <summary>
        /// User-defined information to be stored with the bucket.
        /// </summary>
        public BucketInfo BucketInfo { get; set; }

        /// <summary>
        /// Cors rules for this bucket. 
        /// </summary>
        public CorsRules CorsRules { get; set; }

        /// <summary>
        /// Lifecycle rules for this bucket. 
        /// </summary>
        public LifecycleRules LifecycleRules { get; set; }

        /// <summary>
        /// A counter that is updated every time the bucket is modified and can be used with 
        /// the <see cref="UpdateBucketRequest.IfRevisionIs"/> to prevent colliding simultaneous updates. 
        /// </summary>
        public long Revision { get; set; }

        ///	<summary>
        ///	Debugger display for this object.
        ///	</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{{{nameof(BucketId)}: {BucketId}, {nameof(BucketName)}: {BucketName}, {nameof(BucketType)}: {BucketType.ToString()}}}"; }
        }
    }
}
