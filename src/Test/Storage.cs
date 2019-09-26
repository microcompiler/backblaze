using Bytewizer.Backblaze.Cloud;

using Microsoft.Extensions.Logging;

namespace Backblaze
{
    public class Storage
    {
        public readonly IStorage Agent;
        public readonly ILogger Logger;

        public Storage(IStorage agent, ILogger<Storage> logger)
        {
            Agent = agent;
            Logger = logger;
        }
    }
}