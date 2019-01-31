using System;
using System.Collections.Generic;
using System.IO;
using Eir.Common.Logging;
using Gjallarhorn.Common;
using Gjallarhorn.Notifiers;
using Eir.Common.Common;

namespace Gjallarhorn.Monitors
{
    public class DiskDrivesMonitor : BaseMonitor, IGjallarhornMonitor
    {


        public DiskDrivesMonitor(Func<string, IEnumerable<INotifyerDaemon>> notifyerDaemons): base(notifyerDaemons, "DiskDrivesMonitor")
        {

        }

        public void Execute()
        {
            try
            {
                var msgs = CheckDrives();
                Notify($"{MonitorName} has found the following issues", msgs);
            }
            catch (Exception e)
            {
                Log.To.Main.AddException($"Failed {MonitorName} execute", e);
            }
        }

        private List<string> CheckDrives()
        {
            var ret = new List<string>();
            var dis = DriveInfo.GetDrives();
            var ruleValidator = new RuleValidator();
            var validationExpr = Settings.GetSetting($"{MonitorName}.ValidationExpression", "");
            var driveList = Settings.GetSetting($"{MonitorName}.DriveList", "").ToLower();

            if (string.IsNullOrWhiteSpace(validationExpr)) return ret;

            foreach (var di in dis)
            {
                if (!di.IsReady)
                {
                    continue;
                }

                double freeSpacePercent =
                    (di.TotalFreeSpace > 0 ? (double)di.TotalFreeSpace / di.TotalSize  : 0F);
                if (string.IsNullOrEmpty(driveList) || driveList.Contains(di.Name))
                {
                    if (ruleValidator.ValidateValue(freeSpacePercent, validationExpr))
                    {
                        ret.Add($"The drive {di} has {(int)(freeSpacePercent*100)} percent free disk space.");
                    }
                }
            }
            return ret;
        }
    }
}
