using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Represents custom file information which uses <see cref="Dictionary{string, string}"/> limited to 10 items.
    /// </summary>
    public class FileInfo : Dictionary<string, string>
    {
        /// <summary>
        /// Minimum number of characters in file info key name.
        /// </summary>
        public const int MinimumFileInfoLength = 1;

        /// <summary>
        /// Maximum number of characters in file info key name.
        /// </summary>
        public const int MaximumFileInfoLength = 50;

        /// <summary>
        /// Adds the specified key and value to <see cref="FileInfo"/> limited to 10 items.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        public new void Add(string key, string value)
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

            base.Add(key, value);
        }
    }
}
