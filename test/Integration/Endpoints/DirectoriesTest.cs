using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using System.IO.Abstractions.TestingHelpers;

using Bytewizer.Backblaze.Client;
using Bytewizer.Backblaze.Models;
using Bytewizer.Backblaze.Extensions;
using Bytewizer.Backblaze.Enumerables;

using Xunit;

namespace Backblaze.Tests.Integration
{
    [Collection("Sequential")]
    public class DirectoriesTest : BaseFixture
    {
        private static readonly MockFileSystem _fileSystem = new MockFileSystem();

        public DirectoriesTest(StorageClientFixture fixture)
            : base(fixture)
        {
            _fileSystem.AddFile(@"c:\root-five-bytes.bin", new MockFileData(new byte[] { 0x01, 0x34, 0x56, 0xd2, 0xd2 }));
            _fileSystem.AddFile(@"c:\matrix\five-bytes.bin", new MockFileData(new byte[] { 0x02, 0x34, 0x56, 0xd2, 0xd2 }));
            _fileSystem.AddFile(@"c:\shawshank\five-bytes.bin", new MockFileData(new byte[] { 0x03, 0x34, 0x56, 0xd2, 0xd2 }));
        }
    }
}
