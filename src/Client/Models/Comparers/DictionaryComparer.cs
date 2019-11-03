using System;
using System.Linq;
using System.Collections.Generic;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Compares two dictionaries for equality.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    public sealed class DictionaryComparer<TKey, TValue> : IEqualityComparer<IDictionary<TKey, TValue>>
    {
        private static volatile DictionaryComparer<TKey, TValue> defaultComparer;

        /// <summary>
        /// The comparer to use to compare elements.
        /// </summary>
        private readonly IEqualityComparer<object> _elementComparer;

        /// <summary>
        /// Returns a default instance of the <see cref="DictionaryComparer{TKey, TValue}"/>.
        /// </summary>
        /// <returns>The default instance of the <see cref="DictionaryComparer{TKey, TValue}"/> class.</returns>
        public static DictionaryComparer<TKey, TValue> Default
        {
            get
            {
                if (DictionaryComparer<TKey, TValue>.defaultComparer == null)
                    DictionaryComparer<TKey, TValue>.defaultComparer = new DictionaryComparer<TKey, TValue>();
                return DictionaryComparer<TKey, TValue>.defaultComparer;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryComparer()"/> class.
        /// </summary>
        public DictionaryComparer()
            : this(EqualityComparer<object>.Default)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryComparer()"/> class.
        /// </summary>
        /// <param name="elementComparer">The comparer used to compare elements.</param>
        public DictionaryComparer(IEqualityComparer<object> elementComparer)
        {
            if (elementComparer == null)
                throw new ArgumentNullException(nameof(elementComparer));

            _elementComparer = elementComparer;
        }


        /// <summary>
        /// Determines whether the specified object instances are considered equal.
        /// </summary>
        /// <param name="objA">The first object to compare.</param>
        /// <param name="objB">The second object to compare.</param>
        /// <returns>true if the objects are considered equal; otherwise, false. If both objA and objB are null, the method returns true.</returns>
        public bool Equals(IDictionary<TKey, TValue> objA, IDictionary<TKey, TValue> objB)
        {
            if (objA.Count != objB.Count)
                return false;

            if (objA.Keys.Except(objB.Keys).Any())
                return false;

            if (objB.Keys.Except(objA.Keys).Any())
                return false;

            return objA.All(pair => _elementComparer.Equals(pair.Value, objB[pair.Key]));
        }

        /// <summary>
        /// Returns a hash code for this collection.
        /// </summary>
        public int GetHashCode(IDictionary<TKey, TValue> obj)
        {
            if (obj == null)
                return 0;

            int hash = -953698877;
            unchecked
            {
                foreach (var value in obj)
                {
                    hash = hash * 896954327 + (value.Key != default ? _elementComparer.GetHashCode(value.Key) : 126954565);
                    hash = hash * 896954327 + (value.Value != default ? _elementComparer.GetHashCode(value.Value) : 126954565);
                }
            }
            return hash;
        }
    }
}
