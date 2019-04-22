using System;
using System.Collections.Generic;
using System.Linq;
using Eir.Common.Common;
using Eir.Common.Logging;
using Gjallarhorn.Common;
using Gjallarhorn.Notifiers;

namespace Gjallarhorn.Monitors
{
    public class BaseMonitor
    {
        private readonly IEnumerable<INotifyerDaemon> _notifyerDaemons;
        private string _lastSentMessage = string.Empty;
        private DateTime _badStuffFlush = DateTimeProvider.Singleton.Today;

        public string MonitorName { get; }
        public DateTime NextExec { get; set; }

        public BaseMonitor(Func<string,IEnumerable<INotifyerDaemon>> notifyerDaemonsCreator, string monitorName)
        {
            try
            {
                MonitorName = monitorName;
                _notifyerDaemons = notifyerDaemonsCreator.Invoke(monitorName);
            }
            catch (Exception e)
            {
                Log.To.Main.AddException("Failed getting the Notifyer deamons from the factory", e);
                throw;
            }
            
        }

        public virtual string GetDigestMessages()
        {
            return "";
        }

        public virtual void Stop() { }

        /// <summary>
        /// this vill send an ok signal if this monitor has given a ads fire message to the notifiers.
        /// </summary>
        protected void ResetNotification(AdsRuleSetting setting)
        {
            Notify("",new List<string>());
        }

        ///// <summary>
        ///// Sends notification to the list of notifier active.
        ///// </summary>
        ///// <param name="header">the header of the message</param>
        ///// <param name="message">The message it self</param>
        ///// <param name="overideLastSentMessageKey">
        /////     When you have dynamic values in your message you might considere having a key that is non dynamic to stop spamming the channel.
        /////     cpu and memory would trigger every time, but if the msgKey is machine_ruleName will only send update to channel on start/stop of the rule.
        ///// </param>
        ////protected void Notify(string header, string message, string overideLastSentMessageKey ="")
        ////{
        ////    Notify(header,new List<string>{ message}, overideLastSentMessageKey);
        ////}

        /// <summary>
        /// Sends notification to the list of notifier active.
        /// </summary>
        /// <param name="header">the header of the message</param>
        /// <param name="messages">The message it self</param>
        /// <param name="overideLastSentMessageKey">
        ///     When you have dynamic values in your message you might considere having a key that is non dynamic to stop spamming the channel.
        ///     cpu and memory would trigger every time, but if the msgKey is machine_ruleName will only send update to channel on start/stop of the rule.
        /// </param>
        /// <param name="force">will send message eveythime and therefore ignore the spam filter</param>
        protected void Notify(string header, List<string> messages, string overideLastSentMessageKey = "")
        {
            //we resend the bad stuff once a day.
            if (_badStuffFlush < DateTimeProvider.Singleton.Today)
            {
                _lastSentMessage = string.Empty;
                _badStuffFlush = DateTimeProvider.Singleton.Today;
            }

            if (messages.Count > 0)
            {
                string sHeader = string.IsNullOrEmpty(header) ? $"*{MonitorName} has found the following issues*" : $"*{header}*";
                //messages.Insert(0, sHeader);
                var msg = sHeader + messages.Aggregate((value, item) => value + Environment.NewLine + item);

                string msgKey = string.IsNullOrEmpty(overideLastSentMessageKey) ? msg : overideLastSentMessageKey;


                //todo://superhack. Build notifyer settings so we can in settings set up spamfilters or always send.
                if (overideLastSentMessageKey !="-1" && _lastSentMessage.Equals(msgKey))
                {
                    return; // we don't want to spam the channels.  this gets reset once a day.
                }
               
                foreach (var notifyerDaemon in _notifyerDaemons)
                {
                    if (notifyerDaemon.SendRawMessage())
                    {
                        messages.ForEach(p=> notifyerDaemon.EnqueueMessage(MonitorName, p));
                        continue;    
                    }
                    var template = notifyerDaemon.GetBodyTemplate();
                    var outMsg = template.Replace("{MonitorName}", MonitorName)
                            .Replace("{time}", DateTimeProvider.Singleton.Time().ToString("yyyy-MM-dd HH:mm:ss"))
                            .Replace("{message}", msg)
                            .Replace(@"\r\n", Environment.NewLine);

                    notifyerDaemon.EnqueueMessage(MonitorName, outMsg);
                }

                Log.To.AddToDynamicLog(new NotificationLogItem(DateTimeProvider.Singleton.Time(), LogLevel.Info,MonitorName, msg.Replace(Environment.NewLine,"<crlf>")));
                _lastSentMessage = msgKey;
            }
            else
            {
                if (_lastSentMessage != string.Empty)
                {
                    var allClearMsg = $"{MonitorName} monitoring is looking good now.";
                    foreach (var notifyerDaemon in _notifyerDaemons)
                    {
                        notifyerDaemon.EnqueueMessage(MonitorName, allClearMsg);
                    }
                    Log.To.AddToDynamicLog(new NotificationLogItem(DateTimeProvider.Singleton.Time(), LogLevel.Info, MonitorName, allClearMsg));
                    _lastSentMessage = string.Empty;
                }
            }   
        }
    }
}
