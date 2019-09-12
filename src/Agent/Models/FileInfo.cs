using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Represents custom file information which uses <see cref="Dictionary{string, string}"/> limited to 10 items.
    /// </summary>
    public class FileInfo : IDictionary<string, string>
    {
        /// <summary>
        /// Minimum number of characters in file info key name.
        /// </summary>
        public const int MinimumFileInfoLength = 1;

        /// <summary>
        /// Maximum number of characters in file info key name.
        /// </summary>
        public const int MaximumFileInfoLength = 50;

        private readonly Dictionary<string, string> _fileInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileInfo" /> class.
        /// </summary>
        public FileInfo()
        {
            _fileInfo = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets the <see cref="FileInfo" /> at the specified index.
        /// </summary>
        /// <param name="key">The key at which to retrieve the <see cref="FileInfo" />.</param>
        public string this[string key] { get => _fileInfo[key]; set => _fileInfo[key] = value; }

        /// <summary>
        /// Gets a collection containing the keys in the <see cref="FileInfo"/>.
        /// </summary>
        public ICollection<string> Keys => _fileInfo.Keys;

        /// <summary>
        /// Gets a collection containing the values in the <see cref="FileInfo" />.
        /// </summary>
        public ICollection<string> Values => _fileInfo.Values;

        /// <summary>
        /// Gets the number of key/value pairs contained in the <see cref="FileInfo" />.
        /// </summary>
        public int Count => _fileInfo.Count;

        /// <summary>
        /// Gets a value indicating whether the <see cref="FileInfo" /> is read-only.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Adds the specified key and value to the <see cref="FileInfo" />.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add. The value can be null for reference types.</param>
        public void Add(string key, string value)
        {
            // Validate required elements
            if (Count >= 10)
                throw new InvalidOperationException(
                    "This list is limited to 10 items. You cannot add more items.");

            if (Count >= 10)
                throw new InvalidOperationException(
                    "This list is limited to 10 items. You cannot add more items.");

            if (key.Length < MinimumFileInfoLength || key.Length > MaximumFileInfoLength)
                throw new ArgumentOutOfRangeException(
                    $"Key must be a minimum of {MinimumFileInfoLength} and a maximum of {MaximumFileInfoLength} characters long.", nameof(key));

            if (!Regex.IsMatch(key, @"^([A-Za-z0-9\-_]+)$"))
                throw new ArgumentOutOfRangeException(
                    "Key can consist of only letters, digits, dashs, and underscore.", nameof(key));

            if (key.StartsWith("b2-"))
                throw new ArgumentException("Key cannot start with 'b2-'. Reserved for internal Backblaze use.", nameof(key));

            _fileInfo.Add(key, value);
        }

        /// <summary>
        /// Adds an item to the <see cref="FileInfo" />.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="FileInfo" />.</param>
        public void Add(KeyValuePair<string, string> item)
        {
            ((IDictionary<string, string>)_fileInfo).Add(item);
        }

        /// <summary>
        /// Removes all keys and values from the <see cref="FileInfo" />.
        /// </summary>
        public void Clear()
        {
            _fileInfo.Clear();
        }

        /// <summary>
        ///  Determines whether the <see cref="FileInfo" /> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="FileInfo" />.</param>
        public bool Contains(KeyValuePair<string, string> item)
        {
            return ((IDictionary<string, string>)_fileInfo).Contains(item);
        }

        /// <summary>
        /// Determines whether the <see cref="FileInfo"/> contains the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="FileInfo"/>.</param>
        public bool ContainsKey(string key)
        {
            return _fileInfo.ContainsKey(key);
        }

        /// <summary>
        /// Copies the elements of the <see cref="FileInfo"/> to an <see cref="Array"/>,
        /// starting at a particular <see cref="Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from <see cref="FileInfo"/>. The <see cref="Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            ((IDictionary<string, string>)_fileInfo).CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the value with the specified key from the <see cref="FileInfo" />.
        /// </summary>
        /// <param name="key">The key of the element to removefrom <see cref="FileInfo" />.</param>
        public bool Remove(string key)
        {
            return _fileInfo.Remove(key);
        }

        /// <summary>
        ///  Removes the first occurrence of a specific object from the <see cref="FileInfo" />.
        /// </summary>
        /// <param name="item">The object to remove from <see cref="FileInfo" />.</param>
        public bool Remove(KeyValuePair<string, string> item)
        {
            return ((IDictionary<string, string>)_fileInfo).Remove(item);
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
            return _fileInfo.TryGetValue(key, out value);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="FileInfo"/>.
        /// </summary>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return ((IDictionary<string, string>)_fileInfo).GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="FileInfo"/>.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IDictionary<string, string>)_fileInfo).GetEnumerator();
        }
    }
}
