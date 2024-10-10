using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using Bytewizer.Backblaze.Client;
using Bytewizer.Backblaze.Client.Internal;

using Xunit;

using Bogus;

using NSubstitute;

using FluentAssertions;

using Bytewizer.Backblaze.Models;

public class ApiClientFixture
{
    [Fact]
    public async Task AuthorizeAccountAync_should_parse_capabilities()
    {
        // Arrange
        var faker = new Faker();

        var dummyKeyId = faker.Random.Hash();
        var dummyApplicationKey = faker.Random.Hash();
        var dummyAccountId = faker.Random.Hash();
        var dummyAuthorizationToken = faker.Random.Hash();
        var dummyApiUrl = new Uri(faker.Internet.Url());
        var dummyDownloadUrl = new Uri(faker.Internet.Url());
        var dummyCapabilities = faker.PickRandom(Enum.GetValues(typeof(Capability)).OfType<Capability>(), 5).ToList();
        var dummyCapabilityStrings = dummyCapabilities.Select(c => c.ToString()).ToList();

        var mockHttpClient = new HttpClient();

        var clientOptions = new ClientOptions();

        clientOptions.RequestMaxParallel = 1;
        clientOptions.DownloadMaxParallel = 1;
        clientOptions.UploadMaxParallel = 1;
        clientOptions.TestMode = "";

        var mockLogger = Substitute.For<ILogger<ApiRest>>();

        var mockMemoryCache = Substitute.For<IMemoryCache>();

        using (var testServer = new TestServer())
        {
            testServer
                .When("/b2_authorize_account")
                .Then(
                    new AuthorizeAccountResponseRaw()
                    {
                        AccountId = dummyAccountId,
                        AuthorizationToken = dummyAuthorizationToken,
                        ApiUrl = dummyApiUrl,
                        DownloadUrl = dummyDownloadUrl,
                        Allowed =
                            new AllowedRaw()
                            {
                                Capabilities = dummyCapabilityStrings
                            }
                    });

            var sut = new ApiClient(mockHttpClient, clientOptions, mockLogger, mockMemoryCache);

            sut.AccountInfo.AuthUrl = testServer.BaseUrl;

            // Act
            var result = await sut.AuthorizeAccountAync(dummyKeyId, dummyApplicationKey, CancellationToken.None);

            // Assert
            result.IsSuccessStatusCode.Should().BeTrue();
            result.Response.Should().NotBeNull();
            result.Response.Allowed.Should().NotBeNull();
            result.Response.Allowed.Capabilities.Should().BeEquivalentTo(dummyCapabilities);
            result.Response.Allowed.UnknownCapabilities.Should().BeEmpty();
        }
    }

    [Fact]
    public async Task AuthorizeAccountAync_should_parse_unknown_capabilities()
    {
        // Arrange
        var faker = new Faker();

        var dummyKeyId = faker.Random.Hash();
        var dummyApplicationKey = faker.Random.Hash();
        var dummyAccountId = faker.Random.Hash();
        var dummyAuthorizationToken = faker.Random.Hash();
        var dummyApiUrl = new Uri(faker.Internet.Url());
        var dummyDownloadUrl = new Uri(faker.Internet.Url());
        var dummyCapabilities = faker.PickRandom(Enum.GetValues(typeof(Capability)).OfType<Capability>(), 5).ToList();
        var dummyCapabilityStrings = dummyCapabilities.Select(c => c.ToString()).ToList();

        var unknownDummyCapabilities =
            new[]
            {
                "UnknownCapability1",
                "UnknownCapability2",
            };

        dummyCapabilityStrings.AddRange(unknownDummyCapabilities);

        var mockHttpClient = new HttpClient();

        var clientOptions = new ClientOptions();

        clientOptions.RequestMaxParallel = 1;
        clientOptions.DownloadMaxParallel = 1;
        clientOptions.UploadMaxParallel = 1;
        clientOptions.TestMode = "";

        var mockLogger = Substitute.For<ILogger<ApiRest>>();

        var mockMemoryCache = Substitute.For<IMemoryCache>();

        using (var testServer = new TestServer())
        {
            testServer
                .When("/b2_authorize_account")
                .Then(
                    new AuthorizeAccountResponseRaw()
                    {
                        AccountId = dummyAccountId,
                        AuthorizationToken = dummyAuthorizationToken,
                        ApiUrl = dummyApiUrl,
                        DownloadUrl = dummyDownloadUrl,
                        Allowed =
                            new AllowedRaw()
                            {
                                Capabilities = dummyCapabilityStrings
                            }
                    });

            var sut = new ApiClient(mockHttpClient, clientOptions, mockLogger, mockMemoryCache);

            sut.AccountInfo.AuthUrl = testServer.BaseUrl;

            // Act
            var result = await sut.AuthorizeAccountAync(dummyKeyId, dummyApplicationKey, CancellationToken.None);

            // Assert
            result.IsSuccessStatusCode.Should().BeTrue();
            result.Response.Should().NotBeNull();
            result.Response.Allowed.Should().NotBeNull();
            result.Response.Allowed.Capabilities.Should().BeEquivalentTo(dummyCapabilities);
            result.Response.Allowed.UnknownCapabilities.Should().BeEquivalentTo(unknownDummyCapabilities);
        }
    }
}
