using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using QMS_API.QMSBackend;

namespace Gjallarhorn.Monitors.QmsApi
{
    public class LicenceHelper
    {
        private const string REGEX_PATTERN_PRODUCTLEVEL = @"PRODUCTLEVEL;\w*;;(19|20)\d\d[- /.](0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])";
        private const string REGEX_PATTERN_TIMELIMIT = @"TIMELIMIT;\w*;;(19|20)\d\d[- /.](0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])";

        public QvLicenceDto AnalyzeLicense(License license)
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

                    DateTime prodlevelEndDate = (string.IsNullOrEmpty(prodlevelMatch.Value) || !DateTime.TryParse(prodlevelMatch.Value.Substring(prodlevelMatch.Value.Length - 10, 10), out _)) ? DateTime.Now.AddYears(100) : DateTime.Parse(prodlevelMatch.Value.Substring(prodlevelMatch.Value.Length - 10, 10));
                    DateTime timelimitEndDate = (string.IsNullOrEmpty(timelimitMatch.Value) || !DateTime.TryParse(timelimitMatch.Value.Substring(timelimitMatch.Value.Length - 10, 10), out _)) ? DateTime.Now.AddYears(100) : DateTime.Parse(timelimitMatch.Value.Substring(timelimitMatch.Value.Length - 10, 10));

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
        public QvCalAgentDto ComputeCals(ServiceInfo qvsService, IEnumerable<CALConfiguration> cals)
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
