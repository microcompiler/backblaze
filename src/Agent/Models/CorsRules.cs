using System;
using System.Collections.Generic;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Represents cors rule information which uses <see cref="List{CorsRule}"/> limited to 100 items.
    /// </summary>
    public class CorsRules : List<CorsRule>
    {
        /// <summary>
        /// Adds the specified value to <see cref="CoresRules"/> limited to 100 items.
        /// </summary>
        /// <param name="value">The value of the rule to add.</param>
        public new void Add(CorsRule value)
        {
            if (Count >= 100)
                throw new InvalidOperationException(
                          "This list is limited to 100 items. You cannot add more items.");

            base.Add(value);
        }
    }
}
