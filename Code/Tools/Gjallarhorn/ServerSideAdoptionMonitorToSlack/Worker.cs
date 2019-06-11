using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServerSideAdoptionMonitorToSlack
{
    /// <summary>
    /// This is a brutaly simple monitoring of customers sending data so we see what customers are not sending
    ///
    /// It is running on xxx-service04
    /// </summary>
    class Worker
    {
        readonly DynaSql _db = new DynaSql("our connection string.......");
        public async Task DoWork()
        {
            var weekDate = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            var dayDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            var weekList = GetWeeklistFromDb(weekDate);
            var senseCalInfo = GetDaily("qliksensecalinfo", dayDate);
            var fileMiner = GetDaily("qliksensefileminer", dayDate);
            var qlikCals = GetDaily("qlikviewcals", dayDate);

            var outputData = "";

            foreach (var installationId in weekList)
            {
                if(string.IsNullOrWhiteSpace(installationId)) continue;
                outputData += $"{installationId.PadRight(54, ' ')} : Sense {GetValue(senseCalInfo, installationId, "-").PadRight(3, ' ')}  : QV {GetValue(qlikCals, installationId, "-").PadRight(3, ' ')} : Files {GetValue(fileMiner, installationId, "-").PadRight(3, ' ')} {Environment.NewLine}" ;
            }


            var msg = $@"*Current senders of data*
These are the customers we have had contact with the last week with numbers from the last 24 hours.
```{outputData}```
";

            var slack = new SlackNotifyer();
            await slack.PostMessageToSlackAsync(msg);
        }

        private Dictionary<string, string> GetDaily(string tableName, string date)
        {
            var ret = _db.SqlDictionary($"select installationid,count(*) FROM `sensestatistics.{tableName}` sense where timestamp > '{date}' group by installationId;");
            return ret;
        }


        private List<string> GetWeeklistFromDb(string date)
        {


            var ret = _db.SqlList($@"
select distinct installationid FROM `sensestatistics.qliksensecalinfo` sense where timestamp > '{date}'
union 
select distinct installationid FROM `sensestatistics.qliksensefileminer` sense where timestamp > '{date}'
union 
select distinct installationid FROM `sensestatistics.qlikviewcals` sense where timestamp > '{date}'

");

            return ret;
        }
        private static TV GetValue<TK, TV>(IDictionary<TK, TV> dict, TK key, TV defaultValue = default(TV))
        {
            return dict.TryGetValue(key, out var value) ? value : defaultValue;
        }
    }
}
