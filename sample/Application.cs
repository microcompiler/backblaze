using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

using Bytewizer.Backblaze.Cloud;

namespace Backblaze.Sample
{
    public class Application
    {
        public readonly IStorage _storage;
        public readonly ILogger _logger;

        public ProgressBar progress = new ProgressBar();
        
        public Application(IStorage storage, ILogger<Application> logger)
        {
            _storage = storage;
            _logger = logger;
        }

        public async Task MainAsync(string[] args, CancellationToken token)
        {
            // example:  blaze.exe upload C:/filefolder/samplefile.zip
            // example:  blaze.exe download C:/filefolder/samplefile.zip

            progress.ProgressChanged += (s, p) => progress.Report(p);

            if (args.Length == 0)
            {
                Usage();
                Exit(1);
            }

            var bucketlist = await _storage.Buckets.GetAsync();
            if (bucketlist == null) Exit(1);

            var bucket = bucketlist.ToList().First();

            switch (args[0].ToLower())
            {
                case "upload":
                    Console.WriteLine($"Uploading File {args[1]}.");

                    var uploadResults = await _storage.Files.UploadAsync(bucket.BucketId, args[1], args[1], progress, token);
                    if (uploadResults.IsSuccessStatusCode)
                    {
                        Console.WriteLine(" Done");
                        Exit(0);
                    }
                    else { Console.Error.WriteLine($"\nCould not complete upload: The API returned status '{uploadResults.Error.Message}'."); }

                    break;
                case "download":
                    Console.WriteLine($"Downloading File {args[1]}.");

                    var downloadResults = await _storage.Files.DownloadAsync(bucket.BucketName, args[1], args[1], progress, token);
                    if (downloadResults.IsSuccessStatusCode)
                    {
                        Console.WriteLine(" Done");
                        Exit(0);
                    }
                    else { Console.Error.WriteLine($"\nCould not complete upload: The API returned status '{downloadResults.Error.Message}'."); }

                    break;
                case "copy":         
                    var source = new DirectoryInfo(args[1]);
                    var files = source.EnumerateFiles(args[2], SearchOption.AllDirectories);

                    var parallelTasks = new List<Task>();
                    foreach (var file in files)
                    {

                        Console.WriteLine($"Uploading File {file.FullName}");

                        parallelTasks.Add(Task.Run(async () =>
                        {
                            progress = new ProgressBar();
                            await _storage.Files.UploadAsync(bucket.BucketId, file.FullName, file.FullName, progress, token);
                        }));

                        Console.WriteLine(Environment.NewLine);
                    }
                    await Task.WhenAll(parallelTasks);

                    break;
                default:
                    Console.WriteLine($"\nFailed to parse commands.");
                    break;
            }

            Exit(1);
        }

        public void Exit(int exitCode)
        {
            if (Debugger.IsAttached)
            {
                Console.WriteLine();
                Console.Write("Press a key to quit");
                Console.ReadKey();
                Console.WriteLine();
            }

            Environment.Exit(exitCode);
        }

        /// <summary>
        /// Help message for command line arguments.
        /// </summary>
        public static void Usage()
        {
            Version v = Assembly.GetExecutingAssembly().GetName().Version;
            string pname = Process.GetCurrentProcess().ProcessName;
            Console.WriteLine($"{pname} version {v.Major}.{v.Minor}.{v.Build} (r{v.Revision})");
            Console.WriteLine("Command line arguments:");
            Console.WriteLine("  help                show this help");
            Console.WriteLine("  examples            show command line examples");
            Console.WriteLine("  download            downloads a file or directory");
            Console.WriteLine("  upload              uploads a file or directory");
            Console.WriteLine("Options");
            Console.WriteLine("  -a, --a             option a");
            Console.WriteLine("  -b, --b             option b");
        }

        /// <summary>
        /// Example message for command line arguments.
        /// </summary>
        public static void Examples()
        {
            string pname = Process.GetCurrentProcess().ProcessName;
            Console.WriteLine("Examples:");
            Console.WriteLine($"  {pname} download c:\\path\\file.txt");
            Console.WriteLine($"  {pname} upload c:\\path\\file.txt");
        }
    }
}