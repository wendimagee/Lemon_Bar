using System;
using System.Collections.Generic;

#nullable disable

namespace Lemon_Bar.Models
{
    public partial class Syncgroup
    {
        public Syncgroup()
        {
            Syncgroupmembers = new HashSet<Syncgroupmember>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? Subscriptionid { get; set; }
        public string SchemaDescription { get; set; }
        public int? State { get; set; }
        public Guid? HubMemberid { get; set; }
        public int ConflictResolutionPolicy { get; set; }
        public int SyncInterval { get; set; }
        public bool? SyncEnabled { get; set; }
        public DateTime? Lastupdatetime { get; set; }
        public string Ocsschemadefinition { get; set; }
        public bool? Hubhasdata { get; set; }
        public bool ConflictLoggingEnabled { get; set; }
        public int ConflictTableRetentionInDays { get; set; }

        public virtual Userdatabase HubMember { get; set; }
        public virtual Subscription Subscription { get; set; }
        public virtual ScheduleTask ScheduleTask { get; set; }
        public virtual ICollection<Syncgroupmember> Syncgroupmembers { get; set; }
    }
}
