using System;
using System.Collections;
using System.Collections.Generic;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Represents a collection of <see cref="Capability"/> that contains no duplicates.
    /// </summary>
    public class Capabilities :  ICollection<Capability>
    {
        private readonly HashSet<Capability> _capability;

        /// <summary>
		/// Initializes a new instance of the <see cref="Capabilities" /> class.
		/// </summary>
        public Capabilities()
        {
            _capability = new HashSet<Capability>();
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="Capabilities" />.
        /// </summary>
        public int Count => _capability.Count;

        /// <summary>
        /// Gets a value indicating whether the <see cref="Capabilities" /> is read-only.
        /// </summary>
        public bool IsReadOnly => false;

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
    }
}
