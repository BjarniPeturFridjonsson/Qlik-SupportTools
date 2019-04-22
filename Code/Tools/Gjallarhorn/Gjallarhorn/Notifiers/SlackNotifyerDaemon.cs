using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Eir.Common.Common;
using Eir.Common.Logging;
using Eir.Common.Net.Http;
using Eir.Common.Security;
using Gjallarhorn.Common;
using Newtonsoft.Json;

namespace Gjallarhorn.Notifiers
{
    internal class SlackNotifyerDaemon : INotifyerDaemon
    {
        private readonly Func<IEirWebClient> _webClientFactory;
        private readonly string _slackSettingsGropName;

        private class SlackMessageBody
        {
            public string username { get; set; }

            public string text { get; set; }

            public string icon_emoji { get; set; }

            public string ToJson() => JsonConvert.SerializeObject(this);
        }

        public SlackNotifyerDaemon(Func<IEirWebClient> webClientFactory, NotificationCategories slackSettingsGropName)
        {
            _webClientFactory = webClientFactory;
            _slackSettingsGropName = slackSettingsGropName.ToString().ToLower();
        }

        public bool SendRawMessage()
        {
            return false;
        }

        public void EnqueueMessage(string monitorName, string text)
        {
            Task.Run(()=>PostMessageToSlackAsync(text)) ;
        }

        private async Task PostMessageToSlackAsync(string text)
        {
            try
            {
                string jsonBody = new SlackMessageBody
                {
                    username = SlackSenderName,
                    text = text,
                    icon_emoji = SlackSenderEmoji
                }.ToJson();

                using (var webClient = _webClientFactory())
                {
                    var username = Settings.GetSetting("General.WebhookAlternativeCredentials.UserName");
                    var pass = Settings.GetSetting("General.WebhookAlternativeCredentials.EncryptedPassword");
                    if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(pass))
                    {
                        webClient.Credentials = new NetworkCredential(username, new Encryption().Decrypt(pass, NotificationHelper.NotificationCategoryHelper));
                    }
                    
                    await webClient.UploadStringAsync(SlackUri, HttpMethod.Post, jsonBody, CancellationToken.None);
                }
            }
            catch (Exception ex)
            {
                Log.To.Main.AddException("Error when posting to Slack.", ex);
            }
        }

        public string GetBodyTemplate()
        {
            return Settings.GetSetting($"slack.{_slackSettingsGropName}.Template");
        }

        private Uri SlackUri => new Uri(Settings.GetSetting($"slack.{_slackSettingsGropName}.webhook.url"));

        private string SlackSenderName => Settings.GetSetting($"slack.{_slackSettingsGropName}.sender.name", Settings.GetSetting($"slack.{_slackSettingsGropName}.sender.subject"));

        private string SlackSenderEmoji => ":" + Settings.GetSetting($"slack.{_slackSettingsGropName}.sender.emoji", "warning") + ":";
    }
}
