using System;
using System.Security.Authentication;
using System.Management.Automation;

using Microsoft.Extensions.DependencyInjection;

using Bytewizer.Backblaze.Models;
using Bytewizer.Backblaze.Client;
using System.Threading.Tasks;

namespace Bytewizer.Backblaze.Cmdlet
{
    /// <summary>
    /// <para type="synopsis">Builds the connection context for subsequent cmdlets.</para>
    /// <para type="description">Establishes a <see cref="BackblazeClient"/> context for use with all subsequent cmdlets. </para> 
    /// If this command is not run first, all other cmdlets will throw an error.
    /// </summary>
    [Cmdlet(VerbsCommunications.Connect, "Backblaze", ConfirmImpact = ConfirmImpact.None)]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(AuthorizeAccountResponse))]
    public class ConnectBackblaze: BaseCmdlet
    {
        #region Public Properties

        /// <summary>
        /// The identifier for the key.
        /// </summary>
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public string KeyId { get; set; }

        /// <summary>
        /// The secret part of the key. You can use either the master application key or a normal application key.
        /// </summary>
        [Parameter(Mandatory = true, Position = 1, ValueFromPipeline = true)]
        public string ApplicationKey { get; set; }

        /// <summary>
        /// Client configuration options.
        /// </summary>
        [Parameter(Mandatory = false, Position = 2, ValueFromPipeline = true)]
        public ClientOptions Options { get; set; }

        #endregion

        #region Cmdlet Processing

        /// <summary>
        /// Executes the process pipeline.
        /// </summary>
        protected override void BeginProcessing()
        {
            var services = new ServiceCollection();

            services.AddBackblazeAgent(options =>
            {
                options.KeyId = KeyId;
                options.ApplicationKey = ApplicationKey;
            });

            services.AddMemoryCache();

            var serviceProvider = services.BuildServiceProvider();

            Context.Client = serviceProvider.GetService<IStorageClient>();
        }

        /// <summary>
        /// Executes the process pipeline.
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                Task<AuthorizeAccountResponse> task = Context.Client.ConnectAsync();
                task.Wait();

                WriteObject(task.Result);
            }
            catch (AuthenticationException ex)
            {
                WriteError(new ErrorRecord(ex, null, ErrorCategory.AuthenticationError, Context.Client));
            }
            catch (ApiException ex)
            {
                WriteError(new ErrorRecord(ex, null, ErrorCategory.InvalidResult, Context.Client));
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, null, ErrorCategory.InvalidOperation, Context.Client));
            }
        }

        #endregion
    }
}
