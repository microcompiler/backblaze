using System;
using System.Threading;
using System.Management.Automation;

namespace Bytewizer.Backblaze.Cmdlet
{
    /// <summary>
    /// The main base <see cref="PSCmdlet"/> class for all cmdlets.
    /// </summary>
    public abstract class BaseCmdlet : PSCmdlet
    {
        /// <summary>
        /// Executes the process pipeline.
        /// </summary>
        protected override void BeginProcessing()

        {
            if (!Context.IsConnected)
                throw new ContextNotSetException();

            Context.Client.CancellationToken = CancellationToken.None;

            Console.CancelKeyPress += (source, e) =>
            {
                Console.Out.WriteLine("CTRL-C pressed...");
                var cts = new CancellationTokenSource();
                e.Cancel = true;
                cts.Cancel();
                Context.Client.CancellationToken = cts.Token;
            };
        }
    }
}
