using System;

using Microsoft.Extensions.Configuration;

using Bytewizer.Backblaze.Agent;
using Bytewizer.Backblaze.Client;
using Bytewizer.Backblaze.Handlers;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for setting up the agent in an <see cref="IServiceCollection" />.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the repository agent services to the collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">Delegate to define the configuration.</param>
        public static IBackblazeAgentBuilder AddBackblazeAgent(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            var options = configuration.Get<ClientOptions>();

            return AddBackblazeAgent(services, options);
        }

        /// <summary>
        /// Adds the repository agent services to the collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="setupBuilder">Delegate to define the configuration.</param>
        public static IBackblazeAgentBuilder AddBackblazeAgent(this IServiceCollection services, Action<IClientOptions> setupBuilder)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (setupBuilder == null)
                throw new ArgumentNullException(nameof(setupBuilder));

            var options = new ClientOptions();
            setupBuilder(options);

            return AddBackblazeAgent(services, options);
        }

        /// <summary>
        /// Adds the Backblaze client agent services to the collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="options">The agent options.</param>
        public static IBackblazeAgentBuilder AddBackblazeAgent(this IServiceCollection services, IClientOptions options)
        {

            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (options == null)
                throw new ArgumentNullException(nameof(options));

            options.Validate();
            services.AddSingleton(options);

            services.AddTransient<UserAgentHandler>();
            services.AddHttpClient<IApiClient, ApiClient>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(options.Timeout);
            })
            .AddHttpMessageHandler<UserAgentHandler>()
            .SetHandlerLifetime(TimeSpan.FromSeconds(options.HandlerLifetime));

            services.AddSingleton<IStorageClient, StorageService>();

            return new BackblazeAgentBuilder(services);
        }
    }
}
