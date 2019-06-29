using System;
using System.Collections.Generic;
using System.Text;

namespace Conductor.Domain.Models
{
    public class Resource
    {
        //public Bucket Bucket { get; set; }

        public string Name { get; set; }

        //public int Version { get; set; }

        public string ContentType { get; set; }

        public string Content { get; set; }

        public byte[] CompiledContent { get; set; }

    }

    public enum Bucket { Lambda, Protobuf, File };
}
