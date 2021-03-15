using System;
using System.Collections.Generic;

#nullable disable

namespace Lemon_Bar.Models
{
    public partial class Subscription
    {
        public Subscription()
        {
            Agents = new HashSet<Agent>();
            Syncgroups = new HashSet<Syncgroup>();
            Userdatabases = new HashSet<Userdatabase>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime? Creationtime { get; set; }
        public DateTime? Lastlogintime { get; set; }
        public int Tombstoneretentionperiodindays { get; set; }
        public int? Policyid { get; set; }
        public byte Subscriptionstate { get; set; }
        public Guid? WindowsAzureSubscriptionId { get; set; }
        public bool? EnableDetailedProviderTracing { get; set; }
        public string Syncserveruniquename { get; set; }
        public string Version { get; set; }

        public virtual ICollection<Agent> Agents { get; set; }
        public virtual ICollection<Syncgroup> Syncgroups { get; set; }
        public virtual ICollection<Userdatabase> Userdatabases { get; set; }
    }
}
