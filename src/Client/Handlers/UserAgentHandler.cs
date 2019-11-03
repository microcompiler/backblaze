using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Bytewizer.Backblaze.Handlers
{
    /// <summary>
    /// The user agent handler.
    /// </summary>
    public sealed class UserAgentHandler : DelegatingHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserAgentHandler" /> class.
        /// </summary>
        public UserAgentHandler()
            : this(Assembly.GetEntryAssembly())
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="UserAgentHandler" /> class.
        /// </summary>
        /// <param name="assembly">An assembly of a common language runtime application.</param>
        public UserAgentHandler(Assembly assembly)
            : this(GetProduct(assembly), GetVersion(assembly))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAgentHandler" /> class.
        /// </summary>
        public UserAgentHandler(string applicationName, string applicationVersion)
        {
            if (applicationName == null)
                throw new ArgumentNullException(nameof(applicationName));

            if (applicationVersion == null)
                throw new ArgumentNullException(nameof(applicationVersion));

            UserAgentValues = new List<ProductInfoHeaderValue>
            {
                new ProductInfoHeaderValue(applicationName.Replace(' ', '-'), applicationVersion),
                new ProductInfoHeaderValue($"({Environment.OSVersion})"),
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAgentHandler" /> class.
        /// </summary>
        public UserAgentHandler(List<ProductInfoHeaderValue> userAgentValues) =>
            UserAgentValues = userAgentValues ?? throw new ArgumentNullException(nameof(userAgentValues));

        /// <summary>
        /// Gets or sets user agent values.
        /// </summary>
        public List<ProductInfoHeaderValue> UserAgentValues { get; set; }

        /// <summary>
        /// Add the user agent to the outgoing request.
        /// </summary>
        /// <param name="request">The HTTP request message.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (!request.Headers.UserAgent.Any())
            {
                foreach (var userAgentValue in this.UserAgentValues)
                {
                    request.Headers.UserAgent.Add(userAgentValue);
                }
            }

            // else the header has already been added due to a retry.
            return base.SendAsync(request, cancellationToken);
        }

        /// <summary>
        /// Get product info.
        /// </summary>
        /// <param name="assembly">An assembly of a common language runtime application.</param>
        private static string GetProduct(Assembly assembly) => GetAttributeValue<AssemblyProductAttribute>(assembly);

        /// <summary>
        /// Get version info.
        /// </summary>
        /// <param name="assembly">An assembly of a common language runtime application.</param>
        private static string GetVersion(Assembly assembly)
        {
            var infoVersion = GetAttributeValue<AssemblyInformationalVersionAttribute>(assembly);
            if (infoVersion != null)
            {
                return infoVersion;
            }

            return GetAttributeValue<AssemblyFileVersionAttribute>(assembly);
        }

        /// <summary>
        /// Get attribute values.
        /// </summary>
        /// <typeparam name="T">Attribute type.</typeparam>
        /// <param name="assembly">An assembly of a common language runtime application.</param>
        private static string GetAttributeValue<T>(Assembly assembly)
            where T : Attribute
        {
            var type = typeof(T);
            var attribute = assembly
                .CustomAttributes
                .Where(x => x.AttributeType == type)
                .Select(x => x.ConstructorArguments.FirstOrDefault())
                .FirstOrDefault();
            return attribute == null ? string.Empty : attribute.Value.ToString();
        }
    }
}
