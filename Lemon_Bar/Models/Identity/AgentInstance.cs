using System;
using System.Collections.Generic;

#nullable disable

namespace Lemon_Bar.Models
{
    public partial class AgentInstance
    {
        public Guid Id { get; set; }
        public Guid Agentid { get; set; }
        public DateTime? Lastalivetime { get; set; }
        public string Version { get; set; }

        public virtual Agent Agent { get; set; }
    }
}
