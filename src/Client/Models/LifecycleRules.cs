using System;
using System.Collections;
using System.Collections.Generic;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Represents a collection of <see cref="LifecycleRule"/> limited to 100 rules.
    /// </summary>
    public class LifecycleRules : IList<LifecycleRule>, IEquatable<LifecycleRules>
    {
        /// <summary>
        /// Maximum number of rules allowed.
        /// </summary>
        public static readonly int MaximumRulesAllowed = 100;

        /// <summary>
        /// The list used to store elements.
        /// </summary>
        private readonly List<LifecycleRule> _lifecycleRules;

        /// <summary>
        /// Initializes a new instance of the <see cref="CorsRules" /> class.
        /// </summary>
        public LifecycleRules()
        {
            _lifecycleRules = new List<LifecycleRule>();
        }

        #region IList

        /// <summary>
        /// Gets the <see cref="LifecycleRule" /> at the specified index.
        /// </summary>
        /// <param name="index">The index at which to retrieve the <see cref="LifecycleRule" />.</param>
        public LifecycleRule this[int index] { get => _lifecycleRules[index]; set => _lifecycleRules[index] = value; }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="LifecycleRules" />.
        /// </summary>
        public int Count => _lifecycleRules.Count;

        /// <summary>
        /// Gets a value indicating whether the <see cref="LifecycleRules" /> is read-only.
        /// </summary>
        public bool IsReadOnly => ((IList<LifecycleRule>)_lifecycleRules).IsReadOnly;

        /// <summary>
        /// Adds the specified <see cref="LifecycleRule"/> to <see cref="LifecycleRules"/> limited to 100 rules.
        /// </summary>
        /// <param name="value">The value of the rule to add.</param>
        public void Add(LifecycleRule value)
        {
            // Validate required elements
            if (Count >= MaximumRulesAllowed)
                throw new InvalidOperationException(
                          $"This list is limited to {MaximumRulesAllowed} rules. You cannot add more rules.");

            if (!string.IsNullOrEmpty(value.FileNamePrefix))
            {
                var rule = _lifecycleRules.Find(x => x.FileNamePrefix.Contains(value.FileNamePrefix));
                if (rule != null)
                    throw new InvalidOperationException(
                         $"File name prefix must be unique. You cannot add a rule with the same file name prefix.");
            }
            _lifecycleRules.Add(value);
        }

        /// <summary>
        /// Removes all elements from the <see cref="LifecycleRules" />.
        /// </summary>
        public void Clear()
        {
            _lifecycleRules.Clear();
        }

        /// <summary>
        ///  Determines whether the <see cref="LifecycleRules" /> contains a specific value.
        /// </summary>
        /// <param name="item">The <see cref="LifecycleRule" /> to locate in the <see cref="LifecycleRules" />.</param>
        public bool Contains(LifecycleRule item)
        {
            return _lifecycleRules.Contains(item);
        }

        /// <summary>
        /// Copies the elements of the <see cref="LifecycleRules"/> to an <see cref="Array"/>,
        /// starting at a particular <see cref="Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from <see cref="LifecycleRules"/>. The <see cref="Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(LifecycleRule[] array, int arrayIndex)
        {
            _lifecycleRules.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="LifecycleRules"/>.
        /// </summary>
        public IEnumerator<LifecycleRule> GetEnumerator()
        {
            return ((IList<LifecycleRule>)_lifecycleRules).GetEnumerator();
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the entire <see cref="LifecycleRules"/>.
        /// </summary>
        /// <param name="item">The <see cref="LifecycleRule"/> to locate in the <see cref="LifecycleRules"/>.</param>
        /// <returns>The zero-based index of the first occurrence of <see cref="LifecycleRule"/> within the entire <see cref="LifecycleRules"/>, if found; otherwise, –1.</returns>
        public int IndexOf(LifecycleRule item)
        {
            return _lifecycleRules.IndexOf(item);
        }

        /// <summary>
        /// Inserts an <see cref="LifecycleRule"/> into the <see cref="LifecycleRules"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <see cref="LifecycleRule"/> should be inserted.</param>
        /// <param name="item">The <see cref="LifecycleRule"/> to insert.</param>
        public void Insert(int index, LifecycleRule item)
        {
            _lifecycleRules.Insert(index, item);
        }

        /// <summary>
        /// Removes the first occurrence of a specific <see cref="LifecycleRule"/> from the <see cref="LifecycleRules"/>.
        /// </summary>
        /// <param name="item">The <see cref="LifecycleRule"/> to remove from the <see cref="LifecycleRules"/>.</param>
        public bool Remove(LifecycleRule item)
        {
            return _lifecycleRules.Remove(item);
        }

        /// <summary>
        /// Removes the element at the specified index of the <see cref="LifecycleRules"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the <see cref="LifecycleRule"/> to remove.</param>
        public void RemoveAt(int index)
        {
            _lifecycleRules.RemoveAt(index);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="LifecycleRules"/>.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<LifecycleRule>)_lifecycleRules).GetEnumerator();
        }

        #endregion

        #region IEquatable

        /// <summary>
        /// Determines whether the specified <see cref="object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        public override bool Equals(object obj)
        {
            return Equals(obj as LifecycleRules);
        }

        /// <summary>
        /// Determines whether this instance is equal to another <see cref="LifecycleRules" />.
        /// </summary>
        /// <param name="other">The <see cref="LifecycleRules" /> to compare to this instance.</param>
        public bool Equals(LifecycleRules other)
        {
            return other != null &&
                ListComparer<LifecycleRule>.Default.Equals(this, other);
        }

        /// <summary>
        /// Returns a hash code for this <see cref="LifecycleRules" />.
        /// </summary>
        public override int GetHashCode()
        {
            var hashCode = -235947683;
            hashCode = hashCode * -1528764295 + ListComparer<LifecycleRule>.Default.GetHashCode(this);
            return hashCode;
        }

        /// <summary>
        /// Compares two <see cref="LifecycleRules" /> instances for equality.
        /// </summary>
        /// <param name="a">The first <see cref="LifecycleRules" /> to compare.</param>
        /// <param name="b">The second <see cref="LifecycleRules" /> to compare.</param>
        public static bool operator ==(LifecycleRules a, LifecycleRules b)
        {
            return EqualityComparer<LifecycleRules>.Default.Equals(a, b);
        }

        /// <summary>
        /// Compares two <see cref="LifecycleRules" /> instances for inequality.
        /// </summary>
        /// <param name="a">The first <see cref="LifecycleRules" /> to compare.</param>
        /// <param name="b">The second <see cref="LifecycleRules" /> to compare.</param>
        public static bool operator !=(LifecycleRules a, LifecycleRules b)
        {
            return !(a == b);
        }

        #endregion
    }
}
