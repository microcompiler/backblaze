using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Represents a <see cref="BucketInfo"/> dictionary limited to 10 items.
    /// </summary>
    public class BucketInfo : IDictionary<string, string>, IEquatable<BucketInfo>
    {
        /// <summary>
        /// Maximum number of bucket items allowed.
        /// </summary>
        public const int MaximumBucketItemsAllowed = 10;

        /// <summary>
        /// Minimum number of characters in bucket key name.
        /// </summary>
        public const int MinimumBucketInfoLength = 1;

        /// <summary>
        /// Maximum number of characters in bucket key name.
        /// </summary>
        public const int MaximumBucketInfoLength = 50;

        /// <summary>
        /// The dictionary used to store elements.
        /// </summary>
        private readonly Dictionary<string, string> _bucketInfo;

        /// <summary>
		/// Initializes a new instance of the <see cref="BucketInfo" /> class.
		/// </summary>
        public BucketInfo()
        {
            _bucketInfo = new Dictionary<string, string>();
        }

        #region IDictionary

        /// <summary>
        /// Gets the <see cref="BucketInfo" /> at the specified index.
        /// </summary>
        /// <param name="key">The kay at which to retrieve the <see cref="BucketInfo" />.</param>
        public string this[string key] { get => _bucketInfo[key]; set => _bucketInfo[key] = value; }

        /// <summary>
        /// Gets a collection containing the keys in the <see cref="BucketInfo"/>.
        /// </summary>
        public ICollection<string> Keys => ((IDictionary<string, string>)_bucketInfo).Keys;

        /// <summary>
        /// Gets a collection containing the values in the <see cref="BucketInfo" />.
        /// </summary>
        public ICollection<string> Values => ((IDictionary<string, string>)_bucketInfo).Values;

        /// <summary>
        /// Gets the number of key/value pairs contained in the <see cref="BucketInfo" />.
        /// </summary>
        public int Count => _bucketInfo.Count;

        /// <summary>
        /// Gets a value indicating whether the <see cref="BucketInfo" /> is read-only.
        /// </summary>
        public bool IsReadOnly => ((IDictionary<string, string>)_bucketInfo).IsReadOnly;

        /// <summary>
        /// Adds the specified key and value to the <see cref="BucketInfo" />.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add. The value can be null for reference types.</param>
        public void Add(string key, string value)
        {
            // Validate required elements
            if (Count >= MaximumBucketItemsAllowed)
                throw new InvalidOperationException(
                    $"This list is limited to {MaximumBucketItemsAllowed} items. You cannot add more items.");

            if (key.Length < MinimumBucketInfoLength || key.Length > MaximumBucketInfoLength)
                throw new ArgumentOutOfRangeException(
                    $"Key must be a minimum of {MinimumBucketInfoLength} and a maximum of {MaximumBucketInfoLength} characters long.", nameof(key));

            if (!Regex.IsMatch(key, @"^([A-Za-z0-9\-_]+)$"))
                throw new ArgumentOutOfRangeException(
                    "Key can consist of only letters, digits, dashs, and underscore.", nameof(key));

            if (key.StartsWith("b2-"))
                throw new ArgumentException("Key cannot start with 'b2-'. Reserved for internal Backblaze use.", nameof(key));

            _bucketInfo.Add(key, value);
        }

        /// <summary>
        /// Adds an item to the <see cref="BucketInfo" />.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="BucketInfo" />.</param>
        public void Add(KeyValuePair<string, string> item)
        {
            ((IDictionary<string, string>)_bucketInfo).Add(item);
        }

        /// <summary>
		/// Removes all keys and values from the <see cref="BucketInfo" />.
		/// </summary>
        public void Clear()
        {
            _bucketInfo.Clear();
        }

        /// <summary>
        ///  Determines whether the <see cref="BucketInfo" /> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="BucketInfo" />.</param>
        public bool Contains(KeyValuePair<string, string> item)
        {
            return ((IDictionary<string, string>)_bucketInfo).Contains(item);
        }

        /// <summary>
        /// Determines whether the <see cref="BucketInfo"/> contains the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="BucketInfo"/>.</param>
        public bool ContainsKey(string key)
        {
            return _bucketInfo.ContainsKey(key);
        }

        /// <summary>
        /// Copies the elements of the <see cref="BucketInfo"/> to an <see cref="Array"/>,
        /// starting at a particular <see cref="Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from <see cref="BucketInfo"/>. The <see cref="Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            ((IDictionary<string, string>)_bucketInfo).CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the value with the specified key from the <see cref="BucketInfo"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        public bool Remove(string key)
        {
            return _bucketInfo.Remove(key);
        }

        /// <summary>
        ///  Removes the first occurrence of a specific object from the <see cref="BucketInfo"/>.
        /// </summary>
        /// <param name="item">The object to remove from <see cref="BucketInfo"/>.</param>
        public bool Remove(KeyValuePair<string, string> item)
        {
            return ((IDictionary<string, string>)_bucketInfo).Remove(item);
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">
        /// When this method returns, contains the value associated with the specified key,
        /// if the key is found; otherwise, the default value for the type of the value parameter.
        /// This parameter is passed uninitialized.
        /// </param>
        public bool TryGetValue(string key, out string value)
        {
            return _bucketInfo.TryGetValue(key, out value);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="BucketInfo"/>.
        /// </summary>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return ((IDictionary<string, string>)_bucketInfo).GetEnumerator();
        }

        /// <summary>
		/// Returns an enumerator that iterates through the <see cref="BucketInfo"/>.
		/// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IDictionary<string, string>)_bucketInfo).GetEnumerator();
        }

        #endregion

        #region IEquatable

        /// <summary>
        /// Determines whether the specified <see cref="object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        public override bool Equals(object obj)
        {
            return Equals(obj as BucketInfo);
        }

        /// <summary>
        /// Determines whether this instance is equal to another <see cref="BucketInfo" />.
        /// </summary>
        /// <param name="other">The <see cref="BucketInfo" /> to compare to this instance.</param>
        public bool Equals(BucketInfo other)
        {
            return other != null &&
                DictionaryComparer<string, string>.Default.Equals(this, other);
        }

        /// <summary>
        /// Returns a hash code for this <see cref="BucketInfo" />.
        /// </summary>
        public override int GetHashCode()
        {
            var hashCode = -245693683;
            hashCode = hashCode * -1522297595 + DictionaryComparer<string, string>.Default.GetHashCode(this);
            return hashCode;
        }

        /// <summary>
        /// Compares two <see cref="BucketInfo" /> instances for equality.
        /// </summary>
        /// <param name="a">The first <see cref="BucketInfo" /> to compare.</param>
        /// <param name="b">The second <see cref="BucketInfo" /> to compare.</param>
        public static bool operator ==(BucketInfo a, BucketInfo b)
        {
            return EqualityComparer<BucketInfo>.Default.Equals(a, b);
        }

        /// <summary>
        /// Compares two <see cref="BucketInfo" /> instances for inequality.
        /// </summary>
        /// <param name="a">The first <see cref="BucketInfo" /> to compare.</param>
        /// <param name="b">The second <see cref="BucketInfo" /> to compare.</param>
        public static bool operator !=(BucketInfo a, BucketInfo b)
        {
            return !(a == b);
        }

        #endregion
    }
}
