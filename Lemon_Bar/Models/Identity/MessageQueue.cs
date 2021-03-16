using System;
using System.Collections.Generic;

#nullable disable

namespace Lemon_Bar.Models
{
    public partial class MessageQueue
    {
        public Guid MessageId { get; set; }
        public Guid? JobId { get; set; }
        public int MessageType { get; set; }
        public string MessageData { get; set; }
        public DateTime InitialInsertTimeUtc { get; set; }
        public DateTime InsertTimeUtc { get; set; }
        public DateTime? UpdateTimeUtc { get; set; }
        public byte ExecTimes { get; set; }
        public int ResetTimes { get; set; }
        public long Version { get; set; }
        public Guid? TracingId { get; set; }
        public Guid? QueueId { get; set; }
        public Guid? WorkerId { get; set; }

        public virtual Job Job { get; set; }
    }
}
