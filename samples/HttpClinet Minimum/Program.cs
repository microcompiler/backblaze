using System;

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
                Client = BackblazeAgent.Initialize("[key_id]", "[application_key]");

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
}
