using Eir.Common.Common;
using Eir.Common.Logging;
using Gjallarhorn.Monitors.QmsApi;
using Gjallarhorn.Notifiers;
using Newtonsoft.Json;
using SenseApiLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;

namespace Gjallarhorn.Monitors
{
    public class SenseStatisticsMonitor : BaseMonitor, IGjallarhornMonitor
    {



        public SenseStatisticsMonitor(Func<string, IEnumerable<INotifyerDaemon>> notifyerDaemons) : base(notifyerDaemons, "SenseStatisticsMonitor")
        {
        }

        public void Execute()
        {
            try
            {

                var host = Settings.GetSetting($"{MonitorName}.HostName", "(undefined)");
                if (host.Equals("(undefined)", StringComparison.InvariantCultureIgnoreCase))
                {
                    host = (Dns.GetHostEntry(Dns.GetHostName()).HostName).ToLower();
                }
                var senseApi = SenseApiSupport.Create(host);
                var helper = new SenseApiHelper();

                SenseEnums senseEnums = new SenseEnums(senseApi);
                var data = new StatisticsDto();
                try
                {
                    data.QLikSenseCalInfo = helper.ExecuteCalAgentWithOverview(senseApi, senseEnums);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex);
                    try { data.QLikSenseCalInfo = helper.ExecuteCalAgent(senseApi, senseEnums); } catch (Exception e) { data.Exceptions.Add(e); }
                }

                try { data.QlikSenseLicenseAgent = helper.ExecuteLicenseAgent(senseApi, senseEnums); } catch (Exception e) { data.Exceptions.Add(e); }
                try { data.QlikSenseQrsAbout = helper.GetQrsAbout(senseApi, senseEnums); } catch (Exception e) { data.Exceptions.Add(e); }
                try { data.QlikSenseAboutSystemInfo = helper.GetAboutSystemInfo(senseApi, senseEnums); } catch (Exception e) { data.Exceptions.Add(e); }
                try { data.QlikSenseServiceInfo = helper.GetQlikSenseServiceInfos(senseApi, senseEnums).ToList(); ; } catch (Exception e) { data.Exceptions.Add(e); }
                try { data.QlikSenseAppListShort = helper.GetQrsAppListShort(senseApi, senseEnums).ToList(); } catch (Exception e) { data.Exceptions.Add(e); }
                try { data.QlikSenseMachineInfos = helper.GetQlikSenseMachineInfos(senseApi, senseEnums).ToList(); } catch (Exception e) { data.Exceptions.Add(e); }

                data.InstallationId = $"{data.QlikSenseLicenseAgent?.LicenseSerialNo ?? "(unknown)"}_{data.QlikSenseServiceInfo?.FirstOrDefault()?.ServiceClusterId.ToString() ?? "(unknown)"} ";
                //try
                //{
                //    var wmiData = new WmiSystemInfo().GetValuesFromWin32Os();
                //    data.WmiSystemInfo = wmiData;
                //}
                //catch (Exception e)
                //{
                //    data.Exceptions.Add(e);
                //}

                Notify($"{MonitorName} has analyzed the following system", new List<string> { JsonConvert.SerializeObject(data, Formatting.Indented) }, "-1");

            }
            catch (Exception ex)
            {
                Log.To.Main.AddException($"Failed executing {MonitorName}", ex);
            }
        }

    }
}
