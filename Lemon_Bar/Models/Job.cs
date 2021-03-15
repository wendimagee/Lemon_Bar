using System;
using System.Collections.Generic;

#nullable disable

namespace Lemon_Bar.Models
{
    public partial class Job
    {
        public Job()
        {
            MessageQueues = new HashSet<MessageQueue>();
        }

        public Guid JobId { get; set; }
        public bool IsCancelled { get; set; }
        public DateTime InitialInsertTimeUtc { get; set; }
        public int JobType { get; set; }
        public string InputData { get; set; }
        public int TaskCount { get; set; }
        public int CompletedTaskCount { get; set; }
        public Guid? TracingId { get; set; }

        public virtual ICollection<MessageQueue> MessageQueues { get; set; }
    }
}
