using System.Collections.Generic;

using Microsoft.Extensions.Configuration;

namespace Bytewizer.Backblaze.Agent.Console
{
    /// <summary>
    /// Context containing the common services on the <see cref="IConsoleHost" />. Some properties may be null until set by the <see cref="IConsoleHost" />.
    /// </summary>
    public class ConsoleHostBuilderContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleHostBuilderContext"/> class.
        /// </summary>
        public ConsoleHostBuilderContext(IDictionary<object, object> properties)
        {
            Properties = properties ?? throw new System.ArgumentNullException(nameof(properties));
        }

        /// <summary>
        /// The <see cref="IConfiguration" /> containing the merged configuration of the application and the <see cref="IConsoleHost" />.
        /// </summary>
        public IConfiguration Configuration { get; set; }

        /// <summary>
        /// A central location for sharing state between components during the host building process.
        /// </summary>
        public IDictionary<object, object> Properties { get; }
    }
}