using System;
using System.Collections.Generic;

#nullable disable

namespace Lemon_Bar.Models
{
    public partial class Action
    {
        public Action()
        {
            Tasks = new HashSet<Task>();
        }

        public Guid Id { get; set; }
        public Guid? Syncgroupid { get; set; }
        public int Type { get; set; }
        public int State { get; set; }
        public DateTime? Creationtime { get; set; }
        public DateTime? Lastupdatetime { get; set; }

        public virtual ICollection<Task> Tasks { get; set; }
    }
}
