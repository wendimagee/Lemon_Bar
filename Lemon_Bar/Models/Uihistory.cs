using System;
using System.Collections.Generic;

#nullable disable

namespace Lemon_Bar.Models
{
    public partial class Uihistory
    {
        public Guid Id { get; set; }
        public DateTime CompletionTime { get; set; }
        public int TaskType { get; set; }
        public int RecordType { get; set; }
        public Guid Serverid { get; set; }
        public Guid Agentid { get; set; }
        public Guid Databaseid { get; set; }
        public Guid SyncgroupId { get; set; }
        public string DetailEnumId { get; set; }
        public string DetailStringParameters { get; set; }
        public bool? IsWritable { get; set; }
    }
}
