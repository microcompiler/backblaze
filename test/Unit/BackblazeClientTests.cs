using System.Net.Http;

using Bytewizer.Backblaze.Client;

using Microsoft.Extensions.Caching.Memory;

using Xunit;

using NSubstitute;

namespace Backblaze.Tests.Unit
{
	public class BackblazeClientTests
	{
		[Fact]
		public void BackblazeClient_constructor_does_not_crash_on_null_loggerFactory()
		{
			// Arrange
			var clientOptions = new ClientOptions();

			var dummyMemoryCache = Substitute.For<IMemoryCache>();

			// Act & Assert
      new BackblazeClient(clientOptions, logger: null, dummyMemoryCache);
		}

		[Fact]
		public void BackblazeClient_constructor_with_HttpClient_does_not_crash_on_null_loggerFactory()
		{
			// Arrange
			var clientOptions = new ClientOptions();

			var httpClient = new HttpClient();

			var dummyMemoryCache = Substitute.For<IMemoryCache>();

			// Act & Assert
      new BackblazeClient(httpClient, clientOptions, logger: null, dummyMemoryCache);
		}
	}
}
