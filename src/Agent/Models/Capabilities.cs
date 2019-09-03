using System;
using System.Collections.Generic;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Represents capability information which uses <see cref="HashSet{Capability}"/> and contains no duplicate capabilities.
    /// </summary>
    public class Capabilities : HashSet<Capability>, IEquatable<Capability>
    {
        public bool Equals(Capability other)
        {
            return base.Equals(other);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj as Capabilities);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
