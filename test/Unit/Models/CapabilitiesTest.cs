using Bytewizer.Backblaze.Models;

using Xunit;

namespace Backblaze.Tests.Unit
{
    public class CapabilitiesTest
    {
        [Fact]
        public void NoDuplicatesAllowed()
        {
            var Capabilities1 = new Capabilities
            {
                Capability.ListBuckets,
                Capability.ListBuckets,
                Capability.ListFiles,
                Capability.ReadFiles,
                Capability.ShareFiles,
                Capability.WriteFiles,
                Capability.DeleteFiles
            };

            var Capabilities2 = new Capabilities
            {
                Capability.ListBuckets,
                Capability.ListFiles,
                Capability.ReadFiles,
                Capability.ShareFiles,
                Capability.WriteFiles,
                Capability.DeleteFiles
            };

            Assert.True(Capabilities1.Equals(Capabilities2));
            Assert.Equal(Capabilities1.GetHashCode(), Capabilities2.GetHashCode());
        }
    }
}
