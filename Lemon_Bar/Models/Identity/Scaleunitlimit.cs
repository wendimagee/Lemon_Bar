using System;
using System.Collections.Generic;

#nullable disable

namespace Lemon_Bar.Models
{
    public partial class Scaleunitlimit
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MaxValue { get; set; }
        public DateTime LastModified { get; set; }
    }
}
