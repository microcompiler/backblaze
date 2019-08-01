using System;
using System.Collections.Generic;
using System.Text;

namespace Bytewizer.Backblaze.Models
{
    public class FileParts
    {
        public int PartNumber { get; set; }
        public long Position { get; set; }
        public long Length { get; set; }
    }
}
