using System;
using System.Diagnostics;

namespace Bifrost.Model
{
    [DebuggerDisplay("SettingsAction: Hostname={Hostname}, Action={Action}, ActionState={ActionState}")]
    public class SettingsAction
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string Hostname { get; set; }
        public DateTime Timestamp { get; set; }
        public CustomerSettingAction Action { get; set; }
        public CustomerSettingActionState ActionState { get; set; }
        public string CreatedByUser { get; set; }
        public DateTime CreatedTimestamp { get; set; }
        public string ClosedByUser { get; set; }
        public DateTime ClosedTimestamp { get; set; }
    }

    public enum CustomerSettingAction
    {
        Undefined = 0,
        PushSettingsToBackend = 1,
        GetSettingsFromBackend = 2,

        // for future use:
        ResetMonitor = 3
    }

    public enum CustomerSettingActionState
    {
        Undefined,
        Pending,
        Completed,
        CancelledByUser
    }
}