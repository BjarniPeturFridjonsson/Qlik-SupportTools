using System;
using System.Diagnostics;

namespace Bifrost.Model
{
    [DebuggerDisplay("SettingsRequest: Active={Active}, IssuedTimestamp={IssuedTimestamp}")]
    public class SettingsRequest
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime IssuedTimestamp { get; set; }
        public bool Active { get; set; }

        public SettingsRequest()
        {
            Id = Guid.NewGuid();
            CustomerId = Guid.Empty;
            IssuedTimestamp = DateTime.UtcNow;
            Active = true;
        }
    }
}