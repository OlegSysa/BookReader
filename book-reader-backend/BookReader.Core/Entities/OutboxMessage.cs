using BookReader.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Core.Entities
{
    public class OutboxMessage
    {
        public long Id { get; set; }
        public OutboxMessageType EventType { get; set; }
        public string Payload { get; set; } = null!;
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? ProcessedAtUtc { get; set; }
        public int RetryCount { get; set; }
        public string? LastError { get; set; }
        public string? CorrelationId { get; set; }
    }
}
