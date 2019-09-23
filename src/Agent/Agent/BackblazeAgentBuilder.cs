using System;

using Microsoft.Extensions.DependencyInjection;

namespace Bytewizer.Backblaze.Agent
{
    /// <summary>
    /// Backblaze B2 Cloud Storage agent helper class for DI configuration.
    /// </summary>
    public class BackblazeAgentBuilder : IBackblazeAgentBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BackblazeAgentBuilder"/> class.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <exception cref="System.ArgumentNullException">services</exception>
        public BackblazeAgentBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        /// <summary>
        /// Gets the service collection.
        /// </summary>
        public IServiceCollection Services { get; }
    }
}
