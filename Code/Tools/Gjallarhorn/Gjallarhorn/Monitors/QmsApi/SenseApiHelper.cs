using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using Eir.Common.Common;
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
        private JsonDynamicHelper _jsonHelper;

        public SenseApiHelper()
        {
            _jsonHelper = new JsonDynamicHelper();
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
                BuildVersion = _jsonHelper.GetString(serverNodeConfigJson, "buildVersion"),
                BuildDate = _jsonHelper.GetString(serverNodeConfigJson, "buildDate"),
                DatabaseProvider = _jsonHelper.GetString(serverNodeConfigJson, "databaseProvider"),
                NodeType = _jsonHelper.GetInt(serverNodeConfigJson, "nodeType")
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
                SenseId = _jsonHelper.GetString(senseSystemInfoJson, "senseId"),
                Version = _jsonHelper.GetString(senseSystemInfoJson, "version"),
                DeploymentType = _jsonHelper.GetString(senseSystemInfoJson, "deploymentType"),
                ReleaseLabel = _jsonHelper.GetString(senseSystemInfoJson, "releaseLabel"),
                ProductName = _jsonHelper.GetString(senseSystemInfoJson, "productName"),
                CopyrightYearRange = _jsonHelper.GetString(senseSystemInfoJson, "copyrightYearRange"),
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
                    Component = _jsonHelper.GetString(component, "component"),
                    Version = _jsonHelper.GetString(component, "version")
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

                var nodePurposeId = _jsonHelper.GetInt(serverNodeStruct, "nodePurpose");

                var qlikSenseMachineInfo = new QlikSenseMachineInfo
                {
                    HostName = _jsonHelper.GetString(serverNodeStruct, "hostName"),
                    ServiceClusterId = _jsonHelper.GetGuid(serverNodeStruct, new[] { "serviceCluster", "id" }),
                    Name = _jsonHelper.GetString(serverNodeStruct, "name"),
                    IsCentral = _jsonHelper.GetBool(serverNodeStruct, "isCentral"),
                    NodePurpose = senseEnums.GetValue("NodePurposeEnum", nodePurposeId, MISSING_VALUE),
                    ModifiedDate = _jsonHelper.GetDate(serverNodeStruct, "modifiedDate")
                };
                _serviceClusterId = qlikSenseMachineInfo.ServiceClusterId;
                yield return qlikSenseMachineInfo;
            }
        }
     

        public Dictionary<string, QlikSenseAppObjectsShort> GetQlikSenseAppObjectInfos(SenseApiSupport senseApiSupport, SenseEnums senseEnums)
        {
            dynamic json = senseApiSupport.RequestWithResponse(
                ApiMethod.Get,
                $"https://{senseApiSupport.Host}:4242/qrs/app/object/full",
                null,
                null,
                HttpStatusCode.OK,
                JArray.Parse);

            var analyzer = new Dictionary<string, QlikSenseAppObjectsShort>();
            foreach (dynamic data in json)
            {
                var appid = _jsonHelper.GetString(data, new []{ "app", "id" });
                var type = _jsonHelper.GetString(data, "objectType");
               
                if (!analyzer.TryGetValue(appid, out QlikSenseAppObjectsShort value))
                {
                    value = new QlikSenseAppObjectsShort {AppId = appid};
                    analyzer.Add(appid, value);
                }

                if(string.Equals(type, "sheet", StringComparison.InvariantCultureIgnoreCase))
                {
                    value.Sheets++;
                }
                else
                {
                    value.Objects++;
                }
                    
            }

            return analyzer;
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
                int serviceTypeId = _jsonHelper.GetInt(serviceStatusStruct, "serviceType");
                int serviceStateId = _jsonHelper.GetInt(serviceStatusStruct, "serviceState");

                var qlikSenseServiceInfo = new QlikSenseServiceInfo
                {
                    ServiceType = senseEnums.GetValue("ServiceTypeEnum", serviceTypeId, MISSING_VALUE),
                    HostName = _jsonHelper.GetString(serviceStatusStruct, new[] { "serverNodeConfiguration", "hostName" }),
                    ServiceClusterId = _jsonHelper.GetGuid(serviceStatusStruct, new[] { "serverNodeConfiguration", "serviceCluster", "id" }),
                    ServiceState = senseEnums.GetValue("ServiceStateEnum", serviceStateId, MISSING_VALUE),
                    ModifiedDate = _jsonHelper.GetDate(serviceStatusStruct, "modifiedDate")
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
            var appObjs = new Dictionary<string, QlikSenseAppObjectsShort>();
            try
            {
                appObjs = GetQlikSenseAppObjectInfos(senseApiSupport, senseEnums);
            }
            catch (Exception e)
            {
               Log.To.Main.AddException("Failed accessing the app objects in GetQrsAppListShort",e);
               appObjs = new Dictionary<string, QlikSenseAppObjectsShort>();
            }
            
            foreach (dynamic item in resp)
            {
                //var a = serverNodeConfigJson.nodeType;
                var id = _jsonHelper.GetString(item, "id");
                appObjs.TryGetValue(id, out QlikSenseAppObjectsShort appObj);
                ret.Add(new QlikSenseAppListShort
                {
                    Id = id,
                    AppObjects = appObj?.Objects ?? 0,
                    SheetObjects = appObj?.Sheets ?? 0,
                    CreatedDate = _jsonHelper.GetString(item, "createdDate"),
                    ModifiedDate = _jsonHelper.GetString(item, "modifiedDate"),
                    PublishTime = _jsonHelper.GetString(item, "publishTime"),
                    Published = _jsonHelper.GetString(item, "published"),
                    FileSize = _jsonHelper.GetString(item, "fileSize"),
                    LastReloadTime = _jsonHelper.GetString(item, "lastReloadTime")
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

        public QLikSenseCalInfo ExecuteCalAgentWithOverview(SenseApiSupport senseApiSupport, SenseEnums senseEnums)
        {
            try
            {

               

                dynamic calJson = senseApiSupport.RequestWithResponse(
                    ApiMethod.Get,
                    $"https://{senseApiSupport.Host}:4242/qrs/license/accesstypeoverview",
                    null,
                    null,
                    HttpStatusCode.OK,
                    JToken.Parse);

                Func<JObject, string, double> getDouble = (jObject, propertyName)
                    => jObject?.GetValue(propertyName).ToObject<double>() ?? 0;
                Func<JObject, string, bool> getBool = (jObject, propertyName)
                    => jObject?.GetValue(propertyName).ToObject<bool>() ?? false;


                double totalTokens = calJson.totalTokens;
                double availableTokens = calJson.availableTokens;

                JObject userAccessType = calJson.userAccess;
                double userAllocatedTokens = getDouble(userAccessType, "allocatedTokens");
                double userUsedTokens = getDouble(userAccessType, "usedTokens");
                double userQuarantinedTokens = getDouble(userAccessType, "quarantinedTokens");

                JObject loginAccessType = calJson.loginAccess;
                double loginAllocatedTokens = getDouble(loginAccessType, "allocatedTokens");
                double loginUsedTokens = getDouble(loginAccessType, "usedTokens");
                double loginUnavailableTokens = getDouble(loginAccessType, "unavailableTokens");

                //JObject applicationAccessType = calJson.applicationAccess;
                //double applicationAllocatedTokens = getDouble(applicationAccessType, "allocatedTokens");
                //double applicationUsedTokens = getDouble(applicationAccessType, "usedTokens");
                //double applicationQuarantinedTokens = getDouble(applicationAccessType, "quarantinedTokens");

                JObject professionalAccessType = calJson.professionalAccess;
                double professionalAllocatedTokens = getDouble(professionalAccessType, "allocated");
                double professionalUsedTokens = getDouble(professionalAccessType, "used");
                double professionalQuarantinedTokens = getDouble(professionalAccessType, "quarantined");
                double professionalTotalTokens = getDouble(professionalAccessType, "total");

                JObject analyzerAccessType = calJson.analyzerAccess;
                double analyzerAllocatedTokens = getDouble(analyzerAccessType, "allocated");
                double analyzerUsedTokens = getDouble(analyzerAccessType, "used");
                double analyzerQuarantinedTokens = getDouble(analyzerAccessType, "quarantined");
                double analyzerTotalTokens = getDouble(analyzerAccessType, "total");

                JObject analyzerTimeAccessType = calJson.analyzerTimeAccess;
                var timeAccessEnabled = getBool(analyzerTimeAccessType, "enabled");
              
                double analyzerTimeAllocatedMinutes = timeAccessEnabled ? getDouble(analyzerTimeAccessType, "allocatedMinutes") : 0;
                double analyzerTimeUsedMinutes = timeAccessEnabled ? getDouble(analyzerTimeAccessType, "usedMinutes") : 0;
                double analyzerTimeUnavailableMinutes = timeAccessEnabled ? getDouble(analyzerTimeAccessType, "unavailableMinutes") : 0;


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
                    AnalyzerTimeAllocatedMinutes = analyzerTimeAllocatedMinutes,
                    AnalyzerTimeUsedMinutes = analyzerTimeUsedMinutes,
                    AnalyzerTimeUnavailableMinutes = analyzerTimeUnavailableMinutes,
                    ProfessionalAllocatedTokens = professionalAllocatedTokens,
                    ProfessionalTotalTokens = professionalTotalTokens,
                    ProfessionalUsedTokens = professionalUsedTokens,
                    ProfessionalQuarantinedTokens = professionalQuarantinedTokens,
                    AnalyzerAllocatedTokens = analyzerAllocatedTokens,
                    AnalyzerUsedTokens = analyzerUsedTokens,
                    AnalyzerQuarantinedTokens = analyzerQuarantinedTokens,
                    AnalyzerTotalTokens = analyzerTotalTokens,
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
