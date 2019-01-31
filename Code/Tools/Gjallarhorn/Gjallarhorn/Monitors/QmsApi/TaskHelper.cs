using System;
using System.Collections.Generic;
using System.Diagnostics;
using SenseApiLibrary;

namespace Gjallarhorn.Monitors.QmsApi
{
    public class TaskHelper
    {
        public List<TaskDto> GetAllTasksFullDto(SenseApiSupport senseApiSupport, SenseEnums senseEnums)
        {
            var dynamicJson = new QmsHelper().GetJArray(senseApiSupport, "qrs/task/full");
            var ret = new List<TaskDto>();
            foreach (dynamic serviceStatusStruct in dynamicJson)
            {
                try
                {
                    var task = new TaskDto
                    {
                        Taskid = serviceStatusStruct.id,
                        Createddate = serviceStatusStruct.createdDate,
                        Modifieddate = serviceStatusStruct.modifiedDate,
                        Modifiedbyusername = serviceStatusStruct.modifiedByUserName,
                        Ismanuallytriggered = serviceStatusStruct.isManuallyTriggered,
                        Name = serviceStatusStruct.name,
                        Tasktype = serviceStatusStruct.taskType,
                        Enabled = serviceStatusStruct.enabled,
                        Tasksessiontimeout = serviceStatusStruct.taskSessionTimeout,
                        Maxretries = serviceStatusStruct.maxRetries,
                        Impactsecurityaccess = serviceStatusStruct.impactSecurityAccess,
                        Schemapath = serviceStatusStruct.schemaPath,

                    };
                    if (serviceStatusStruct.app != null)
                    {
                        task.App = new AppDto
                        {
                            AppId = serviceStatusStruct.app.id,
                            Name = serviceStatusStruct.app.name,
                            Publishtime = serviceStatusStruct.app.publishTime,
                            Published = serviceStatusStruct.app.published,
                            Savedinproductversion = serviceStatusStruct.app.savedInProductVersion,
                            Migrationhash = serviceStatusStruct.app.migrationHash,
                            Availabilitystatus = serviceStatusStruct.app.availabilityStatus
                        };
                        if (serviceStatusStruct.app.stream != null)
                        {
                            task.App.Stream = new StreamDto
                            {
                                Name = serviceStatusStruct.app.stream.name,
                                StreamId = serviceStatusStruct.app.stream.id
                            };
                        }
                    }
                    if (serviceStatusStruct.operational != null)
                    {
                        task.Operational = new TaskOperationalDto
                        {
                            NextExecution = serviceStatusStruct.operational.nextExecution,
                            OperationalId = serviceStatusStruct.operational.id,
                            //ExecutionResult = new TaskLastExecutionResultDto
                        };
                        if (serviceStatusStruct.operational.lastExecutionResult != null)
                        {
                            task.Operational.LastExecutionResult = new TaskLastExecutionResultDto
                            {
                                LastexecutionresultId = serviceStatusStruct.operational.lastExecutionResult.id,
                                Executingnodename = serviceStatusStruct.operational.lastExecutionResult.executingNodeName,
                                Status = serviceStatusStruct.operational.lastExecutionResult.status,
                                StatusName = senseEnums.GetValue("StatusEnum", (int)serviceStatusStruct.operational.lastExecutionResult.status, string.Format(Constants.SENSE_API_MISSING_VALUE, serviceStatusStruct.operational.lastExecutionResult.status)),
                                Starttime = serviceStatusStruct.operational.lastExecutionResult.startTime,
                                Stoptime = serviceStatusStruct.operational.lastExecutionResult.stopTime,
                                Duration = serviceStatusStruct.operational.lastExecutionResult.duration,
                                Filereferenceid = serviceStatusStruct.operational.lastExecutionResult.fileReferenceID,
                                Scriptlogavailable = serviceStatusStruct.operational.lastExecutionResult.scriptLogAvailable
                            };
                            if (serviceStatusStruct.operational.lastExecutionResult.details != null)
                            {
                                task.Operational.LastExecutionResult.LastExecutionResultDetails = new List<TaskLastExecutionResultDetailsDto>();
                                foreach (dynamic details in serviceStatusStruct.operational.lastExecutionResult.details)
                                {
                                    task.Operational.LastExecutionResult.LastExecutionResultDetails.Add(new TaskLastExecutionResultDetailsDto
                                    {
                                        Detailcreateddate = details.detailCreatedDate,
                                        DetailsId = details.id,
                                        Detailstype = details.detailsType,
                                        Message = details.message
                                    });
                                }
                            }
                        }
                    }

                    ret.Add(task);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }


            }
            return ret;
        }
    }
}
