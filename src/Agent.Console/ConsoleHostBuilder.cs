using System;
using System.Collections.Generic;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bytewizer.Backblaze.Agent.Console
{
    class ConsoleHostBuilder : IConsoleHostBuilder
    {
        private List<Action<IConfigurationBuilder>> _configureHostConfigActions = new List<Action<IConfigurationBuilder>>();
        private List<Action<ConsoleHostBuilderContext, IConfigurationBuilder>> _configureAppConfigActions = new List<Action<ConsoleHostBuilderContext, IConfigurationBuilder>>();
        private List<Action<ConsoleHostBuilderContext, IServiceCollection>> _configureServicesActions = new List<Action<ConsoleHostBuilderContext, IServiceCollection>>();
        private bool _hostBuilt;
        private IServiceProvider _appServices;

        /// <summary>
        /// A central location for sharing state between components during the host building process.
        /// </summary>
        public IDictionary<object, object> Properties { get; } = new Dictionary<object, object>();

        /// <summary>
        /// Set up the configuration for the builder itself. This will be used to initialize the <see cref="IHostEnvironment"/>
        /// for use later in the build process. This can be called multiple times and the results will be additive.
        /// </summary>
        /// <param name="configureDelegate">The delegate for configuring the <see cref="IConfigurationBuilder"/> that will be used
        /// to construct the <see cref="IConfiguration"/> for the host.</param>
        /// <returns>The same instance of the <see cref="IConsoleHostBuilder"/> for chaining.</returns>
        public IConsoleHostBuilder ConfigureHost(Action<IConfigurationBuilder> configureDelegate)
        {
            _configureHostConfigActions.Add(configureDelegate ?? throw new ArgumentNullException(nameof(configureDelegate)));
            return this;
        }

        /// <summary>
        /// Sets up the configuration for the remainder of the build process and application. This can be called multiple times and
        /// the results will be additive. The results will be available at <see cref="ConsoleHostBuilderContext.Configuration"/> for
        /// subsequent operations, as well as in <see cref="IConsoleHost.Services"/>.
        /// </summary>
        /// <param name="configureDelegate">The delegate for configuring the <see cref="IConfigurationBuilder"/> that will be used
        /// to construct the <see cref="IConfiguration"/> for the host.</param>
        /// <returns>The same instance of the <see cref="IConsoleHostBuilder"/> for chaining.</returns>
        public IConsoleHostBuilder ConfigureAppConfiguration(Action<ConsoleHostBuilderContext, IConfigurationBuilder> configureDelegate)
        {
            _configureAppConfigActions.Add(configureDelegate ?? throw new ArgumentNullException(nameof(configureDelegate)));
            return this;
        }

        /// <summary>
        /// Adds services to the container. This can be called multiple times and the results will be additive.
        /// </summary>
        /// <param name="configureDelegate"></param>
        /// <returns>The same instance of the <see cref="IConsoleHostBuilder"/> for chaining.</returns>
        public IConsoleHostBuilder ConfigureServices(Action<ConsoleHostBuilderContext, IServiceCollection> configureDelegate)
        {
            _configureServicesActions.Add(configureDelegate ?? throw new ArgumentNullException(nameof(configureDelegate)));
            return this;
        }

        /// <summary>
        /// Run the given actions to initialize the host. This can only be called once.
        /// </summary>
        /// <returns>An initialized <see cref="IConsoleHost"/></returns>
        public IConsoleHost Build()
        {
            if (_hostBuilt)
            {
                throw new InvalidOperationException("Build can only be called once.");
            }
            _hostBuilt = true;

            //BuildHostConfiguration();
            //CreateHostingEnvironment();
            //CreateHostBuilderContext();
            //BuildAppConfiguration();
            //CreateServiceProvider();

            return _appServices.GetRequiredService<IConsoleHost>();
        }
    }
}
