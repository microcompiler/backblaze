using System.Diagnostics;

using Newtonsoft.Json;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains the results of a <see cref="CancelLargeFileRequest"/> operation.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class CancelLargeFileResponse : IResponse
    {
        /// <summary>
        /// The unique identifier of the file whose upload that was canceled.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string FileId { get; internal set; }

        /// <summary>
        /// The account that the bucket is in.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string AccountId { get; internal set; }

        /// <summary>
        /// The unique identifier of the bucket.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string BucketId { get; internal set; }

        /// <summary>
        /// The name of the file that was canceled.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string FileName { get; internal set; }

        ///	<summary>
        ///	Debugger display for this object.
        ///	</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"{{{nameof(BucketId)}: {BucketId}, {nameof(FileId)}: {FileId}, {nameof(FileName)}: {FileName}}}"; }
        }

        //#region IEquatable

        ///// <summary>
        ///// Determines whether the given <paramref name="left"/> is equal <paramref name="right"/>.
        ///// </summary>
        //public static bool operator ==(MatrixColor left, MatrixColor right)
        //{
        //    return left.Equals(right);
        //}

        ///// <summary>
        ///// Determines whether the given <paramref name="left"/> is equal <paramref name="right"/>.
        ///// </summary>
        //public static bool operator !=(MatrixColor left, MatrixColor right)
        //{
        //    return !left.Equals(right);
        //}

        ///// <summary>
        ///// Determines whether this object is equal <paramref name="other"/>.
        ///// </summary>
        //public bool Equals(MatrixColor other)
        //{
        //    return ((R == other.R) && (G == other.G) && (B == other.B));
        //}

        ///// <summary>
        ///// Determines whether this object is equal <paramref name="obj"/>.
        ///// </summary>
        //public override bool Equals(object obj)
        //{
        //    return (obj is MatrixColor) ? Equals((MatrixColor)obj) : false;
        //}

        ///// <summary>
        ///// Provides the hash code for the point.
        ///// </summary>
        //public override int GetHashCode()
        //{
        //    return R.GetHashCode() ^ G.GetHashCode();
        //}

        //#endregion

    }
}
