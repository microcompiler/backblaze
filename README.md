# Backblaze Agent for .NET Core
The Backblaze Agent (client) for .NET Core is an implementation of the [Backblaze B2 Cloud Storage API](https://www.backblaze.com/b2/cloud-storage.html). Backblaze B2 Cloud Storage provides the cheapest cloud storage available on the internet. Backblaze B2 Cloud Storage is ¼ of the price of other storage providers. Give it a try as the first 10 GB of storage is free. 

## Features
- Full support for Backblaze B2 Cloud Storage API v2 including accounts, keys, buckets and files.
- Built on .NET Core, targeting .NET Standard 2.0, which means Backblaze Agent will work on Windows, Mac and Linux systems.
- Seamlessly intergrates with .NET Core Dependency Injection and HttpClientFactory to implement resilient requests.
- Large file support with low memory allocation.
- Native support of task based programming model (async/await).

For feature requests and bug reports, please [open an issue on GitHub](https://github.com/microcompiler/backblaze/issues/new).

## Installation via .NET CLI

To install Backblaze.Agent run the following command:

```bash
> dotnet add package Backblaze.Agent
```

## Getting Started
You will need an <strong>key_id</strong> and an <strong>application_key</strong> to configure Backblaze Agent. You can obtain these from the [Backblaze B2 Cloud Storage](https://www.backblaze.com/b2/cloud-storage.html) portal. See the [Sample Project](https://github.com/microcompiler/backblaze/tree/master/sample) for an example of how to use this packages.

*Work in Progress!* Whilst we encourage users to play with the samples and test programs, this project has not yet reached a stable state.

### Adding Backblaze Agent to Services ###

```CSharp
public void ConfigureServices(IServiceCollection services)
{
    // Register Backblaze Agent Service
    services.AddBackblazeAgent(options =>
    {
		options.KeyId = "[key_id]";
		options.ApplicationKey = "[application_key]";
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
## Microsoft Logging Integration

Install the Microsoft.Extensions.Logging packages:
```bash
> dotnet add package Microsoft.Extensions.Logging
> dotnet add package Microsoft.Extensions.Logging.Debug
```
Tracing to the Debug window can be enabled with the following code:
```CSharp
services.AddLogging(builder =>
{
    builder.AddDebug();
})
.Configure<LoggerFilterOptions>(options =>
    options.MinLevel = LogLevel.Trace
);
```
## Agent Options

```CSharp
services.AddBackblazeAgent(options =>
{
	options.KeyId = "[key_id]";
	options.ApplicationKey = "[application_key]";
});
```
The following table describes the [Agent Options](https://github.com/microcompiler/backblaze/blob/master/src/Agent/Agent/AgentOptions.cs) available:

| Option Name | Default | Description | 
| ----------- | ------- | ----------- |
| KeyId  | --- | <strong>Required - </strong> The key identifier used to authenticate. |
| ApplicationKey | --- | <strong>Required - </strong> The secret part of the key used to authenticate. |
| AgentTimeout | 600 | The time in seconds to wait before the client request times out. |
| AgentRetryCount | 3 | The number of times the client will retry failed requests before timing out.  |
| UploadCutoffSize | 200MB | File upload cutoff size for switching to chunked parts in bits. |
| UploadPartSize | 200MB | File upload part size in bits of chunked parts. |
| DownloadCutoffSize | 200MB | File download cutoff size for switching to chunked parts in bits. |
| DownloadPartSize | 200MB | File download part size in bits of chunked parts. |
| HandlerLifetime | 600 | The time in seconds that the message handler instance can be reused. |
| TestMode | --- | This is for testing use only and not recomended for production environments. |

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
All source, documentation, instructions and products of this project are provided as-is without warranty. No liability is accepted for any damage, data loss or costs incurred by its use. 

## Contributions
Contributions to this project are always welcome. Please consider forking this project on GitHub and sending a pull request to get your improvements added to the original project.