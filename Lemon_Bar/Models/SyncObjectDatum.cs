using System;
using System.Collections.Generic;

#nullable disable

namespace Lemon_Bar.Models
{
    public partial class SyncObjectDatum
    {
        public Guid ObjectId { get; set; }
        public int DataType { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? DroppedTime { get; set; }
        public byte[] LastModified { get; set; }
        public byte[] ObjectData { get; set; }
    }
}
