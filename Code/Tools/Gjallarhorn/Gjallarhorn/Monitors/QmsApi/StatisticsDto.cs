﻿using System;
using System.Collections.Generic;
using FreyrCommon.Models;
using Gjallarhorn.Common;
using Gjallarhorn.SenseLogReading.FileMiners;

namespace Gjallarhorn.Monitors.QmsApi
{
    public class StatisticsDto
    {
        public string InstallationId { get; set; }
        /* General */
        public List<Exception> Exceptions { get; set; } = new List<Exception>();
        public DateTime CollectionDateUtc { get; set; } = DateTime.UtcNow;
        public WmiSystemInfoDto WmiSystemInfo { get; set; }
        /* Sense */
        public IEnumerable<QlikSenseServiceInfo> QlikSenseServiceInfo { get; set; }
        public QlikSenseQrsAbout QlikSenseQrsAbout { get; set; }
        public QlikSenseLicenseInfo QlikSenseLicenseAgent { get; set; }
        public QLikSenseCalInfo QLikSenseCalInfo { get; set; }
        public QlikSenseAboutSystemInfo QlikSenseAboutSystemInfo { get; set; }
        public IEnumerable<QlikSenseAppListShort> QlikSenseAppListShort { get; set; }
        public IEnumerable<QlikSenseMachineInfo> QlikSenseMachineInfos { get; set; }
        /* QlikView */
        public QvLicenceDto QlikViewLicence { get; set; }
        public IEnumerable<QvCalAgentDto> QlikViewCals { get; set; }
        public FileMinerDto LogFileMinerData { get; set; }

    }
}
