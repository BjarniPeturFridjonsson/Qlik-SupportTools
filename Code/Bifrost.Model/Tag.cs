using System;
using System.Diagnostics;

namespace Bifrost.Model
{
    [DebuggerDisplay("Tag: {Key}={Value}")]
    public class Tag
    {
        public Guid DatapointId { get; set; }
        public Guid CustomerId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }

        public DateTime CollectedTimestamp { get; set; }
        public DateTime ReceivedTimestamp { get; set; }

        public Tag()
        {
            DatapointId = Guid.NewGuid();
            CustomerId = Guid.NewGuid();
            Key = string.Empty;
            Value = string.Empty;
            CollectedTimestamp = DateTime.UtcNow;
            ReceivedTimestamp = DateTime.UtcNow;
        }
    }
}