using System;
using System.Collections.Generic;
using Eir.Common.Common;
using Eir.Common.Logging;
using Gjallarhorn.Monitors.QmsApi;
using Gjallarhorn.Notifiers;
using SenseApiLibrary;

namespace Gjallarhorn.Monitors
{
    class SenseNodeStatusMonitor : BaseMonitor, IGjallarhornMonitor
    {
        private SenseApiSupport _senseApiSupport;
        private readonly GroupedNotifyerWorker _groupedNotifyer;

        public SenseNodeStatusMonitor(Func<string, IEnumerable<INotifyerDaemon>> notifyerDaemons): base(notifyerDaemons, "SenseNodeStatusMonitor")
        {
            //_ruleMonitors = new Dictionary<string,AdsRuleMonitor>();
            _groupedNotifyer = new GroupedNotifyerWorker(MonitorName, Notify, null);
        }

        public void Execute()
        {
            try
            {
                var host = Settings.GetSetting("SenseNodeStatusMonitor.HostName", "localhost");
                if ((_senseApiSupport == null) || (_senseApiSupport.Host != host))
                {
                    _senseApiSupport = Settings.GetBool("SenseNodeStatusMonitor.UseHttp", false) ? 
                        SenseApiSupport.CreateHttp(host) : SenseApiSupport.Create(host);
                }

                SenseEnums senseEnums = new SenseEnums(_senseApiSupport);

                var serviceStatuses = GetServiceStatusFull(senseEnums);
                foreach (ServiceStatusFullDto dto in serviceStatuses)
                {
                    var key = dto.ServerNodeHostName + "_" + dto.ServiceTypeId;
                    _groupedNotifyer.AddIfMissing(key, $"Node down {dto.ServiceTypeName} on {dto.ServerNodeHostName}","");
                    _groupedNotifyer.Analyze(key, dto.ServiceStateId);
                }
                _groupedNotifyer.AnalyzeRoundFinished();
            }
            catch (Exception ex)
            {
                _senseApiSupport = null;
                Log.To.Main.AddException($"Failed executing {MonitorName}", ex);
            }
            
        }

        private List<ServiceStatusFullDto> GetServiceStatusFull(SenseEnums senseEnums)
        {
            var dynamicJson = new QmsHelper().GetJArray(_senseApiSupport, "qrs/servicestatus/full");
            var ret = new  List<ServiceStatusFullDto>();
            foreach (dynamic serviceStatusStruct in dynamicJson)
            {

                ret.Add(new ServiceStatusFullDto
                {
                    ServiceTypeId = serviceStatusStruct.serviceType,
                    ServiceStateId = serviceStatusStruct.serviceState,
                    Timestamp = serviceStatusStruct.timestamp,
                    ServerNodeName = serviceStatusStruct.serverNodeConfiguration.name,
                    ServerNodeHostName = serviceStatusStruct.serverNodeConfiguration.hostName,
                    ServiceTypeName = senseEnums.GetValue("ServiceTypeEnum", (int)serviceStatusStruct.serviceType, string.Format(Constants.SENSE_API_MISSING_VALUE, serviceStatusStruct.serviceType)),
                    ServiceStateName = senseEnums.GetValue("ServiceStateEnum", (int)serviceStatusStruct.serviceState, string.Format(Constants.SENSE_API_MISSING_VALUE, serviceStatusStruct.serviceState)),
                 });
            }
            return ret;
        }  
    }
}
