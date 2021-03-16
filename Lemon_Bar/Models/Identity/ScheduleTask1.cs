using System;
using System.Collections.Generic;

#nullable disable

namespace Lemon_Bar.Models
{
    public partial class ScheduleTask1
    {
        public Guid ScheduleTaskId { get; set; }
        public int TaskType { get; set; }
        public string TaskName { get; set; }
        public int? Schedule { get; set; }
        public int State { get; set; }
        public DateTime NextRunTime { get; set; }
        public Guid? MessageId { get; set; }
        public string TaskInput { get; set; }
        public Guid QueueId { get; set; }
        public Guid TracingId { get; set; }
        public Guid JobId { get; set; }

        public virtual Schedule ScheduleNavigation { get; set; }
    }
}
