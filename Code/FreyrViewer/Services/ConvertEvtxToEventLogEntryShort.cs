using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Eir.Common.Time;
using FreyrCommon.Models;

namespace FreyrViewer.Services
{
    class ConvertEvtxToEventLogEntryShort
    {
        public List<string> ConversionExceptions { get; set; } = new List<string>();
        private int _convertingItem=0;
        private PauseTrigger _trigger;
        private Action<string> _notify;

        private Task Notify()
        {
            _notify.Invoke($"Converting Windows log line {_convertingItem}");
            return Task.FromResult(false);
        }

        public List<EventLogEntryShort> Convert(string path,Action<string> notify)
        {
            _trigger = new PauseTrigger(() => TimeSpan.FromSeconds(1));
            _notify = notify;
            _trigger.RegisterAction(Notify);
            ConversionExceptions = new List<string>();
            var data = new List<EventLogEntryShort>();
            var beforeCulture = Thread.CurrentThread.CurrentCulture;
            //var lineCount = 0;
            try
            {
                //bug in .net been around for ages and not fixed in 4.0
                //https://stackoverflow.com/questions/7531557/why-does-eventrecord-formatdescription-return-null#7531575
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                using (var reader = new EventLogReader(path, PathType.FilePath))
                {
                    EventRecord record;
                    while ((record = reader.ReadEvent()) != null)
                    {
                        using (record)
                        {
                            string desc ="";
                            string level = null;
                            try
                            {
                                //failes if locale is not US/?? weird
                                desc = record.FormatDescription() + "";
                                
                                level = record.Level.HasValue ? ((EventLogEntryType) record.Level.Value).ToString() : "(unknown)";
                            }
                            catch (Exception e)
                            {
                                ConversionExceptions.Add($"Failed on line {_convertingItem} with {e}");
                            }
                            data.Add(new EventLogEntryShort
                            {
                                InstanceId = record.RecordId.GetValueOrDefault(-1),
                                Level = level,
                                Logged = record.TimeCreated.GetValueOrDefault(),
                                Source = record.ProviderName,
                                Message = desc,
                                LogName = record.LogName,
                                User = record.UserId?.Value + ""

                            });
                        }
                        _convertingItem++;
                    }
                    
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine($"hmmm =>{e}");
                throw;
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = beforeCulture;
                _notify.Invoke($"Finished converting log with {_convertingItem}nr of lines");
                _trigger.UnregisterAction(Notify);
                _trigger.Dispose();
                
            }
            return data;

        }
    }
}
