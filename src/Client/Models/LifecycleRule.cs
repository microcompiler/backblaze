using System;
using System.Collections.Generic;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Represents a <see cref="LifecycleRule"/> and related properties.
    /// </summary>
    public class LifecycleRule : IEquatable<LifecycleRule>
    {
        /// <summary>
        /// Minimum number of days from hiding to deleting.
        /// </summary>
        public static readonly int MinimumDaysFromHidingToDeleting = 1;

        /// <summary>
        /// Minimum number of days from uploading to hiding.
        /// </summary>
        public static readonly int MinimumDaysFromUploadingToHiding = 1;

        /// <summary>
        /// This specifis how long to keep file versions after hiding. When set 0 is not allowed and it must be a positive number. 
        /// </summary>
        public int DaysFromHidingToDeleting
        {
            get { return _daysFromHidingToDeleting; }
            set
            {
                if (value < MinimumDaysFromHidingToDeleting)
                    throw new ArgumentOutOfRangeException("Argument must be greater then zero.", nameof(DaysFromHidingToDeleting));

                _daysFromHidingToDeleting = value;
            }
        }
        private int _daysFromHidingToDeleting;

        /// <summary>
        /// This specifis files to be hidden automatically after the given number of days. When set 0 is not allowed and it must be a positive number. 
        /// </summary>
        public int DaysFromUploadingToHiding
        {
            get { return _daysFromUploadingToHiding; }
            set
            {
                if (value < MinimumDaysFromUploadingToHiding)
                    throw new ArgumentOutOfRangeException("Argument must be greater then zero.", nameof(DaysFromUploadingToHiding));

                _daysFromUploadingToHiding = value;
            }
        }
        private int _daysFromUploadingToHiding;

        /// <summary>
        /// Specifies which files in the bucket it applies to. Any file whose name starts with the prefix is subject to the rule. 
        /// </summary>
        public string FileNamePrefix { get; set; }

        #region IEquatable

        /// <summary>
        /// Determines whether the specified <see cref="object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        public override bool Equals(object obj)
        {
            return Equals(obj as LifecycleRule);
        }

        /// <summary>
        /// Determines whether this instance is equal to another <see cref="LifecycleRule" />.
        /// </summary>
        /// <param name="other">The <see cref="LifecycleRule" /> to compare to this instance.</param>
        public bool Equals(LifecycleRule other)
        {
            return other != null &&
                   DaysFromHidingToDeleting == other.DaysFromHidingToDeleting &&
                   DaysFromUploadingToHiding == other.DaysFromUploadingToHiding &&
                   FileNamePrefix == other.FileNamePrefix;
        }

        /// <summary>
        /// Returns a hash code for this <see cref="LifecycleRule" />.
        /// </summary>
        public override int GetHashCode()
        {
            var hashCode = 1793785783;
            hashCode = hashCode * -1521134295 + DaysFromHidingToDeleting.GetHashCode();
            hashCode = hashCode * -1521134295 + DaysFromUploadingToHiding.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FileNamePrefix);
            return hashCode;
        }

        /// <summary>
        /// Compares two <see cref="LifecycleRule" /> instances for equality.
        /// </summary>
        /// <param name="a">The first <see cref="LifecycleRule" /> to compare.</param>
        /// <param name="b">The second <see cref="LifecycleRule" /> to compare.</param>
        public static bool operator ==(LifecycleRule a, LifecycleRule b)
        {
            return EqualityComparer<LifecycleRule>.Default.Equals(a, b);
        }

        /// <summary>
        /// Compares two <see cref="LifecycleRule" /> instances for inequality.
        /// </summary>
        /// <param name="a">The first <see cref="LifecycleRule" /> to compare.</param>
        /// <param name="b">The second <see cref="LifecycleRule" /> to compare.</param>
        public static bool operator !=(LifecycleRule a, LifecycleRule b)
        {
            return !(a == b);
        }

        #endregion
    }
}
