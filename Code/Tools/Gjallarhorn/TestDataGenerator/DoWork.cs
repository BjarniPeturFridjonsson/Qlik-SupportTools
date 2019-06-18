using System;
using System.Collections.Generic;
using Eir.Common.IO;
using FreyrCommon.Models;
using Gjallarhorn.Db;
using Gjallarhorn.Monitors.QmsApi;
using Gjallarhorn.SenseLogReading.FileMiners;
using Newtonsoft.Json;

namespace TestDataGenerator
{
    public class DoWork
    {
        public void GetItDone()
        {
            var txtData = new TextData();

            var installationId = "f113051f-ef10-4969-a2b1-e3c890506b4e";
            var monitorNameFile = "SenseLogFileParserMonitor";
            var monitorNameStats = "SenseStatisticsMonitor";
            var date = DateTime.Now.AddMonths(-12);
            var gjallarhornDb = new GjallarhornDb(FileSystem.Singleton);
            gjallarhornDb.EnsureMonitorTableExists(monitorNameFile);
            gjallarhornDb.EnsureMonitorTableExists(monitorNameStats);
            Random random = new Random();
            

            while (date < DateTime.Now)
            {
                date = date.AddHours(1);

                var dto = new FileMinerDto();
                dto.LicenseSerialNo = "(fake LICENCE)";
                dto.CollectionDateUtc = date;
                dto.IsMonthly = false;

                var data = new StatisticsDto { LogFileMinerData = dto, CollectionDateUtc = dto.CollectionDateUtc };
                data.QLikSenseCalInfo = JsonConvert.DeserializeObject<QLikSenseCalInfo>(txtData.GetCalInfo());
                data.InstallationId = "(fake)_" + installationId;
                dto.TotalUniqueActiveAppsList = new Dictionary<string, int>();
                gjallarhornDb.SaveMonitorData(monitorNameFile, JsonConvert.SerializeObject(data));

                
                if (date.Hour == 0)
                {
                    var dtoDay = new FileMinerDto();
                    dtoDay.LicenseSerialNo = "(fake)";
                    dtoDay.CollectionDateUtc = date;
                    dto.TotalUniqueActiveApps = random.Next(100);
                    dto.TotalUniqueActiveUsers = random.Next(55);
                    dto.SessionLengthAvgInMinutes = random.Next(100);
                    dto.SessionLengthMedInMinutes = random.Next(50);

                    var dataDay = new StatisticsDto { LogFileMinerData = dtoDay, CollectionDateUtc = dtoDay.CollectionDateUtc };

                    dataDay.InstallationId = "(fake)_" + installationId;
                    dtoDay.TotalUniqueActiveAppsList = new Dictionary<string, int>();
                    dataDay.QlikSenseMachineInfos = JsonConvert.DeserializeObject<List<QlikSenseMachineInfo>>(txtData.GetMachineInfo());
                    dataDay.QlikSenseServiceInfo = JsonConvert.DeserializeObject<List<QlikSenseServiceInfo>>(txtData.GetServiceInfo());
                    dataDay.QlikSenseQrsAbout = JsonConvert.DeserializeObject<QlikSenseQrsAbout>(txtData.GetQrsAbout());
                    dataDay.QlikSenseAppListShort = JsonConvert.DeserializeObject<List<QlikSenseAppListShort>>(txtData.GetAppList());

                    gjallarhornDb.SaveMonitorData(monitorNameStats, JsonConvert.SerializeObject(dataDay));
                }
            }
        }
    }
}
