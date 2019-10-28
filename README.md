# Backblaze Agent for .NET Core
[![NuGet Version](https://img.shields.io/nuget/vpre/Backblaze.Agent.svg?style=flat-square)](https://www.nuget.org/packages/Backblaze.Agent)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Backblaze.Agent.svg?style=flat-square)](https://www.nuget.org/packages/Backblaze.Agent)

The Backblaze Agent (client) for .NET Core is an implementation of the [Backblaze B2 Cloud Storage API](https://www.backblaze.com/b2/cloud-storage.html). Backblaze B2 Cloud Storage provides the cheapest cloud storage available on the internet. Backblaze B2 Cloud Storage is ¼ of the price of other storage providers. Give it a try as the first 10 GB of storage is free. 

## Features
- Full support for Backblaze B2 Cloud Storage API v2 including accounts, keys, buckets and files.
- Built targeting .NET Standard 2.0 which means Backblaze Agent will work on Windows, Mac and Linux systems.
- Seamlessly intergrates with .NET Core Dependency Injection and HttpClientFactory to implement resilient requests.
- Simple in-memory response cache using MemoryCache.
- Large file support with low memory allocation.
- Native support of task based programming model (async/await).

For feature requests and bug reports, please [open an issue on GitHub](https://github.com/microcompiler/backblaze/issues/new).

## Installation via .NET CLI

To install Backblaze.Agent run the following command:

```bash
> dotnet add package Backblaze.Agent
```

## Getting Started

*Work in Progress!* Whilst we encourage users to play with the samples and test programs, this project has not yet reached a stable state.

You will need an <strong>key_id</strong> and an <strong>application_key</strong> to configure Backblaze Agent. You can obtain these from the [Backblaze B2 Cloud Storage](https://www.backblaze.com/b2/cloud-storage.html) portal. See the [Sample Project](https://github.com/microcompiler/backblaze/tree/master/samples ) for an example of how to use this packages.

### Adding Backblaze Agent to Services ###

```CSharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddBackblazeAgent(options =>
    {
		options.KeyId = "[key_id]";
		options.ApplicationKey = "[application_key]";
    });
}
```

To get a list of backblaze buckets simply inject `IStorageAgent` into your class and call the async client.  

```csharp
public class IndexModel : PageModel
{
  private readonly IStorageAgent _storage;

  public IndexModel(IStorageAgent storage)
  {
      _storage = storage;
  }

  [BindProperty]
  public IEnumerable<BucketItem> Buckets { get; private set; }

  public async Task<IActionResult> OnGetAsync()
  {
    Buckets = await _storage.Buckets.GetAsync();

    if (Buckets == null)
    {
      return NotFound();
    }

    return Page();
  }
}
```
### Initializing Backblaze Client without Dependency Injection ###
Install the following packages:
```bash
> dotnet add package Backblaze.Client
> dotnet add package Microsoft.Extensions.Caching.Memory
> dotnet add package Microsoft.Extensions.Logging.Debug
```
Sample Code:
```CSharp
class Program
{
  private static BackblazeAgent Client;

  static async Task Main(string[] args)
  {
    try
    {
      var options = new ClientOptions();

      var loggerFactory = LoggerFactory.Create(builder =>
      {
        builder
          .AddFilter("Bytewizer.Backblaze", LogLevel.Trace)
          .AddDebug();
      });
 
      var cache = new MemoryCache(new MemoryCacheOptions());

      Client = new BackblazeAgent(options, loggerFactory, cache);  
      await Client.ConnectAsync("[key_id]", "[application_key]");

      var buckets = await Client.Buckets.GetAsync();

      foreach (var bucket in buckets)
        Console.WriteLine($"Bucket Name: {bucket.BucketName} - Type: {bucket.BucketType}");   
    }
    catch (Exception ex)
    {
      Console.Error.WriteLine(ex.Message);
    }
  }
}
```
### Initializing Backblaze Client without Dependency Injection, Logging or Caching ###
Install the following package:
```bash
> dotnet add package Backblaze.Client
```
Sample Code:
```CSharp
class Program
{
  private static BackblazeAgent Client;

  static void Main(string[] args)
  {
    try
    {
      Client = new BackblazeAgent();
      Client.Connect("[key_id]", "[application_key]");
      
      var buckets = Client.Buckets.GetAsync().GetAwaiter().GetResult();

      foreach (var bucket in buckets)
        Console.WriteLine($"Bucket Name: {bucket.BucketName} - Type: {bucket.BucketType}");
    }
    catch (Exception ex)
    {
      Console.Error.WriteLine(ex.Message);
    }
  }
}
```
## Basic Usage

Upload File Stream
```CSharp
foreach (var filePath in Directory.GetFiles(@"c:\my\directory"))
{
  using (var stream = File.OpenRead(filePath))
  {
    var results = await Client.UploadAsync("[BucketId]", new FileInfo(filePath).Name, stream);
  }
}
```

## Microsoft Logging Integration

Install the Microsoft.Extensions.Logging packages:
```bash
> dotnet add package Microsoft.Extensions.Logging.Debug
```
Tracing to the Debug window can be enabled with the following code:
```CSharp
services.AddLogging(builder =>
{
    builder.AddDebug();
    builder.AddFilter("Bytewizer.Backblaze", LogLevel.Trace);
});
```

## Agent Options

```CSharp
services.AddBackblazeAgent(options =>
{
	options.KeyId = "[key_id]";
	options.ApplicationKey = "[application_key]";
});
```
The following table describes the [Agent Options](https://github.com/microcompiler/backblaze/blob/master/src/Client/Client/IClientOptions.cs) available:

| Option Name | Default | Description | 
| ----------- | ------- | ----------- |
| KeyId  | --- | <strong>Required - </strong> The key identifier used to authenticate. |
| ApplicationKey | --- | <strong>Required - </strong> The secret part of the key used to authenticate. |
| HandlerLifetime | 600 | The time in seconds that the message handler instance can be reused. |
| Timeout | 600 | The time in seconds to wait before the client request times out. |
| RetryCount | 5 | The number of times the client will retry failed requests before timing out. |
| RequestMaxParallel | 10 | The maximum number of parallel request connections established. |
| DownloadMaxParallel | 5 | The maximum number of parallel download connections established. |
| DownloadCutoffSize | 100MB | Download cutoff size for switching to chunked parts in bytes. |
| DownloadPartSize | 100MB | Download part size of chunked parts in bytes. |
| UploadMaxParallel | 3 | The maximum number of parallel upload connections established. |
| UploadCutoffSize | 100MB | Upload cutoff size for switching to chunked parts in bytes. |
| UploadPartSize | 100MB | Upload part size of chunked parts in bytes. |
| AutoSetPartSize  | false | Use the recommended part size returned by the Backblaze service. |
| ChecksumDisabled  | false | This is for testing use only and not recomended for production environments. |
| TestMode  | --- | This is for testing use only and not recomended for production environments. |

### Test Mode Options

```CSharp
services.AddBackblazeAgent(options =>
{
	// This is for testing use only and not recomended for production environments. 
	options.TestMode = "fail_some_uploads";  
});
```
The following test mode options are available to verify that your code correctly handles error conditions.

| Option String | Description | 
| ------------ | -------------------------------------------------------------------- |
| fail_some_uploads| Random uploads fail or become rejected by the service. |
| expire_some_account_authorization_tokens | Random account authorization tokens expire. |
| force_cap_exceeded |Cap exceeded conditions are forced. |


## Disclaimer
All source, documentation, instructions and products of this project are provided as-is without warranty. No liability is accepted for any damages, data loss or costs incurred by its use. 

## Contributions
Contributions to this project are always welcome. Please consider forking this project on GitHub and sending a pull request to get your improvements added to the original project.