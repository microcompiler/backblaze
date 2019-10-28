using System;

using Bytewizer.Backblaze.Models;

using Xunit;

namespace Backblaze.Tests.Unit
{
    public class FileInfoTest
    {
        [Fact]
        public void FileInfoEquatable()
        {
            var fileinfo1 = new BucketInfo
                {
                    { "key1", "value1" },
                    { "key2", "value2" }
                };

            var fileinfo2 = new BucketInfo
                {
                    { "key1", "value1" },
                    { "key2", "value2" }
                };

            Assert.True(fileinfo1.Equals(fileinfo2));
            Assert.Equal(fileinfo1.GetHashCode(), fileinfo2.GetHashCode());
        }

        [Fact]
        public void MaximumItemsAllowed()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var list = new FileInfo();

                for (int i = 0; i < FileInfo.MaximumFileInfoItemsAllowed + 1; i++)
                {
                    list.Add(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
                }
            });
        }
    }
}
