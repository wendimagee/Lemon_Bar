using System;
using System.Collections.Generic;

#nullable disable

namespace Lemon_Bar.Models
{
    public partial class Userdatabase
    {
        public Userdatabase()
        {
            Syncgroupmembers = new HashSet<Syncgroupmember>();
            Syncgroups = new HashSet<Syncgroup>();
        }

        public Guid Id { get; set; }
        public string Server { get; set; }
        public string Database { get; set; }
        public int State { get; set; }
        public Guid Subscriptionid { get; set; }
        public Guid Agentid { get; set; }
        public byte[] ConnectionString { get; set; }
        public string DbSchema { get; set; }
        public bool IsOnPremise { get; set; }
        public string SqlazureInfo { get; set; }
        public DateTime? LastSchemaUpdated { get; set; }
        public DateTime? LastTombstonecleanup { get; set; }
        public string Region { get; set; }
        public Guid? JobId { get; set; }

        public virtual Subscription Subscription { get; set; }
        public virtual ICollection<Syncgroupmember> Syncgroupmembers { get; set; }
        public virtual ICollection<Syncgroup> Syncgroups { get; set; }
    }
}
