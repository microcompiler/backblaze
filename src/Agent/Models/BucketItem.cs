using System.Diagnostics;
using System.Collections.Generic;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Represents information related to a <see cref="BucketItem"/>.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class BucketItem : IItem
    {
        /// <summary>
        /// Gets or sets the account id that this bucket is for.
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// Gets or sets the unique id of the bucket.
        /// </summary>
        public string BucketId { get; set; }

        /// <summary>
        /// Gets or sets the unique name of the bucket.
        /// </summary>
        public string BucketName { get; set; }

        /// <summary>
        /// Gets or sets the bucket secuirty authorization type. The <see cref="BucketType.AllPublic"/> indicates that files in this bucket can be downloaded by anybody
        /// or <see cref="BucketType.AllPrivate"/> requires that you need a bucket authorization token to download the files. 
        /// </summary>
        public BucketType BucketType { get; set; }

        /// <summary>
        /// Gets or set the <see cref="Dictionary{TKey, TValue}"/> stored with the bucket. List is limited to 10 items.
        /// </summary>
        public BucketInfo BucketInfo { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="CorsRule"/> for this bucket. 
        /// </summary>
        public CorsRules CorsRules { get; set; }

        /// <summary>
        /// Gets or sets <see cref="LifecycleRule"/> for this bucket. 
        /// </summary>
        public LifecycleRules LifecycleRules { get; set; }

        /// <summary>
        /// Gets or sets a counter that is updated every time the bucket is modified and can be used with 
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
