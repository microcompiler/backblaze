using System;
using System.Collections.Generic;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains response information related to a lifecycle rule.
    /// </summary>
    public class LifecycleRule : IEquatable<LifecycleRule>
    {
        /// <summary>
        /// This specifis how long to keep file versions after hiding. When set 0 is not allowed and it must be a positive number. 
        /// </summary>
        public uint DaysFromHidingToDeleting
        {
            get { return _daysFromHidingToDeleting; }
            set
            {
                if (value == 0) throw new ArgumentOutOfRangeException("Argument can not be zero.", nameof(DaysFromHidingToDeleting));
                _daysFromHidingToDeleting = value;
            }
        }
        private uint _daysFromHidingToDeleting;

        /// <summary>
        /// This specifis files to be hidden automatically after the given number of days. When set 0 is not allowed and it must be a positive number. 
        /// </summary>
        public uint DaysFromUploadingToHiding
        {
            get { return _daysFromUploadingToHiding; }
            set
            {
                if (value == 0) throw new ArgumentOutOfRangeException("Argument can not be zero.", nameof(DaysFromUploadingToHiding));
                _daysFromUploadingToHiding = value;
            }
        }
        private uint _daysFromUploadingToHiding;

        /// <summary>
        /// A bucket can have up to 100 lifecycle rules. Each rule has a <see cref="FileNamePrefix"/> that specifies which files in the bucket it applies to. 
        /// Any file whose name starts with the prefix is subject to the rule. 
        /// </summary>
        public string FileNamePrefix { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as LifecycleRule);
        }

        public bool Equals(LifecycleRule other)
        {
            return other != null &&
                   DaysFromHidingToDeleting == other.DaysFromHidingToDeleting &&
                   DaysFromUploadingToHiding == other.DaysFromUploadingToHiding &&
                   FileNamePrefix == other.FileNamePrefix;
        }

        public override int GetHashCode()
        {
            var hashCode = 1793785783;
            hashCode = hashCode * -1521134295 + DaysFromHidingToDeleting.GetHashCode();
            hashCode = hashCode * -1521134295 + DaysFromUploadingToHiding.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FileNamePrefix);
            return hashCode;
        }
    }
}
