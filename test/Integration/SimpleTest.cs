using System.Threading.Tasks;

using Xunit;

namespace Backblaze.Tests.Integration
{
    public class SimpleTest
    {
        [Fact]
        public async Task CreateAsync()
        {
            await Task.CompletedTask;
            Assert.True(true);
        }
    }
}
