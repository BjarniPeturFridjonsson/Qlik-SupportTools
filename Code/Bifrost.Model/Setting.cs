using System;
using System.Diagnostics;

namespace Bifrost.Model
{
    [DebuggerDisplay("Setting: Key={Key}, Value={Value}, Host={Hostname}")]
    public class Setting
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string Hostname { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public int VerificationStatus { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public bool Deleted { get; set; }

        public Setting()
        {
            Id = Guid.NewGuid();
            CustomerId = Guid.Empty;
            Hostname = string.Empty;
            Key = string.Empty;
            Value = string.Empty;
            VerificationStatus = 0;
            ModifiedDateTime = DateTime.Now;
            Deleted = false;
        }
    }
}