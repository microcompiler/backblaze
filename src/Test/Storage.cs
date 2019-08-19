using Bytewizer.Backblaze.Agent;
using Bytewizer.Backblaze.Client;
using Microsoft.Extensions.Logging;

namespace Backblaze.Test
{
    public class Storage
    {
        public readonly IBackblazeAgent Agent;
        public readonly ILogger Logger;

        public Storage(IBackblazeAgent agent, ILogger<Storage> logger)
        {
            Agent = agent;
            Logger = logger;
        }
    }
}