using System;
using System.Collections.Generic;

#nullable disable

namespace Lemon_Bar.Models
{
    public partial class ScheduleTask
    {
        public Guid Id { get; set; }
        public Guid SyncGroupId { get; set; }
        public long Interval { get; set; }
        public DateTime LastUpdate { get; set; }
        public byte State { get; set; }
        public DateTime? ExpirationTime { get; set; }
        public Guid? PopReceipt { get; set; }
        public byte DequeueCount { get; set; }
        public int Type { get; set; }

        public virtual Syncgroup SyncGroup { get; set; }
    }
}
