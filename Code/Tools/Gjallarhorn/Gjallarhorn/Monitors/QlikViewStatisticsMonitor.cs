using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Eir.Common.Common;
using Eir.Common.Logging;
using Gjallarhorn.Common;
using Gjallarhorn.Monitors.QmsApi;
using Gjallarhorn.Notifiers;
using Newtonsoft.Json;
using QMS_API.QMSBackend;
using Exception = System.Exception;

namespace Gjallarhorn.Monitors
{
    class QlikViewStatisticsMonitor : BaseMonitor, IGjallarhornMonitor
    {
        private readonly LicenceHelper _licenceHelper = new LicenceHelper();
        public QlikViewStatisticsMonitor(Func<string, IEnumerable<INotifyerDaemon>> notifyerDaemons) : base(notifyerDaemons, "QlikViewStatisticsMonitor")
        {
        }

        public void Execute()
        {
            try
            {
                var data = new StatisticsDto();
                var cals = new List<QvCalAgentDto>();

                var qmsAddress = Settings.GetSetting($"{MonitorName}.QmsAddress", "(undefined)");
                if (qmsAddress.Equals("(undefined)", StringComparison.InvariantCultureIgnoreCase))
                {
                    qmsAddress = $"http://{(Dns.GetHostEntry(Dns.GetHostName()).HostName).ToLower()}:4799/QMS/Service";
                }

                WmiSystemInfoDto wmiData = new WmiSystemInfoDto();
                var exceptionList = new List<Exception>();
                try
                {
                    wmiData = new WmiSystemInfo().GetValuesFromWin32Os();
                }
                catch (Exception e)
                {
                    data.Exceptions.Add(e);
                }

                using (var qmsApiService = new QMS_API.AgentsQmsApiService(qmsAddress))
                {
                    if (!qmsApiService.TestConnection())
                    {
                        Log.To.Main.Add($"Could not connect to QMS API ({qmsAddress})!");
                        return;
                    }

                    var services = qmsApiService.GetServices(ServiceTypes.QlikViewServer | ServiceTypes.QlikViewDistributionService);
                    var qvServers = services.Where(p => p.Type == ServiceTypes.QlikViewServer).ToList();
                    var installationId = qvServers.OrderBy(p => p.ID).First().ID.ToString();
                    qvServers.ForEach(p =>
                    {

                        if(p.Type == ServiceTypes.QlikViewServer)
                        {
                            var calConfigurations = new[]
                            {
                                qmsApiService.GetCalConfiguration(p.ID, CALConfigurationScope.DocumentCALs),
                                qmsApiService.GetCalConfiguration(p.ID, CALConfigurationScope.NamedCALs),
                                qmsApiService.GetCalConfiguration(p.ID, CALConfigurationScope.SessionCALs),
                                qmsApiService.GetCalConfiguration(p.ID, CALConfigurationScope.UsageCALs)
                            };
                            cals.Add(_licenceHelper.ComputeCals(p, calConfigurations));
                        }
                        
                        var license = qmsApiService.GetLicense(p.Type == ServiceTypes.QlikViewServer ? LicenseType.QlikViewServer : LicenseType.Publisher, p.ID);
                        data.InstallationId = installationId;
                        data.WmiSystemInfo = wmiData;
                        data.QlikViewLicence = _licenceHelper.AnalyzeLicense(license);                        
                        data.QlikViewCals = cals;
                        data.Exceptions = exceptionList;

                        Notify($"{MonitorName} has analyzed the following system", new List<string> { JsonConvert.SerializeObject(data, Formatting.Indented) },"-1");
                    });
                }
            }
            catch (Exception ex)
            {
                Log.To.Main.AddException($"Failed executing {MonitorName}", ex);
            }
        }
    }

}
