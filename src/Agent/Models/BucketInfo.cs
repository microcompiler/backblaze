using System;
using System.Collections.Generic;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Represents custom bucket information which uses <see cref="Dictionary{string, string}"/> limited to 10 items.
    /// </summary>
    public class BucketInfo : Dictionary<string, string>
    {
        /// <summary>
        /// Adds the specified key and value to <see cref="BucketInfo"/> limited to 10 items.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        public new void Add(string key, string value)
        {
            if (Count >= 10)
                throw new InvalidOperationException(
                          "This list is limited to 10 items. You cannot add more items.");

            base.Add(key, value);
        }
    }
}
