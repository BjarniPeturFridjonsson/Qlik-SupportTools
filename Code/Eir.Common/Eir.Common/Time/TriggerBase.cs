using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eir.Common.Extensions;
using Eir.Common.Logging;

namespace Eir.Common.Time
{
    public abstract class TriggerBase : ITrigger
    {
        private static readonly Task _completedTask = Task.FromResult<object>(null);
        private Func<Task> _funcs = () => _completedTask;
        private readonly object _runningTasksLock = new object();
        private readonly List<Task> _runningTasks = new List<Task>();

        protected TriggerBase()
        {
            Enabled = true;
        }

        public bool Enabled { get; set; }

        public void RegisterAction(Func<Task> action)
        {
            _funcs = (Func<Task>)Delegate.Combine(_funcs, action);
        }

        public void UnregisterAction(Func<Task> action)
        {
            _funcs = (Func<Task>)Delegate.Remove(_funcs, action);
        }

        protected async Task TrigInternal()
        {
            if (Enabled)
            {
                try
                {
                    Task[] tasks = _funcs.GetInvocationList()
                        .Cast<Func<Task>>()
                        .Select(func => func())
                        .ToArray();

                    AddRunningTasks(tasks);

                    await tasks.AwaitAll();
                }
                catch (Exception ex)
                {
                    Log.To.Main.AddException("Error in TrigInternal", ex);
                }
            }
        }

        private void AddRunningTasks(Task[] tasks)
        {
            // remove tasks that are already completed
            tasks = tasks
                .Where(t => !t.IsCompleted)
                .ToArray();

            if (tasks.Length == 0)
            {
                return;
            }

            lock (_runningTasksLock)
            {
                // add the tasks to the list of running tasks
                foreach (var task in tasks)
                {
                    _runningTasks.Add(task);

                    // add a continuation that will remove the task from the list, when completed
                    task.ContinueWith(RemoveRunningTask);
                }
            }
        }

        private void RemoveRunningTask(Task task)
        {
            lock (_runningTasksLock)
            {
                _runningTasks.Remove(task);
            }
        }

        private Task[] GetRunningTasks()
        {
            lock (_runningTasksLock)
            {
                return _runningTasks.ToArray();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            Task[] runningTasks = GetRunningTasks();
            try
            {
                // the call to AwaitAll here is used in order to catch all exceptions
                // from any failing tasks. Task.WaitAll does not necessarily include them all
                Task.WaitAll(runningTasks.AwaitAll());
            }
            catch (Exception ex)
            {
                Log.To.Main.AddException("At least one task threw an exception in Trigger.Dispose", ex);
            }
        }
    }
}