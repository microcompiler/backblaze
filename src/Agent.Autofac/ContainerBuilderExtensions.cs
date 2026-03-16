using System;
using System.Net.Http;

using Autofac;

using Bytewizer.Backblaze.Client;
using Bytewizer.Backblaze.Handlers;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DQD.Backblaze.Agent.Autofac
{
    /// <summary>
    /// Extension methods for setting up the agent in an Autofac <see cref="ContainerBuilder" />.
    /// </summary>
    public static class ContainerBuilderExtensions
    {
        /// <summary>
        /// Adds the repository agent services to the collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="setupBuilder">Delegate to define the configuration.</param>
        public static ContainerBuilder AddBackblazeAgent(this ContainerBuilder services, Action<IClientOptions> setupBuilder)
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
        public static ContainerBuilder AddBackblazeAgent(this ContainerBuilder services, IClientOptions options)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (options == null)
                throw new ArgumentNullException(nameof(options));

            options.Validate();

            services.RegisterInstance(options).AsImplementedInterfaces();

            services.RegisterType<UserAgentHandler>();
            services.RegisterType<HttpErrorHandler>();

            services.RegisterType<StorageService>().SingleInstance();

            services.RegisterType<HttpClient>();

            var memoryCacheOptions = new MemoryCacheOptions();

            services.RegisterInstance(Options.Create(memoryCacheOptions)).AsImplementedInterfaces();

            services.RegisterInstance(LoggerFactory.Create(_ => { })).AsImplementedInterfaces().IfNotRegistered(typeof(ILoggerFactory));
            services.RegisterType<MemoryCache>().AsImplementedInterfaces();

            services.RegisterType<BackblazeClient>().As<IStorageClient>();

            return services;
        }
    }
}
