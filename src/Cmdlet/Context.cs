using Bytewizer.Backblaze.Client;

namespace Bytewizer.Backblaze.Cmdlet
{
    /// <summary>
    /// The static context used by the modules.  The 'Context' is set with the <see cref="ConnectBackblaze"/> cmdlet.
    /// </summary>
    public static class Context
    {
        /// <summary>
        /// The main <see cref="BackblazeClient"/> from which all cmdlets issue their requests.
        /// </summary>
        public static IStorageClient Client { get; internal set; }

        /// <summary>
        /// Returns true when <see cref="Context.Client"/> is not null.
        /// </summary>
        public static bool IsConnected => Client != null;
    }
}
