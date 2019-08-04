using System;
using System.Collections.Generic;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Represents life cycle rule information which uses <see cref="List{LifeCycleRule}"/> limited to 100 items.
    /// </summary>
    public class LifecycleRules : List<LifecycleRule>
    {
        /// <summary>
        /// Adds the specified value to <see cref="LifecycleRule"/> limited to 100 items.
        /// </summary>
        /// <param name="value">The value of the rule to add.</param>
        public new void Add(LifecycleRule value)
        {
            if (Count >= 100)
                throw new InvalidOperationException(
                          "This list is limited to 100 items. You cannot add more items.");

            base.Add(value);
        }
    }
}
