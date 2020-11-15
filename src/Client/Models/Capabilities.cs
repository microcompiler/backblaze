using System;
using System.Collections;
using System.Collections.Generic;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Represents a collection of <see cref="Capability"/> that contains no duplicates.
    /// </summary>
    public class Capabilities :  ICollection<Capability>, IEquatable<Capabilities>
    {
        /// <summary>
        /// The hashlist used to store elements.
        /// </summary>
        private readonly HashSet<Capability> _capability;

        /// <summary>
		/// Initializes a new instance of the <see cref="Capabilities" /> class.
		/// </summary>
        public Capabilities()
        {
            _capability = new HashSet<Capability>();
        }

        /// <summary>
        /// Initialized instance of <see cref="Capabilities"/> including all capabilities.
        /// </summary>
        public static Capabilities AllControl()
        {
            return new Capabilities
            { 
                Capability.ListKeys,
                Capability.WriteKeys,
                Capability.DeleteKeys,
                Capability.ListBuckets,
                Capability.ListAllBucketNames,
                Capability.ReadBucketRetentions,
                Capability.WriteBucketRetentions,
                Capability.WriteBuckets,
                Capability.DeleteBuckets,
                Capability.ListFiles,
                Capability.ReadFiles,
                Capability.ShareFiles,
                Capability.WriteFiles,
                Capability.DeleteFiles,
                Capability.WriteFileRetentions,
                Capability.ReadFileRetentions,
                Capability.BypassGovernance
            };
        }

        /// <summary>
        /// Initialized instance of <see cref="Capabilities"/> including read only capabilities.
        /// </summary>
        public static Capabilities ReadOnly()
        {
            return new Capabilities
            {
                Capability.ListKeys,
                Capability.ListAllBucketNames,
                Capability.ListBuckets,
                Capability.ListFiles,
                Capability.ReadFiles,
                Capability.ReadBucketRetentions,
                Capability.ReadFileRetentions,
            };
        }

        /// <summary>
        /// Initialized instance of <see cref="Capabilities"/> including bucket only capabilities.
        /// </summary>
        public static Capabilities BucketOnly()
        {
            return new Capabilities
            {
                Capability.ListAllBucketNames,
                Capability.ListBuckets,
                Capability.ListFiles,
                Capability.ReadFiles,
                Capability.ShareFiles,
                Capability.WriteFiles,
                Capability.DeleteFiles,
                Capability.ReadBucketRetentions,
                Capability.WriteBucketRetentions,
            };
        }

        #region ICollection

        /// <summary>
        /// Gets the number of elements contained in the <see cref="Capabilities" />.
        /// </summary>
        public int Count => _capability.Count;

        /// <summary>
        /// Gets a value indicating whether the <see cref="Capabilities" /> is read-only.
        /// </summary>
        public bool IsReadOnly => ((ICollection<Capability>)_capability).IsReadOnly;

        /// <summary>
		/// Adds the specified <see cref="Capability" /> to this <see cref="Capabilities" />.
		/// </summary>
		/// <param name="item">The <see cref="Capability" /> to be added.</param>
        public void Add(Capability item)
        {
            _capability.Add(item);
        }

        /// <summary>
        /// Removes all elements from the <see cref="Capabilities" />.
        /// </summary>
        public void Clear()
        {
            _capability.Clear();
        }

        /// <summary>
		/// Determines whether an element is in the <see cref="Capabilities" />.
		/// </summary>
		/// <param name="item">The <see cref="Capability" /> to locate in the <see cref="Capabilities" />.</param>
        public bool Contains(Capability item)
        {
            return _capability.Contains(item);
        }
        
        /// <summary>
        /// Removes the first occurrence of a specific <see cref="Capability" /> from the <see cref="Capabilities" />.
        /// </summary>
        /// <param name="item">The <see cref="Capability" /> to remove from the <see cref="Capabilities" />.</param>
        public bool Remove(Capability item)
        {
            return _capability.Remove(item);
        }

        /// <summary>
        /// Copies the elements of the <see cref="Capabilities"/> to an <see cref="Array"/>,
        /// starting at a particular <see cref="Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from <see cref="Capabilities"/>. The <see cref="Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(Capability[] array, int arrayIndex)
        {
            _capability.CopyTo(array, arrayIndex);
        }

        /// <summary>
		/// Returns an enumerator that iterates through the <see cref="Capabilities" />.
		/// </summary>
        public IEnumerator<Capability> GetEnumerator()
        {
            return _capability.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="Capabilities" />.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _capability.GetEnumerator();
        }

        #endregion

        #region IEquatable

        /// <summary>
        /// Determines whether the specified <see cref="object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        public override bool Equals(object obj)
        {
            return Equals(obj as Capabilities);
        }

        /// <summary>
        /// Determines whether this instance is equal to another <see cref="Capabilities" />.
        /// </summary>
        /// <param name="other">The <see cref="Capabilities" /> to compare to this instance.</param>
        public bool Equals(Capabilities other)
        {
            return other != null &&
                ListComparer<Capability>.Default.Equals(this, other);
        }

        /// <summary>
        /// Returns a hash code for this <see cref="Capabilities" />.
        /// </summary>
        public override int GetHashCode()
        {
            var hashCode = -124993683;
            hashCode = hashCode * -842234295 + ListComparer<Capability>.Default.GetHashCode(this);
            return hashCode;
        }

        /// <summary>
        /// Compares two <see cref="Capabilities" /> instances for equality.
        /// </summary>
        /// <param name="a">The first <see cref="Capabilities" /> to compare.</param>
        /// <param name="b">The second <see cref="Capabilities" /> to compare.</param>
        public static bool operator == (Capabilities a, Capabilities b)
        {
            return EqualityComparer<Capabilities>.Default.Equals(a, b);
        }

        /// <summary>
        /// Compares two <see cref="Capabilities" /> instances for inequality.
        /// </summary>
        /// <param name="a">The first <see cref="Capabilities" /> to compare.</param>
        /// <param name="b">The second <see cref="Capabilities" /> to compare.</param>
        public static bool operator != (Capabilities a, Capabilities b)
        {
            return !(a == b);
        }

        #endregion
    }
}
