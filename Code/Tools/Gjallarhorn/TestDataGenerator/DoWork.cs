using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eir.Common.IO;
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
            var monitorNameFile = "SenseLogFileParserMonitor";
            var date = DateTime.Now.AddMonths(-12);
            var gjallarhornDb = new GjallarhornDb(FileSystem.Singleton);
            gjallarhornDb.EnsureMonitorTableExists(monitorNameFile);
            Random random = new Random();
            while (date < DateTime.Now)
            {
                date = date.AddHours(1);
               
                if (date.Hour == 0)
                {
                    var dto = new FileMinerDto();
                    dto.LicenseSerialNo = "(fake LICENCE)";
                    dto.CollectionDateUtc = date;
                    dto.IsMonthly = false;
                    dto.TotalUniqueActiveApps = random.Next(100);
                    dto.TotalUniqueActiveUsers = random.Next(55);
                    dto.SessionLengthAvgInMinutes = random.Next(100);
                    dto.SessionLengthMedInMinutes = random.Next(50);

                    var data = new StatisticsDto { LogFileMinerData = dto, CollectionDateUtc = dto.CollectionDateUtc };
                    data.InstallationId = "(fake)_" + Guid.NewGuid();
                    dto.TotalUniqueActiveAppsList = new Dictionary<string, int>();
                    gjallarhornDb.SaveMonitorData(monitorNameFile, JsonConvert.SerializeObject(data));
                }
            }
        }
    }
}
