using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
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

        private const string REGEX_PATTERN_PRODUCTLEVEL = @"PRODUCTLEVEL;\w*;;(19|20)\d\d[- /.](0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])";
        private const string REGEX_PATTERN_TIMELIMIT = @"TIMELIMIT;\w*;;(19|20)\d\d[- /.](0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])";

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
                            cals.Add(ComputeCals(p, calConfigurations));
                        }
                        
                        var license = qmsApiService.GetLicense(p.Type == ServiceTypes.QlikViewServer ? LicenseType.QlikViewServer : LicenseType.Publisher, p.ID);
                        data.InstallationId = installationId;
                        data.WmiSystemInfo = wmiData;
                        data.QlikViewLicence = AnalyzeLicense(license);                        
                        data.QlikViewCals = cals;
                        data.Exceptions = exceptionList;

                        Notify($"{MonitorName} has analyzed the following system", new List<string> { JsonConvert.SerializeObject(data, Formatting.Indented) });
                    });
                }
            }
            catch (Exception ex)
            {
                Log.To.Main.AddException($"Failed executing {MonitorName}", ex);
            }
        }

        private QvLicenceDto AnalyzeLicense(License license)
        {
            var ret = new QvLicenceDto
            {
                LicenseSerialNo = "UnknownLefType"
            };

            if (license != null)
            {
                string lef = license.LEFFile;

                Match prodlevelMatch = Regex.Match(lef, REGEX_PATTERN_PRODUCTLEVEL);
                Match timelimitMatch = Regex.Match(lef, REGEX_PATTERN_TIMELIMIT);

                if (lef.Length > 0)
                {
                    if (string.IsNullOrEmpty(prodlevelMatch.Value) && string.IsNullOrEmpty(timelimitMatch.Value))
                    {
                        return ret;
                    }

                    DateTime d;
                    DateTime prodlevelEndDate = (string.IsNullOrEmpty(prodlevelMatch.Value) || !DateTime.TryParse(prodlevelMatch.Value.Substring(prodlevelMatch.Value.Length - 10, 10), out d)) ? DateTime.Now.AddYears(100) : DateTime.Parse(prodlevelMatch.Value.Substring(prodlevelMatch.Value.Length - 10, 10));
                    DateTime timelimitEndDate = (string.IsNullOrEmpty(timelimitMatch.Value) || !DateTime.TryParse(timelimitMatch.Value.Substring(timelimitMatch.Value.Length - 10, 10), out d)) ? DateTime.Now.AddYears(100) : DateTime.Parse(timelimitMatch.Value.Substring(timelimitMatch.Value.Length - 10, 10));

                    //get closest date of the two
                    DateTime firstEndDate = prodlevelEndDate <= timelimitEndDate ? prodlevelEndDate : timelimitEndDate;
                    string serial = license.Serial;
                    return new QvLicenceDto
                    {
                        ExpireDate = firstEndDate,
                        LicenseSerialNo = serial,
                        LicenseType = license.LicenseType.ToString()
                    };
                }
            }
            return ret;
        }

        private QvCalAgentDto ComputeCals(ServiceInfo qvsService, IEnumerable<CALConfiguration> cals)
        {
            int namedCalsAssigned = 0;
            int namedCalsInLicense = 0;
            int documentCalsAssigned = 0;
            int documentCalsInLicense = 0;
            int sessionCalsAssigned = 0;
            int sessionCalsInLicense = 0;
            int usageCalsAssigned = 0;
            int usageCalsInLicense = 0;

            foreach (var cal in cals)
            {
                switch (cal.Scope)
                {
                    case CALConfigurationScope.DocumentCALs:
                        documentCalsAssigned += cal.DocumentCALs.Assigned;
                        documentCalsInLicense += cal.DocumentCALs.InLicense;
                        break;
                    case CALConfigurationScope.NamedCALs:
                        namedCalsAssigned += cal.NamedCALs.Assigned;
                        namedCalsInLicense += cal.NamedCALs.InLicense;
                        break;
                    case CALConfigurationScope.SessionCALs:
                        sessionCalsAssigned += cal.SessionCALs.InLicense - cal.SessionCALs.Available;
                        sessionCalsInLicense += cal.SessionCALs.InLicense;
                        break;
                    case CALConfigurationScope.UsageCALs:
                        usageCalsAssigned += cal.UsageCALs.InLicense - cal.UsageCALs.Available;
                        usageCalsInLicense += cal.UsageCALs.InLicense;
                        break;
                }
            }

            return new QvCalAgentDto
            {
                QvsName = qvsService.Name,
                NamedCalsAssigned = namedCalsAssigned,
                NamedCalsInLicense = namedCalsInLicense,
                NamedCalsUtilizationPercent = GetUtilizationPercent(namedCalsAssigned, namedCalsInLicense),
                DocumentCalsAssigned = documentCalsAssigned,
                DocumentCalsInLicense = documentCalsInLicense,
                DocumentCalsUtilizationPercent = GetUtilizationPercent(documentCalsAssigned, documentCalsInLicense),
                SessionCalsAssigned = sessionCalsAssigned,
                SessionCalsInLicense = sessionCalsInLicense,
                SessionCalsUtilizationPercent = GetUtilizationPercent(sessionCalsAssigned, sessionCalsInLicense),
                UsageCalsAssigned = usageCalsAssigned,
                UsageCalsInLicense = usageCalsInLicense,
                UsageCalsUtilizationPercent = GetUtilizationPercent(usageCalsAssigned, usageCalsInLicense)
            };
        }

        private static double GetUtilizationPercent(int calsAssignedOrAvailable, int calsInLicense)
        {
            double utilizationPercent = ((double)calsAssignedOrAvailable / calsInLicense) * 100;
            if (double.IsNaN(utilizationPercent) || double.IsInfinity(utilizationPercent))
            {
                return 0;
            }
            return utilizationPercent;
        }
    }

}
