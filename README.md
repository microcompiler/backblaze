# Backblaze Agent for .NET Core

The Backblaze Agent (client) for .NET Core is an implementation of the [Backblaze B2 Cloud Storage API](https://www.backblaze.com/b2/cloud-storage.html). Backblaze B2 Cloud Storage provides the cheapest cloud object storage and transfer available on the internet. Backblaze B2 Cloud Storage is ¼ of the price of Amazon S3. Give it a try as the first 10 GB of storage is free. 

## Features
- Full support for Backblaze B2 Cloud Storage API v2 including accounts, keys, buckets and files.
- Built on .NET Core, targeting .NET Standard 2.0, which means Backblaze Agent will work on Windows, Mac and Linux systems.
- Seamlessly intergrates with .NET Core Dependency Injection and HttpClientFactory to implement resilient requests.
- Large file support with low memory allocation.
- Native support of task based programming model (async/await).

## Installation

To install Backblaze.Agent run the following command:

### Install via .NET CLI

```bash
> dotnet add package Backblaze.Agent
```

### Install via NuGet

```bash
PM> Install-Package Backblaze.Agent
```

## Getting Started
You will need an key_id and an application_key to run Backblaze.Agent. You can obtain these in the Backblaze B2 portal.

### Adding Backblaze Agent to Services ###

```CSharp
public void ConfigureServices(IServiceCollection services)
{
    // Register Backblaze Agent Service
    services.AddBackblazeAgent(options =>
    {
        options.KeyId = "[key_id]"
        options.ApplicationKey = "[application_key]"
    });
}
```

### Injecting Backblaze Client Agent ###

To get a list of backblaze buckets simply inject `IBackblazeAgent` into your class and call the async client.  

```csharp
 public class IndexModel : PageModel
{
    private readonly IBackblazeAgent _apiService;

    [BindProperty]
    public IEnumerable<BucketObject> Buckets { get; private set; }

    public IndexModel(IBackblazeAgent apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var response = await _apiService.Buckets.GetAsync();
        if (response.IsSuccessStatusCode)
        {
            Buckets = response.Response.Buckets;

            if (Buckets == null)
            {
                return NotFound();
            }
        }
        return Page();
    }
}
```

## Basic Usage

```CSharp
await Agent.Files.UploadAsync(
    bucketId,
    @"c:\test\file.zip",
    null,
    TokenSource.Token);
```

## Contributions
Contributions to this project are always welcome. Please consider forking this project on GitHub and sending a pull request to get your improvements added to the original project.