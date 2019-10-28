using System;
using System.IO;
using System.Linq;
using Bytewizer.Backblaze.Client;

namespace Bytewizer.Backblaze.Samples
{
    class Program
    {
        // Note: Run with Control + F5 to keep the console window open

        private static BackblazeAgent Client;

        static void Main(string[] args)
        {
            try
            {
                Client = BackblazeAgent.Initialize("e14ecff4c2db", "0007eb0f509d3f8d7b40f8594b10ea501dd48303e8");

                var buckets = Client.Buckets.GetAsync().GetAwaiter().GetResult();

                foreach (var bucket in buckets)
                    Console.WriteLine($"Bucket Name: {bucket.BucketName} - Type: {bucket.BucketType}");

                foreach (var filepath in Directory.GetFiles(@"c:\my\directory"))
                {
                    using (var stream = File.OpenRead(filepath)) { 
                        var results = Client.UploadAsync(buckets.ToList().First().BucketId, new FileInfo(filepath).Name, stream).GetAwaiter().GetResult();
                        Console.WriteLine(results.Response.FileName);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }
    }
}
