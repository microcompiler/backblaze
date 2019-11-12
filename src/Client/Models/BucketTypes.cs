using System;
using System.Collections;
using System.Collections.Generic;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Represents a collection of <see cref="BucketFilter"/> that contains no duplicates.
    /// </summary>
    public class BucketTypes :  ICollection<BucketFilter>, IEquatable<BucketTypes>
    {
        /// <summary>
        /// The hashlist used to store elements.
        /// </summary>
        private readonly HashSet<BucketFilter> _bucketFilter;

        /// <summary>
		/// Initializes a new instance of the <see cref="BucketTypes" /> class.
		/// </summary>
        public BucketTypes()
        {
            _bucketFilter = new HashSet<BucketFilter>();
        }

        #region ICollection

        /// <summary>
        /// Gets the number of elements contained in the <see cref="BucketTypes" />.
        /// </summary>
        public int Count => _bucketFilter.Count;

        /// <summary>
        /// Gets a value indicating whether the <see cref="BucketTypes" /> is read-only.
        /// </summary>
        public bool IsReadOnly => ((ICollection<BucketFilter>)_bucketFilter).IsReadOnly;

        /// <summary>
		/// Adds the specified <see cref="BucketFilter" /> to this <see cref="BucketTypes" />.
		/// </summary>
		/// <param name="item">The <see cref="BucketFilter" /> to be added.</param>
        public void Add(BucketFilter item)
        {
            // Validate required elements
            if (Count >= 1 & item == BucketFilter.All)
                throw new InvalidOperationException(
                          $"All bucket filter cannot be requested with other bucket types.");

            _bucketFilter.Add(item);
        }

        /// <summary>
        /// Removes all elements from the <see cref="BucketTypes" />.
        /// </summary>
        public void Clear()
        {
            _bucketFilter.Clear();
        }

        /// <summary>
		/// Determines whether an element is in the <see cref="BucketTypes" />.
		/// </summary>
		/// <param name="item">The <see cref="BucketFilter" /> to locate in the <see cref="BucketTypes" />.</param>
        public bool Contains(BucketFilter item)
        {
            return _bucketFilter.Contains(item);
        }
        
        /// <summary>
        /// Removes the first occurrence of a specific <see cref="BucketFilter" /> from the <see cref="BucketTypes" />.
        /// </summary>
        /// <param name="item">The <see cref="BucketFilter" /> to remove from the <see cref="BucketTypes" />.</param>
        public bool Remove(BucketFilter item)
        {
            return _bucketFilter.Remove(item);
        }

        /// <summary>
        /// Copies the elements of the <see cref="BucketTypes"/> to an <see cref="Array"/>,
        /// starting at a particular <see cref="Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from <see cref="BucketTypes"/>. The <see cref="Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(BucketFilter[] array, int arrayIndex)
        {
            _bucketFilter.CopyTo(array, arrayIndex);
        }

        /// <summary>
		/// Returns an enumerator that iterates through the <see cref="BucketTypes" />.
		/// </summary>
        public IEnumerator<BucketFilter> GetEnumerator()
        {
            return _bucketFilter.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="BucketTypes" />.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _bucketFilter.GetEnumerator();
        }

        #endregion

        #region IEquatable

        /// <summary>
        /// Determines whether the specified <see cref="object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        public override bool Equals(object obj)
        {
            return Equals(obj as BucketTypes);
        }

        /// <summary>
        /// Determines whether this instance is equal to another <see cref="BucketTypes" />.
        /// </summary>
        /// <param name="other">The <see cref="BucketTypes" /> to compare to this instance.</param>
        public bool Equals(BucketTypes other)
        {
            return other != null &&
                ListComparer<BucketFilter>.Default.Equals(this, other);
        }

        /// <summary>
        /// Returns a hash code for this <see cref="BucketTypes" />.
        /// </summary>
        public override int GetHashCode()
        {
            var hashCode = -124797683;
            hashCode = hashCode * -842435295 + ListComparer<BucketFilter>.Default.GetHashCode(this);
            return hashCode;
        }

        /// <summary>
        /// Compares two <see cref="BucketTypes" /> instances for equality.
        /// </summary>
        /// <param name="a">The first <see cref="BucketTypes" /> to compare.</param>
        /// <param name="b">The second <see cref="BucketTypes" /> to compare.</param>
        public static bool operator == (BucketTypes a, BucketTypes b)
        {
            return EqualityComparer<BucketTypes>.Default.Equals(a, b);
        }

        /// <summary>
        /// Compares two <see cref="BucketTypes" /> instances for inequality.
        /// </summary>
        /// <param name="a">The first <see cref="BucketTypes" /> to compare.</param>
        /// <param name="b">The second <see cref="BucketTypes" /> to compare.</param>
        public static bool operator != (BucketTypes a, BucketTypes b)
        {
            return !(a == b);
        }

        #endregion
    }
}
