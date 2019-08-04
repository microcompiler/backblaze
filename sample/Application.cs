using System;
using System.Threading;
using System.Threading.Tasks;

using Bytewizer.Backblaze.Agent;

namespace Backblaze.Sample
{
    public class Application
    {
        public readonly IBackblazeAgent _agent;

        public Application(IBackblazeAgent agent)
        {
            _agent = agent;
        }

        public async Task MainAsync(string[] args, CancellationToken token)
        {
            // example:  --upload 8ee1946e6ccf4f146cc20d1b C:/filefolder/samplefile.zip
            // example:  --download e6b1db7e-9749-4686-testbucket C:/filefolder/samplefile.zip

            var progress = new ProgressBar();
            progress.ProgressChanged += (s, p) => progress.Report(p);

            switch (args[0].ToLower())
            {
                case "--upload":
                case "-u":
                    // set command line args: args[1] = [bucketId], args[2] = [fileName]
                    Console.WriteLine($"Uploading File {args[2]}.");

                    var uploadResults = await _agent.Files.UploadAsync(args[1], args[2], args[2], progress, token);
                    if (uploadResults.IsSuccessStatusCode)
                    {
                        Console.WriteLine(" Done");
                        Environment.Exit(0);
                    }
                    else { Console.Error.WriteLine($"\nCould not complete upload: The API returned status '{uploadResults.Error.Message}'."); }

                    break;
                case "--download":
                case "-d":
                    // set command line args: args[1] = [bucketName], args[2] = [fileName]
                    Console.WriteLine($"Downloading File {args[2]}.");

                    var downloadResults = await _agent.Files.DownloadAsync(args[1], args[2], args[2], progress, token);
                    if (downloadResults.IsSuccessStatusCode)
                    {
                        Console.WriteLine(" Done");
                        Environment.Exit(0);
                    }
                    else {Console.Error.WriteLine($"\nCould not complete upload: The API returned status '{downloadResults.Error.Message}'."); }

                    break;
                default:
                    Console.WriteLine($"\nFailed to parse commands.");
                    break;
            }

            Environment.Exit(1);
        }
    }
}