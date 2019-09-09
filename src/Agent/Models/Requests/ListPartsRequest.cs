using System;
using System.Collections.Generic;
using System.Diagnostics;

using Newtonsoft.Json;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains information to create a list parts request.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class ListPartsRequest : IEquatable<ListPartsRequest>, IRequest
    {
        /// <summary>
        /// Represents the default number of keys per transaction.
        /// </summary>
        public const int DefaultPartsPerTransaction = 100;

        /// <summary>
        /// Maximum number of keys per transaction.
        /// </summary>
        public const int MaximumPartsPerTransaction = 10000;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListPartsRequest"/> class.
        /// </summary>
        /// <param name="fileId">The unique identifier for the file.</param>
        public ListPartsRequest(string fileId)
        {
            // Validate required arguments
            if (string.IsNullOrWhiteSpace(fileId))
                throw new ArgumentException("Argument can not be null, empty, or consist only of white-space characters.", nameof(fileId));

            // Initialize and set required properties
            FileId = fileId;
        }

        /// <summary>
        /// The file id whose parts will be listed. 
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string FileId { get; private set; }

        /// <summary>
        /// The maximum number of parts to return from this call. 
        /// The default value is <see cref="DefaultPartsPerTransaction"/> and the maximum allowed is <see cref="MaximumPartsPerTransaction"/>.
        /// Passing in 0 means to use the default of <see cref="DefaultPartPerTransaction"/>. NOTE: <see cref="ListPartsRequest"/> is a Class C transaction. 
        /// If you set <see cref="MaxPartCount"/> to more than 1000 and more than 1000 are returned, the call will be billed as multiple transactions,
        /// as if you had made requests in a loop asking for 1000 at a time. For example: if you set <see cref="MaxPartCount"/> to 10000 and 3123 items are returned,
        /// you will be billed for 4 Class C transactions. 
        /// </summary>
        public long MaxPartCount
        {
            get { return _maxKeyCount; }
            set
            {
                if (value < 0 || value > MaximumPartsPerTransaction)
                    throw new ArgumentException($"Property '{nameof(MaxPartCount)}' must be a minimum of 1 and a maximum of {MaximumPartsPerTransaction} of files to return.");
                else
                    _maxKeyCount = value;
            }
        }
        private long _maxKeyCount = DefaultPartsPerTransaction;

        /// <summary>
        /// The first part to return. If there is a part with this number it will be returned as the first
        /// in the list. If not the returned list will start with the first part number after this one. 
        /// </summary>
        public string StartPartNumber { get; set; }

        /// <summary>
        /// Converts the value of this instance to a memory cache key.
        /// </summary>
        public string ToCacheKey()
        {
            return $"{GetType().Name}--{GetHashCode().ToString()}";
        }

        ///	<summary>
        ///	Debugger display for this object.
        ///	</summary>
        [JsonIgnore]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{{{nameof(FileId)}: {FileId}}}"; }
        }

        #region IEquatable

        /// <summary>
        /// Determines whether this object is equal <paramref name="obj"/>.
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as ListPartsRequest);
        }

        /// <summary>
        /// Determines whether this object is equal <paramref name="other"/>.
        /// </summary>
        public bool Equals(ListPartsRequest other)
        {
            return other != null &&
                   FileId == other.FileId &&
                   MaxPartCount == other.MaxPartCount &&
                   StartPartNumber == other.StartPartNumber;
        }

        /// <summary>
        /// Provides hash code for the request.
        /// </summary>
        public override int GetHashCode()
        {
            var hashCode = -827037148;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FileId);
            hashCode = hashCode * -1521134295 + MaxPartCount.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(StartPartNumber);
            return hashCode;
        }

        /// <summary>
        /// Determines whether the given <paramref name="left"/> is equal <paramref name="right"/>.
        /// </summary>
        public static bool operator ==(ListPartsRequest left, ListPartsRequest right)
        {
            return EqualityComparer<ListPartsRequest>.Default.Equals(left, right);
        }

        /// <summary>
        /// Determines whether the given <paramref name="left"/> is not equal <paramref name="right"/>.
        /// </summary>
        public static bool operator !=(ListPartsRequest left, ListPartsRequest right)
        {
            return !(left == right);
        }

        #endregion
    }
}
