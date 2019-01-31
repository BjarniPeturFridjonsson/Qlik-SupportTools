using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Bifrost.Model
{
    [DebuggerDisplay("Datapoint: {Name}, N:{NumericValue}, S:{StringValue}")]
    public class Datapoint
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid ContainerId { get; set; }
        public string Name { get; set; }
        public double NumericValue { get; set; }
        public string StringValue { get; set; }
        public DateTime CollectedTimestamp { get; set; }
        public DateTime ReceivedTimestamp { get; set; }
        public List<Tag> Tags { get; set; }
        public string HostName { get; set; }

        public Datapoint()
        {
            Id = Guid.NewGuid();
            CustomerId = Guid.Empty;
            ContainerId = Guid.Empty;
            Name = string.Empty;
            HostName = string.Empty;
            NumericValue = 0.0f;
            StringValue = string.Empty;
            CollectedTimestamp = DateTime.UtcNow;
            ReceivedTimestamp = DateTime.UtcNow;
            Tags = new List<Tag>();
        }
    }
}
