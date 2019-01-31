using System;
using System.Collections.Generic;
using System.Linq;
using QMS_API.Enums;
using QMS_API.QMSBackend;

namespace QMS_API
{
    public class CustomerAggregatorQmsTaskStatus : QmsTaskStatus
    {
        public string Sysname { get; set; }
    }

    /// <summary>
    /// Used by the Sync.Service.
    /// </summary>
    public class SyncServiceQmsApiService : QmsApiService
    {
        private const string AGGREGATOR_TASK_NAME_PREFIX = "Incremental load: ";

        public SyncServiceQmsApiService(string address)
            : base(address)
        {
        }

        public void CreateAggregateTask(string sysname, out Guid taskId)
        {
            ServiceInfo qds = Client.GetServices(ServiceTypes.QlikViewDistributionService).FirstOrDefault();
            if (qds == null)
            {
                throw new System.Exception("No QDS found.");
            }

            List<DocumentNode> sourceDocuments = Client.GetSourceDocuments(qds.ID);

            DocumentNode templateDocument = sourceDocuments.FirstOrDefault(t =>
                t.Name.Equals("Aggregator.qvw", StringComparison.InvariantCultureIgnoreCase) &&
                t.RelativePath.Equals("_TEMPLATE", StringComparison.InvariantCultureIgnoreCase));

            if (templateDocument == null)
            {
                throw new System.Exception("Template \"Aggregator.qvw\" not found.");
            }

            DocumentNode customerDocument = sourceDocuments.FirstOrDefault(t =>
                t.Name.Equals("Aggregator.qvw", StringComparison.InvariantCultureIgnoreCase) &&
                t.RelativePath.Equals(sysname, StringComparison.InvariantCultureIgnoreCase));

            if (customerDocument == null)
            {
                throw new System.Exception("Customer \"Aggregator.qvw\" not found.");
            }

            TaskInfo templateTaskInfo = Client.GetTasksForDocument(templateDocument.ID).FirstOrDefault(x => x.Type == TaskType.DocumentTask);
            if (templateTaskInfo == null)
            {
                throw new System.Exception("Document task for the template \"Aggregator.qvw\" not found.");
            }

            string taskName = GetCustomerAggregatorTaskName(sysname);
            TaskInfo existingTaskInfo = Client.FindTask(qds.ID, TaskType.DocumentTask, taskName);
            if (existingTaskInfo != null)
            {
                taskId = existingTaskInfo.ID;
                return;
            }

            DocumentTask task = Client.GetDocumentTask(templateTaskInfo.ID, DocumentTaskScope.All);
            task.ID = Guid.NewGuid();
            task.Document = customerDocument;
            task.General.Enabled = true;
            task.General.TaskName = taskName;
            task.DocumentInfo.Category = "Data aggregators";
            task.Reload.ScriptParameterValue = sysname;
            task.Triggering.Triggers.Clear();
            Client.SaveDocumentTask(task);

            task = Client.GetDocumentTask(task.ID, DocumentTaskScope.All);
            var recurrenceTrigger = new RecurrenceTrigger
            {
                Hourly = new RecurrenceTrigger.RecurrenceTriggerHourly
                {
                    DayOfWeekConstraints = new List<DayOfWeek>
                    {
                        DayOfWeek.Monday,
                        DayOfWeek.Tuesday,
                        DayOfWeek.Wednesday,
                        DayOfWeek.Thursday,
                        DayOfWeek.Friday,
                        DayOfWeek.Saturday,
                        DayOfWeek.Sunday
                    },
                    RecurEvery = 20,
                    TimeConstraintFrom = DateTime.MinValue,
                    TimeConstraintTo = DateTime.MaxValue
                },
                Enabled = true,
                Type = TaskTriggerType.HourlyTrigger
            };

            task.Triggering.Triggers.Add(recurrenceTrigger);
            Client.SaveDocumentTask(task);

            taskId = task.ID;
        }

        public bool RemoveAggregateTask(string sysname, out Guid taskId)
        {
            ServiceInfo qds = Client.GetServices(ServiceTypes.QlikViewDistributionService).FirstOrDefault();
            if (qds == null)
            {
                throw new System.Exception("No QDS found.");
            }

            string taskName = GetCustomerAggregatorTaskName(sysname);
            TaskInfo taskInfo = Client.FindTask(qds.ID, TaskType.DocumentTask, taskName);

            if (taskInfo == null)
            {
                taskId = Guid.Empty;
                return false;
            }

            taskId = taskInfo.ID;
            Client.DeleteTask(taskId, TaskType.DocumentTask);
            return true;
        }

        private static string GetCustomerAggregatorTaskName(string sysname)
        {
            return AGGREGATOR_TASK_NAME_PREFIX + sysname;
        }

        private static string GetSysname(string customerAggregatorTaskName)
        {
            return !customerAggregatorTaskName.StartsWith(AGGREGATOR_TASK_NAME_PREFIX)
                ? null
                : customerAggregatorTaskName.Substring(AGGREGATOR_TASK_NAME_PREFIX.Length);
        }

        public IEnumerable<CustomerAggregatorQmsTaskStatus> GetAllCustomerAggregatorTaskStatuses()
        {
            return Client
                .GetTaskStatuses(new TaskStatusFilter(), TaskStatusScope.All)
                .Select(x => new { TaskStatus = x, Sysname = GetSysname(x.General.TaskName) })
                .Where(x => !string.IsNullOrEmpty(x.Sysname))
                .OrderBy(x => x.Sysname)
                .Select(x => new CustomerAggregatorQmsTaskStatus
                {
                    Sysname = x.Sysname,
                    Category = x.TaskStatus.Extended.Category,
                    DocumentPath = x.TaskStatus.Extended.DocumentPath,
                    FinishedTime = x.TaskStatus.Extended.FinishedTime,
                    LastLogMessages = x.TaskStatus.Extended.LastLogMessages,
                    Qdsid = x.TaskStatus.Extended.QDSID,
                    StartTime = x.TaskStatus.Extended.StartTime,
                    Status = (QmsTaskStatusType)x.TaskStatus.General.Status,
                    TaskId = x.TaskStatus.TaskID,
                    TaskName = x.TaskStatus.General.TaskName,
                    TaskSummary = x.TaskStatus.Extended.TaskSummary,
                    TaskType = (QmsTaskType)x.TaskStatus.General.TaskType
                });
        }
    }
}