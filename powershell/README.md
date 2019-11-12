# Backblaze Agent for Powershell

[![version](https://img.shields.io/powershellgallery/v/Backblaze.Agent.svg)](https://www.powershellgallery.com/packages/Backblaze.Agent)
[![downloads](https://img.shields.io/powershellgallery/dt/Backblaze.Agent.svg?label=downloads)](https://www.powershellgallery.com/stats/packages/Backblaze.Agent?groupby=Version)

A complete PowerShell module for quickly issuing [Backblaze B2 Cloud Storage API](https://www.backblaze.com/b2/cloud-storage.html) calls.

## Features

- Full support for Backblaze B2 Cloud Storage API v2 including the following list of cmdlets:

1. Connect-Backblaze
1. Get-Bucket
1. Get-Key
1. Get-UploadUrl
1. Get-StorageItem
1. Hide-StorageItem
1. Invoke-FileDownload
1. Invoke-FileUpload
1. New-Bucket
1. New-Key
1. Remove-Bucket
1. Remove-Key
1. Remove-StorageItem
1. Set-Bucket

## Installation via Powershell

To install the Backblaze.Agent module run the following command:

```powershell
> Install-Module -Name Backblaze.Agent
```

---

To get started, connect to Blackblaze with "Connect-Backblaze" cmdlet:

```powershell
# Connect to Backblaze B2 Cloud Storage with default settings
Connect-Backblaze -KeyId '[KeyId]' -ApplicationKey '[ApplicationKey]'

# Connect with client options
$Options = [Bytewizer.Backblaze.Client.ClientOptions]::new()
$Options.Timeout = 900
Connect-Backblaze -KeyId '[KeyId]' -ApplicationKey '[ApplicationKey]' -Options $Options
```

An example of some commands in action:

```powershell
# Get all buckets
Get-Bucket

# Get bucket by id
Get-Bucket -BucketId '[BucketId]'

# Get all public buckets and cache request for 1 hour
$Timespan = New-TimeSpan -Hours 1
$Types = [Bytewizer.Backblaze.Models.BucketTypes]::new()
$Types.Add([Bytewizer.Backblaze.Models.BucketFilter]::All)
Get-Bucket -BucketTypes $Types -CacheTTL $Timespan

# Create a bucket
$Bucketinfo = [Bytewizer.Backblaze.Models.BucketInfo]::new()
$Bucketinfo['key'] = 'value'

$LifecycleRule = [Bytewizer.Backblaze.Models.LifecycleRule]::new()
$LifecycleRule.DaysFromHidingToDeleting = 6
$LifecycleRule.DaysFromUploadingToHiding = 5
$LifecycleRule.FileNamePrefix = 'backup/'

$LifecycleRules = [Bytewizer.Backblaze.Models.LifecycleRules]::new()
$LifecycleRules.Add($LifecycleRule)

$AllowedOrigins = [System.Collections.Generic.List[String]]::new()
$AllowedOrigins.Add('https')

$AllowedOperations = [System.Collections.Generic.List[String]]::new()
$AllowedOperations.Add('b2_download_file_by_id')
$AllowedOperations.Add('b2_download_file_by_name')

$AllowedHeaders = [System.Collections.Generic.List[String]]::new()
$AllowedHeaders.Add('range')

$ExposeHeaders  =  [System.Collections.Generic.List[String]]::new()
$ExposeHeaders.Add('x-bz-content-sha1')

$CorsRule = [Bytewizer.Backblaze.Models.CorsRule]::new('downloadFromAnyOrigin', $AllowedOrigins, $AllowedOperations, 3600)

$CorsRules = [Bytewizer.Backblaze.Models.CorsRules]::new()
$CorsRules.Add($CorsRule)

New-Bucket -BucketName 'my-bucket' -BucketType AllPrivate -BucketInfo $BucketInfo -CorsRules $CorsRules -LifecycleRules $LifecycleRules

# Remove a bucket
Remove-Bucket -BucketId '[BucketId]'

# Create a key
$Capabilities = [Bytewizer.Backblaze.Models.Capabilities]::AllControl()
New-Key -KeyName 'l' -Capabilities $Capabilities -ValidDurationInSeconds 3600

```
