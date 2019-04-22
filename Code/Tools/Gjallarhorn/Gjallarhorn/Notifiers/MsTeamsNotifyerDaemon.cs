using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Eir.Common.Common;
using Eir.Common.Logging;
using Eir.Common.Net.Http;
using Gjallarhorn.Common;

namespace Gjallarhorn.Notifiers
{
    internal class MsTeamsNotifyerDaemon : INotifyerDaemon
    {
        private Func<IEirWebClient> _webClientFactory;
        private string _slackSettingsGropName;

        public MsTeamsNotifyerDaemon(Func<IEirWebClient> webClientFactory, NotificationCategories slackSettingsGropName)
        {
            //_httpClient = httpClient;
            _webClientFactory = webClientFactory;
            _slackSettingsGropName = slackSettingsGropName.ToString().ToLower();
        }

        public bool SendRawMessage()
        {
            return false;
        }

        public void EnqueueMessage(string monitorName, string text)
        {
            //Task.Run(() => Send(text));
        }

        public string GetBodyTemplate()
        {
            return Settings.GetSetting($"msTeams.{_slackSettingsGropName}.Template");
        }

        public void Notify()
        {
            try
            {
                throw new NotImplementedException("Teams intigration is not finished");
                //var notify = new Common.MsTeamsIntegrationService(new HttpClientWrapper());
                //await notify.Send(Settings.GetSetting("msTeams.Template", ""), CreateValueDict(analyzerHistoryItem), Settings.GetSetting("msTeams.WebHookUrl", ""));
            }
            catch (Exception e)
            {
                Log.To.Main.AddException("Failed creating msTeams card.", e);
            }
        }

        private async Task Send(string template, Dictionary<string, string> values, string webHookUrl)
        {
            if (string.IsNullOrEmpty(template))
                throw new Exception("The template can't be empty when trying to send card to MsTeams");
            if (values != null)
            {
                foreach (KeyValuePair<string, string> key in values)
                {
                    template = template.Replace($"<tag:{key.Key}>", key.Value);
                }
            }
            await Post(webHookUrl, template);
        }

        private async Task Post(string webHookUrl, string cardData)
        {
            using (var webClient = _webClientFactory())
            {
                await webClient.UploadStringAsync(new Uri(webHookUrl), HttpMethod.Post, cardData, CancellationToken.None);
            }
        }

       
    }
}
