using System;
using System.Collections.Generic;
using System.Linq;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Compares two lists for equality.
    /// </summary>
    /// <typeparam name="T">The type of elements in the lists.</typeparam>
    public sealed class ListComparer<T> : IEqualityComparer<IEnumerable<T>>
    {
        private static volatile ListComparer<T> defaultComparer;

        /// <summary>
        /// The comparer to use to compare elements.
        /// </summary>
        private readonly IEqualityComparer<T> _elementComparer;

        /// <summary>
        /// Returns a default instance of the <see cref="ListComparer{T}"/>.
        /// </summary>
        /// <returns>The default instance of the <see cref="ListComparer{T}"/> class for type <typeparamref name="T"/>.</returns>
        public static ListComparer<T> Default
        {
            get
            {
                if (ListComparer<T>.defaultComparer == null)
                    ListComparer<T>.defaultComparer = new ListComparer<T>();
                return ListComparer<T>.defaultComparer;
            }
        }

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ListComparer{T}"/> class.
        /// </summary>
        public ListComparer()
            : this(EqualityComparer<T>.Default)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListComparer{T}"/> class.
        /// </summary>
        /// <param name="elementComparer">The comparer used to compare elements.</param>
        public ListComparer(IEqualityComparer<T> elementComparer)
        {
            if (elementComparer == null)
                throw new ArgumentNullException(nameof(elementComparer));

            _elementComparer = elementComparer;
        }
        #endregion

        /// <summary>
        /// Determines whether the specified object instances are considered equal.
        /// </summary>
        /// <param name="objA">The first object to compare.</param>
        /// <param name="ObjB">The second object to compare.</param>
        /// <returns>true if the objects are considered equal; otherwise, false. If both objA and objB are null, the method returns true.</returns>
        public bool Equals(IEnumerable<T> objA, IEnumerable<T> ObjB)
        {
            if (objA == null)
                return ObjB == null;

            if (ObjB == null)
                return false;

            if (Object.ReferenceEquals(objA, ObjB))
                return true;

            return objA.SequenceEqual(ObjB, _elementComparer);
        }

        /// <summary>
        /// Returns a hash code for this collection.
        /// </summary>
        public int GetHashCode(IEnumerable<T> enumerable)
        {
            if (enumerable == null)
                return 0;

            int hash = 17;
            unchecked
            {
                foreach (T value in enumerable)
                {
                    hash = hash * 29 + (value != null ? _elementComparer.GetHashCode(value) : 42);
                }
            }
            return hash;
        }
    }
}
