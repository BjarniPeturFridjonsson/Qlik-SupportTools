using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Eir.Common.Logging;
using FreyrCommon.Extensions;
using FreyrCommon.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SenseApiLibrary;

namespace Gjallarhorn.Monitors.QmsApi
{

    public class SenseApiHelper
    {
        private const string MISSING_VALUE = "<Unknown>";
        private const string LEF_DATE_FORMAT = "yyyy'-'MM'-'dd";
        private const string REGEX_PATTERN_YYYY_MM_DD = @"(19|20)\d\d[- /.](0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])";
        private const string REGEX_PATTERN_PRODUCTLEVEL = @"PRODUCTLEVEL;\w*;;" + REGEX_PATTERN_YYYY_MM_DD;
        private const string REGEX_PATTERN_TIMELIMIT = @"TIMELIMIT;\w*;;" + REGEX_PATTERN_YYYY_MM_DD;



        private Guid _serviceClusterId = Guid.Empty;

        public SenseApiHelper()
        {
        }

        public string GetQlikSenseArchivedFolderLocation(SenseApiSupport senseApiSupport)
        {

            dynamic json = senseApiSupport.RequestWithResponse(
              ApiMethod.Get,
              $"https://{senseApiSupport.Host}:4242/qrs/ServiceCluster/{_serviceClusterId}",
              null,
              null,
              HttpStatusCode.OK,
              JToken.Parse);
            string ret;

            try
            {
                ret = json?.settings?.sharedPersistenceProperties?.archivedLogsRootFolder?.Value ?? "";
            }
            catch (Exception ex) //We dont trust the API to not change case.
            {

                Log.To.Main.Add($"Failed parsing the ServiceCluster and get the archived folder path moving to case insensitive search {ex}");
                var settings = GetNode(json, "settings");
                var sharedPersistenceProperties = GetNode(settings?.Value, "sharedPersistenceProperties");
                ret = GetNode(sharedPersistenceProperties?.Value, "archivedLogsRootFolder")?.Value ?? "";
            }

            if (string.IsNullOrWhiteSpace(ret))
            {
                Log.To.Main.Add($"WARNING-Archived Logs conn string missing.");
                Log.To.Main.Add($"ServiceCluster=>{json?.ToString().Replace(Environment.NewLine, @"\r\n")}");
            }
            return ret;
        }

        private dynamic GetNode(dynamic list, string key)
        {
            if (list == null) return null;

            foreach (dynamic child in list)
            {

                if (string.Equals(child.Name, key, StringComparison.InvariantCultureIgnoreCase))
                    return child;
            }
            return null;
        }

        //    private Tuple<bool,string> FindJobjectStringValue (dynamic child,string key)
        //    {
        //        if (child == null)
        //            return new Tuple<bool, string>(false,"");
        //        if (child is JValue )
        //        {
        //            var obj = (JValue) child;
        //            obj.
        //            return new Tuple<bool, string>(false, "");

        //        }
        //        else if (child is JObject)
        //        {
        //            var obj = (JObject)child;
        //            return new Tuple<bool, string>(false, "");
        //            foreach (var property in obj.Properties())
        //            {
        //                var name = property.Name;
        //                FindJobjectStringValue(property.Value, key);
        //            }
        //        }
        //        else if (child is JArray)
        //        {
        //            var array = (JArray)child;
        //            for (int i = 0; i < array.Count; i++)
        //            {
        //                //var childNode = inTreeNode.Nodes[inTreeNode.Nodes.Add(new TreeNode(i.ToString()))];
        //                //childNode.Tag = array[i];
        //                FindJobjectStringValue(array[i], key);
        //            }
        //        }
        //        else
        //        {

        //        }
        //        return new Tuple<bool, string>(false, "");
        //    }
        //}

        private string GetGenericJsonToString(SenseApiSupport senseApiSupport, string url)
        {
            var json = senseApiSupport.RequestWithResponse(
                ApiMethod.Get,
                url,
                null,
                null,
                HttpStatusCode.OK,
                p => p);

            if (string.IsNullOrWhiteSpace(json)) return "";
            //we are only doing this so the end user can understand the text in the json files.
            try
            {
                dynamic parsedJson = JsonConvert.DeserializeObject(json);
                return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
            }
            catch (Exception e)
            {
                Log.To.Main.AddException($"Failed prettifying the json string from {url}", e);
                return json;
            }
        }

        public string GetQrsDataConnections(SenseApiSupport senseApiSupport, SenseEnums senseEnums)
        {
            return GetGenericJsonToString(senseApiSupport, $"https://{senseApiSupport.Host}:4242/qrs/dataconnection/full");
        }

        public string GetQrsServiceCluster(SenseApiSupport senseApiSupport, SenseEnums senseEnums)
        {
            return GetGenericJsonToString(senseApiSupport, $"https://{senseApiSupport.Host}:4242/qrs/ServiceCluster/{_serviceClusterId}");
        }

        public string GetQrsProxyService(SenseApiSupport senseApiSupport, SenseEnums senseEnums)
        {
            return GetGenericJsonToString(senseApiSupport, $"https://{senseApiSupport.Host}:4242/qrs/ProxyService/full");
        }

        public string GetQrsAppList(SenseApiSupport senseApiSupport, SenseEnums senseEnums)
        {
            return GetGenericJsonToString(senseApiSupport, $"https://{senseApiSupport.Host}:4242/qrs/app/full");
        }

        public string GetQrsDataconnections(SenseApiSupport senseApiSupport, SenseEnums senseEnums)
        {
            return GetGenericJsonToString(senseApiSupport, $"https://{senseApiSupport.Host}:4242/qrs/dataconnection/full");
        }

        public QlikSenseQrsAbout GetQrsAbout(SenseApiSupport senseApiSupport, SenseEnums senseEnums)
        {
            dynamic serverNodeConfigJson = senseApiSupport.RequestWithResponse(
                ApiMethod.Get,
                $"https://{senseApiSupport.Host}:4242/qrs/about",
                null,
                null,
                HttpStatusCode.OK,
                JToken.Parse);

            //foreach (dynamic serverNodeStruct in serverNodeConfigJson)
            //{
            //var a = serverNodeConfigJson.nodeType;
            var qlikSenseQrsAbout = new QlikSenseQrsAbout
            {
                BuildVersion = GetString(serverNodeConfigJson, "buildVersion"),
                BuildDate = GetString(serverNodeConfigJson, "buildDate"),
                DatabaseProvider = GetString(serverNodeConfigJson, "databaseProvider"),
                NodeType = GetInt(serverNodeConfigJson, "nodeType")
            };

            return qlikSenseQrsAbout;
        }

        public QlikSenseAboutSystemInfo GetAboutSystemInfo(SenseApiSupport senseApiSupport, SenseEnums senseEnums)
        {
            dynamic senseSystemInfoJson = senseApiSupport.RequestWithResponse(
                ApiMethod.Get,
                $"https://{senseApiSupport.Host}:9032/v1/systeminfo",
                null,
                null,
                HttpStatusCode.OK,
                JToken.Parse);

            var senseSystemInfo = new QlikSenseAboutSystemInfo
            {
                SenseId = GetString(senseSystemInfoJson, "senseId"),
                Version = GetString(senseSystemInfoJson, "version"),
                DeploymentType = GetString(senseSystemInfoJson, "deploymentType"),
                ReleaseLabel = GetString(senseSystemInfoJson, "releaseLabel"),
                ProductName = GetString(senseSystemInfoJson, "productName"),
                CopyrightYearRange = GetString(senseSystemInfoJson, "copyrightYearRange"),
            };

            return senseSystemInfo;
        }

        public IEnumerable<QlikSenseComponent> GetAboutComponents(SenseApiSupport senseApiSupport, SenseEnums senseEnums)
        {
            dynamic senseComponentsJson = senseApiSupport.RequestWithResponse(
                ApiMethod.Get,
                $"https://{senseApiSupport.Host}:9032/v1/components",
                null,
                null,
                HttpStatusCode.OK,
                JToken.Parse);

            foreach (dynamic component in senseComponentsJson)
            {
                var senseComponent = new QlikSenseComponent
                {
                    Component = GetString(component, "component"),
                    Version = GetString(component, "version")
                };

                yield return senseComponent;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="senseApiSupport"></param>
        /// <param name="senseEnums"></param>
        /// <returns></returns>
        public IEnumerable<QlikSenseMachineInfo> GetQlikSenseMachineInfos(SenseApiSupport senseApiSupport, SenseEnums senseEnums)
        {
            dynamic serverNodeConfigJson = senseApiSupport.RequestWithResponse(
                ApiMethod.Get,
                $"https://{senseApiSupport.Host}:4242/qrs/servernodeconfiguration/full",
                null,
                null,
                HttpStatusCode.OK,
                JToken.Parse);

            foreach (dynamic serverNodeStruct in serverNodeConfigJson)
            {

                var nodePurposeId = GetInt(serverNodeStruct, "nodePurpose");

                var qlikSenseMachineInfo = new QlikSenseMachineInfo
                {
                    HostName = GetString(serverNodeStruct, "hostName"),
                    ServiceClusterId = GetGuid(serverNodeStruct, new[] { "serviceCluster", "id" }),
                    Name = GetString(serverNodeStruct, "name"),
                    IsCentral = GetBool(serverNodeStruct, "isCentral"),
                    NodePurpose = senseEnums.GetValue("NodePurposeEnum", nodePurposeId, MISSING_VALUE),
                    ModifiedDate = GetDate(serverNodeStruct, "modifiedDate")
                };
                _serviceClusterId = qlikSenseMachineInfo.ServiceClusterId;
                yield return qlikSenseMachineInfo;
            }
        }
        private Guid GetGuid(JToken funky, string[] names)
        {
            try
            {
                JToken test = funky;
                foreach (string item in names)
                {
                    test = test[item];
                }
                string ret = test.Value<string>();
                return Guid.Parse(ret);
            }
            catch (Exception e)
            {
                Log.To.Main.AddException($"Failed Getting Guid {names?.FirstOrDefault()}", e);
                return Guid.Empty;
            }

        }

        //private Guid GetGuid(JToken funky, string name)
        //{
        //    try
        //    {


        //        JToken test = funky[name];
        //        string ret = test.Value<string>();
        //        return Guid.Parse(ret);
        //    }
        //    catch (Exception e)
        //    {
        //        Log.Add($"Failed Getting Guid {name}", e);
        //        return Guid.Empty;
        //    }
        //}
        private int GetInt(JToken funky, string name)
        {
            try
            {
                JToken test = funky[name];

                var d = test.Value<int>();
                return d;
            }
            catch (Exception e)
            {
                Log.To.Main.AddException($"Failed Getting int {name}", e);
                return -1;
            }
        }
        private DateTime GetDate(JToken funky, string name)
        {
            try
            {
                JToken test = funky[name];
                DateTime ret = test.Value<DateTime>();
                return ret;
            }
            catch (Exception e)
            {
                Log.To.Main.AddException($"Failed Getting Date {name}", e);
                return DateTime.MinValue;
            }
        }
        private string GetString(JToken funky, string[] names)
        {
            try
            {
                JToken test = funky;
                foreach (string item in names)
                {
                    test = test[item];
                }

                string ret = test.Value<string>();
                return ret;
            }
            catch (Exception e)
            {
                Log.To.Main.AddException($"Failed Getting Strings {names?.FirstOrDefault()}", e);
                return string.Empty;
            }
        }
        private string GetString(JToken funky, string name)
        {
            try
            {

                JToken test = funky[name];
                string ret = test.Value<string>();
                return ret;
            }
            catch (Exception e)
            {
                Log.To.Main.AddException($"Failed Getting String {name}", e);
                return string.Empty;
            }
        }

        private bool GetBool(JToken funky, string name)
        {
            try
            {
                JToken test = funky[name];
                bool ret = test.Value<bool>();
                return ret;
            }
            catch (Exception e)
            {
                Log.To.Main.AddException($"Failed Getting bool {name}", e);
                return false;
            }
        }



        public IEnumerable<QlikSenseServiceInfo> GetQlikSenseServiceInfos(SenseApiSupport senseApiSupport, SenseEnums senseEnums)
        {
            dynamic serviceStatusJson = senseApiSupport.RequestWithResponse(
                ApiMethod.Get,
                $"https://{senseApiSupport.Host}:4242/qrs/servicestatus/full",
                null,
                null,
                HttpStatusCode.OK,
                JArray.Parse);

            foreach (dynamic serviceStatusStruct in serviceStatusJson)
            {
                int serviceTypeId = GetInt(serviceStatusStruct, "serviceType");
                int serviceStateId = GetInt(serviceStatusStruct, "serviceState");

                var qlikSenseServiceInfo = new QlikSenseServiceInfo
                {
                    ServiceType = senseEnums.GetValue("ServiceTypeEnum", serviceTypeId, MISSING_VALUE),
                    HostName = GetString(serviceStatusStruct, new[] { "serverNodeConfiguration", "hostName" }),
                    ServiceClusterId = GetGuid(serviceStatusStruct, new[] { "serverNodeConfiguration", "serviceCluster", "id" }),
                    ServiceState = senseEnums.GetValue("ServiceStateEnum", serviceStateId, MISSING_VALUE),
                    ModifiedDate = GetDate(serviceStatusStruct, "modifiedDate")
                };

                yield return qlikSenseServiceInfo;
            }
        }

        public List<QlikSenseAppListShort> GetQrsAppListShort(SenseApiSupport senseApiSupport, SenseEnums senseEnums)
        {
            dynamic resp = senseApiSupport.RequestWithResponse(
                ApiMethod.Get,
                $"https://{senseApiSupport.Host}:4242/qrs/app/full",
                null,
                null,
                HttpStatusCode.OK,
                JToken.Parse);
            var ret = new List<QlikSenseAppListShort>();
            foreach (dynamic item in resp)
            {
                //var a = serverNodeConfigJson.nodeType;
                ret.Add(new QlikSenseAppListShort
                {
                    Id = GetString(item, "id"),
                    CreatedDate = GetString(item, "createdDate"),
                    ModifiedDate = GetString(item, "modifiedDate"),
                    PublishTime = GetString(item, "publishTime"),
                    Published = GetString(item, "published"),
                    FileSize = GetString(item, "fileSize"),
                    LastReloadTime = GetString(item, "lastReloadTime")
                });
            }

            return ret;
        }

        public QLikSenseCalInfo ExecuteCalAgent(SenseApiSupport senseApiSupport, SenseEnums senseEnums)
        {
            try
            {

                if (!IsCentralNode(senseApiSupport))
                {
                    // The "/qrs/license/accesstypeinfo"-call can only be executed on the central node.
                    return null;
                }

                dynamic calJson = senseApiSupport.RequestWithResponse(
                    ApiMethod.Get,
                    $"https://{senseApiSupport.Host}:4242/qrs/license/accesstypeinfo",
                    null,
                    null,
                    HttpStatusCode.OK,
                    JToken.Parse);

                Func<JObject, string, double> getDouble = (jObject, propertyName)
                    => jObject?.GetValue(propertyName).ToObject<double>() ?? 0;

                double totalTokens = calJson.totalTokens;
                double availableTokens = calJson.availableTokens;

                JObject userAccessType = calJson.userAccessType;
                double userAllocatedTokens = getDouble(userAccessType, "allocatedTokens");
                double userUsedTokens = getDouble(userAccessType, "usedTokens");
                double userQuarantinedTokens = getDouble(userAccessType, "quarantinedTokens");

                JObject loginAccessType = calJson.loginAccessType;
                double loginAllocatedTokens = getDouble(loginAccessType, "allocatedTokens");
                double loginUsedTokens = getDouble(loginAccessType, "usedTokens");
                double loginUnavailableTokens = getDouble(loginAccessType, "unavailableTokens");

                JObject applicationAccessType = calJson.applicationAccessType;
                double applicationAllocatedTokens = getDouble(applicationAccessType, "allocatedTokens");
                double applicationUsedTokens = getDouble(applicationAccessType, "usedTokens");
                double applicationQuarantinedTokens = getDouble(applicationAccessType, "quarantinedTokens");

                JObject professionalAccessType = calJson.professionalAccessType;
                double professionalAllocatedTokens = getDouble(professionalAccessType, "allocatedTokens");
                double professionalUsedTokens = getDouble(professionalAccessType, "usedTokens");
                double professionalQuarantinedTokens = getDouble(professionalAccessType, "quarantinedTokens");

                JObject analyzerAccessType = calJson.analyzerAccessType;
                double analyzerAllocatedTokens = getDouble(analyzerAccessType, "allocatedTokens");
                double analyzerUsedTokens = getDouble(analyzerAccessType, "usedTokens");
                double analyzerQuarantinedTokens = getDouble(analyzerAccessType, "quarantinedTokens");

                var ret = new QLikSenseCalInfo
                {
                    TotalTokens = totalTokens,
                    AvailableTokens = availableTokens,
                    UserAllocatedTokens = userAllocatedTokens,
                    UserUsedTokens = userUsedTokens,
                    UserQuarantinedTokens = userQuarantinedTokens,
                    LoginAllocatedTokens = loginAllocatedTokens,
                    LoginUsedTokens = loginUsedTokens,
                    LoginUnavailableTokens = loginUnavailableTokens,
                    ApplicationAllocatedTokens = applicationAllocatedTokens,
                    ApplicationUsedTokens = applicationUsedTokens,
                    ApplicationQuarantinedTokens = applicationQuarantinedTokens,
                    ProfessionalAllocatedTokens = professionalAllocatedTokens,
                    ProfessionalUsedTokens = professionalUsedTokens,
                    ProfessionalQuarantinedTokens = professionalQuarantinedTokens,
                    AnalyzerAllocatedTokens = analyzerAllocatedTokens,
                    AnalyzerUsedTokens = analyzerUsedTokens,
                    AnalyzerQuarantinedTokens = analyzerQuarantinedTokens
                };
                return ret;
            }
            catch (WebException exception)
            {
                if ((senseApiSupport != null) &&
                    (exception.Status == WebExceptionStatus.ProtocolError) &&
                    (exception.Response as HttpWebResponse)?.StatusCode == HttpStatusCode.Forbidden)
                {
                    // The certificate seems to have gone bad...
                    throw new Exception("Failed getting Cal info. Probably a certificate issue.");
                    // Exit silently and retry the next time.

                }

                throw;
            }
        }

        public QlikSenseLicenseInfo ExecuteLicenseAgent(SenseApiSupport senseApiSupport, SenseEnums senseEnums)
        {
            try
            {
                dynamic licenseJson = senseApiSupport.RequestWithResponse(
                    ApiMethod.Get,
                    $"https://{senseApiSupport.Host}:4242/qrs/license",
                    null,
                    null,
                    HttpStatusCode.OK,
                    JToken.Parse);

                string lef = licenseJson.lef;
                string serial = licenseJson.serial;
                bool isExpired = licenseJson.isExpired;
                string expiredReason = licenseJson.expiredReason;
                bool isBlacklisted = licenseJson.isBlacklisted;

                if (lef.Length > 0)
                {
                    Match prodlevelMatch = Regex.Match(lef, REGEX_PATTERN_PRODUCTLEVEL);
                    Match timelimitMatch = Regex.Match(lef, REGEX_PATTERN_TIMELIMIT);

                    if (!prodlevelMatch.Success && !timelimitMatch.Success)
                    {
                        return null;
                    }

                    int lefDateFormatLength = DateTime.UtcNow.ToString(LEF_DATE_FORMAT).Length;

                    Func<Match, DateTime> parseLefDateTimeString = match =>
                    {
                        if (!match.Success)
                        {
                            return DateTime.MaxValue;
                        }

                        DateTime dateTime;

                        string dateTimeString = match.Value.Substring(match.Value.Length - lefDateFormatLength, lefDateFormatLength);

                        if (!DateTime.TryParseExact(
                            dateTimeString,
                            LEF_DATE_FORMAT,
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.AssumeLocal,
                            out dateTime))
                        {
                            dateTime = DateTime.MaxValue;
                        }

                        return dateTime.Date;
                    };

                    DateTime prodlevelEndDate = parseLefDateTimeString(prodlevelMatch);
                    DateTime timelimitEndDate = parseLefDateTimeString(timelimitMatch);

                    // Get the closest date of the two
                    DateTime firstEndDate = prodlevelEndDate <= timelimitEndDate
                        ? prodlevelEndDate
                        : timelimitEndDate;

                    return new QlikSenseLicenseInfo
                    {
                        ExpireDate = firstEndDate.AsDatapointString(),
                        LicenseSerialNo = serial,
                        IsExpired = isExpired ? 1 : 0,
                        ExpiredReason = expiredReason,
                        IsBlacklisted = isBlacklisted ? 1 : 0
                    };
                }
            }
            catch (WebException exception)
            {
                if ((senseApiSupport != null) &&
                    (exception.Status == WebExceptionStatus.ProtocolError) &&
                    (exception.Response as HttpWebResponse)?.StatusCode == HttpStatusCode.Forbidden)
                {
                    // The certificate seems to have gone bad...
                    throw new Exception("Failed getting License info. Probably a sertificate issue.");
                }

                throw;
            }
            return null;
        }

        private bool IsCentralNode(SenseApiSupport senseApiSupport)
        {
            dynamic json = senseApiSupport.RequestWithResponse(
                ApiMethod.Get,
                $"https://{senseApiSupport.Host}:4242/qrs/servernodeconfiguration/local",
                null,
                null,
                HttpStatusCode.OK,
                JToken.Parse);

            return json.isCentral;
        }

    }
}
