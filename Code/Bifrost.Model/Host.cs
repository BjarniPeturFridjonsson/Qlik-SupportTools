using System;
using System.Diagnostics;

namespace Bifrost.Model
{
    [DebuggerDisplay("Host: {Hostname}")]
    public class Host
    {
        public Guid Id { get; set; }

        public string Hostname { get; set; }

        public string Role { get; set; }

        public Host()
        {
            Id = Guid.NewGuid();
            Hostname = string.Empty;
            Role = string.Empty;
        }
    }
}