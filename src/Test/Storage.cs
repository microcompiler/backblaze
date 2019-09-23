using Bytewizer.Backblaze.Storage;
using Bytewizer.Backblaze.Client;
using Microsoft.Extensions.Logging;

namespace Backblaze
{
    public class Storage
    {
        public readonly IBackblazeStorage Agent;
        public readonly ILogger Logger;

        public Storage(IBackblazeStorage agent, ILogger<Storage> logger)
        {
            Agent = agent;
            Logger = logger;
        }
    }
}