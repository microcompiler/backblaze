using System;
using System.Collections;
using System.Collections.Generic;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Represents a <see cref="CorsRules"/> limited to 100 rules.
    /// </summary>
    public class CorsRules : IList<CorsRule>, IEquatable<CorsRules>
    {
        /// <summary>
        /// Maximum number of rules allowed.
        /// </summary>
        public const int MaximumRulesAllowed = 100;

        /// <summary>
        /// The list used to store elements.
        /// </summary>
        private readonly List<CorsRule> _corsRules;

        /// <summary>
        /// Initializes a new instance of the <see cref="CorsRules" /> class.
        /// </summary>
        public CorsRules()
        {
            _corsRules = new List<CorsRule>();
        }

        /// <summary>
        /// Gets the <see cref="CorsRule" /> at the specified index.
        /// </summary>
        /// <param name="index">The index at which to retrieve the <see cref="CorsRule" />.</param>
        public CorsRule this[int index] { get => _corsRules[index]; set => _corsRules[index] = value; }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="CorsRules" />.
        /// </summary>
        public int Count => _corsRules.Count;

        /// <summary>
        /// Gets a value indicating whether the <see cref="CorsRules" /> is read-only.
        /// </summary>
        public bool IsReadOnly => ((IList<CorsRule>)_corsRules).IsReadOnly;

        /// <summary>
        /// Adds the specified <see cref="CorsRule"/> to <see cref="CorsRules"/> limited to 100 rules.
        /// </summary>
        /// <param name="value">The value of the rule to add.</param>
        public void Add(CorsRule value)
        {
            // Validate required elements
            if (Count >= MaximumRulesAllowed)
                throw new InvalidOperationException(
                          $"This list is limited to {MaximumRulesAllowed} rules. You cannot add more rules.");

            if (!string.IsNullOrEmpty(value.CorsRuleName))
            {
                var rule = _corsRules.Find(x => x.CorsRuleName.Contains(value.CorsRuleName));
                if (rule != null)
                    throw new InvalidOperationException(
                         $"Rule names must be unique. You cannot add a rule with the same rule name.");
            }
            _corsRules.Add(value);
        }

        /// <summary>
        /// Removes all elements from the <see cref="CorsRules" />.
        /// </summary>
        public void Clear()
        {
            _corsRules.Clear();
        }

        /// <summary>
        ///  Determines whether the <see cref="CorsRules" /> contains a specific value.
        /// </summary>
        /// <param name="item">The <see cref="CorsRule" /> to locate in the <see cref="CorsRules" />.</param>
        public bool Contains(CorsRule item)
        {
            return _corsRules.Contains(item);
        }

        /// <summary>
        /// Copies the elements of the <see cref="CorsRules"/> to an <see cref="Array"/>,
        /// starting at a particular <see cref="Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from <see cref="CorsRules"/>. The <see cref="Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(CorsRule[] array, int arrayIndex)
        {
            _corsRules.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Determines whether the specified <see cref="object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        public override bool Equals(object obj)
        {
            return Equals(obj as CorsRules);
        }

        /// <summary>
        /// Determines whether this instance is equal to another <see cref="CorsRules" />.
        /// </summary>
        /// <param name="other">The <see cref="CorsRules" /> to compare to this instance.</param>
        public bool Equals(CorsRules other)
        {
            return other != null &&
                ListComparer<CorsRule>.Default.Equals(this, other);
        }

        /// <summary>
        /// Returns a hash code for this <see cref="CorsRules" />.
        /// </summary>
        public override int GetHashCode()
        {
            var hashCode = -235993683;
            hashCode = hashCode * -1522234295 + ListComparer<CorsRule>.Default.GetHashCode(this);
            return hashCode;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="CorsRules"/>.
        /// </summary>
        public IEnumerator<CorsRule> GetEnumerator()
        {
            return ((IList<CorsRule>)_corsRules).GetEnumerator();
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the entire <see cref="CorsRules"/>.
        /// </summary>
        /// <param name="item">The <see cref="CorsRule"/> to locate in the <see cref="CorsRules"/>.</param>
        /// <returns>The zero-based index of the first occurrence of <see cref="CorsRule"/> within the entire <see cref="CorsRules"/>, if found; otherwise, –1.</returns>
        public int IndexOf(CorsRule item)
        {
            return _corsRules.IndexOf(item);
        }

        /// <summary>
        /// Inserts an <see cref="CorsRule"/> into the <see cref="CorsRules"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <see cref="CorsRule"/> should be inserted.</param>
        /// <param name="item">The <see cref="CorsRule"/> to insert.</param>
        public void Insert(int index, CorsRule item)
        {
            _corsRules.Insert(index, item);
        }

        /// <summary>
        /// Removes the first occurrence of a specific <see cref="CorsRule"/> from the <see cref="CorsRules"/>.
        /// </summary>
        /// <param name="item">The <see cref="CorsRule"/> to remove from the <see cref="CorsRules"/>.</param>
        public bool Remove(CorsRule item)
        {
            return _corsRules.Remove(item);
        }

        /// <summary>
        /// Removes the element at the specified index of the <see cref="CorsRules"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the <see cref="CorsRule"/> to remove.</param>
        public void RemoveAt(int index)
        {
            _corsRules.RemoveAt(index);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="CorsRules"/>.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<CorsRule>)_corsRules).GetEnumerator();
        }

        /// <summary>
        /// Compares two <see cref="CorsRules" /> instances for equality.
        /// </summary>
        /// <param name="a">The first <see cref="CorsRules" /> to compare.</param>
        /// <param name="b">The second <see cref="CorsRules" /> to compare.</param>
        public static bool operator ==(CorsRules a, CorsRules b)
        {
            return EqualityComparer<CorsRules>.Default.Equals(a, b);
        }

        /// <summary>
        /// Compares two <see cref="CorsRules" /> instances for inequality.
        /// </summary>
        /// <param name="a">The first <see cref="CorsRules" /> to compare.</param>
        /// <param name="b">The second <see cref="CorsRules" /> to compare.</param>
        public static bool operator !=(CorsRules a, CorsRules b)
        {
            return !(a == b);
        }
    }
}
