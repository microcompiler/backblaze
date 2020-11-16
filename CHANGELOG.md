# Change Log

## 0.9.4 - November 15, 2020

*  Added 'listAllBucketNames' application key capability name
*  Allow project to load with any version of dotnet 3.1

## 0.9.3 - January 25, 2020

*   Added suport for github actions for both CI and release builds
*   Added BucketFilter enum type to bucket requests

## 0.9.2 - November 3, 2019

*   Fixed hash issue with large file uploads
*   Fixed issue with ContentSha1 responses
*   Added .ConfigureAwait(false) to all Task methods
*   Moved test unit tests to xUnit allowing parallel test
*   Fixed general code quality in several areas

## 0.9.1 - October 27, 2019

*   Added support for HttpClient initialization
*   Fixed #6 - Ability to initialize the client without dependency injection
*   Fixed several know issues
*   Published Backblaze.Client package to NuGet
*   Delisted 'Backblaze.Agent.Console' from package listings

## 0.8.5 - September 25, 2019

*   Added intellisense support in nuget package

## 0.8.4 - September 24, 2019

*   Fixed package build error

## 0.8.3 - September 24, 2019

*   Fixed #5 - UriFormatExceptionSystem thrown when calling UploadAsync
*   Fixed several know issues
*   Added caching for ListBuckets, ListFileNames, ListFileVersions, ListKeys and ListParts methods
*   Added iterator adapters for ListBuckets(), ListFileNames(), ListFileVersions(), ListKeys() and ListParts() methods
*   Added code comments for all interfaces
*   Added configuration support for AddBackblazeAgent()
*   Added error checking support for several models

## 0.8.2 - August 18, 2019

*   General code cleanup and documentation
*   Added caching for upload_url and upload_part_url
*   Added option configuration support using setting.json
*   Fixed several know issues
 
## 0.8.1 - August 3, 2019

*   Added additional error checking and trace logging
*   Added http retry for expired tokens
*   Added BucketInfo, Fileinfo, CorsRules and LifecycleRules validation objects
*   Added console progress bar for Iprogress<> upload/download status
*   Fixed several know issues

## 0.8.0 - Jul 31, 2019

*   First public preview
