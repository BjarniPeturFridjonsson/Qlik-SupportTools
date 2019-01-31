using System;
using System.Collections.Generic;
using System.Linq;
using Eir.Common.Common;
using Eir.Common.Net.Http;
using Gjallarhorn.Notifiers;

namespace Gjallarhorn.Common
{
    public class NotifyerListFactory
    {
        
        private readonly List<string> _teams;
        private readonly List<string> _email;
        private readonly List<string> _slack;
        private readonly List<string> _post;

        private readonly Dictionary<string,string> _monitorNames = new Dictionary<string, string>();
        

        public NotifyerListFactory()
        {
            //todo:Move this to some singular instance method.
            _slack = Settings.GetSetting("UseSlackNotifyer").Split(new []{","}, StringSplitOptions.RemoveEmptyEntries).ToList();
            _email = Settings.GetSetting("UseEmailNotifyer").Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
            _teams = Settings.GetSetting("UseTeamsNotifyer").Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
            _post = Settings.GetSetting("UsePostNotifyer").Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
            //pull out the list of active notifyers.
        }

        public Func<string,IEnumerable<INotifyerDaemon>>  NotifyerListCreator()
        {
            //Pull out the notifyersettings categories (NotificationCategorWorkIntervalInSecondsies)
            return GetDemonsForMonitor;
        }

        private IEnumerable<INotifyerDaemon> GetDemonsForMonitor(string monitorName)
        {
            if (_monitorNames.ContainsKey(monitorName))
            {
                throw new Exception("developer error. did you forget to uniquely name your monitor ehh");
            }

            _monitorNames.Add(monitorName,monitorName);
            var cats = Settings.GetSetting($"{monitorName}.NotificationCategories").Split(new []{","},StringSplitOptions.RemoveEmptyEntries);
            var notifyers = new Dictionary<string,INotifyerDaemon>();

            foreach (string cat in cats)
            {
                NotificationCategories en = (NotificationCategories)Enum.Parse(typeof(NotificationCategories), cat, true);
                if (_slack.Contains(cat))
                {
                    notifyers["slack"] = (new SlackNotifyerDaemon(() => new EirWebClient(), en));
                }
                if (_email.Contains(cat))
                {
                    notifyers["mail"] = (new MailerDaemon(en));
                }
                if (_teams.Contains(cat))
                {
                    notifyers["teams"] = (new MsTeamsNotifyerDaemon(() => new EirWebClient(), en));
                }
                if (_post.Contains(cat))
                {
                    notifyers["post"] = (new HttpPostNotifyerDaemon(() => new EirWebClient(), en));
                }
            }
           
            return notifyers.Values.ToList();
        }
    }
}
