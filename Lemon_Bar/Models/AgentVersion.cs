using System;
using System.Collections.Generic;

#nullable disable

namespace Lemon_Bar.Models
{
    public partial class AgentVersion
    {
        public int Id { get; set; }
        public string Version { get; set; }
        public DateTime ExpiresOn { get; set; }
        public string Comment { get; set; }
    }
}
