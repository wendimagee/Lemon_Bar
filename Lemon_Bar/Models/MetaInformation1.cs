using System;
using System.Collections.Generic;

#nullable disable

namespace Lemon_Bar.Models
{
    public partial class MetaInformation1
    {
        public int Id { get; set; }
        public int VersionMajor { get; set; }
        public int VersionMinor { get; set; }
        public int VersionBuild { get; set; }
        public int VersionService { get; set; }
        public string VersionString { get; set; }
        public long? Version { get; set; }
        public bool? State { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
