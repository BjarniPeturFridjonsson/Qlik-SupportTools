using System;
using System.Threading;
using System.Threading.Tasks;
using Eir.Common.Common;
using Eir.Common.Logging;
using Eir.Common.Net.Http;
using Gjallarhorn.Common;


namespace Gjallarhorn.Notifiers
{
    public class HttpPostNotifyerDaemon : INotifyerDaemon
    {
        private readonly Func<IEirWebClient> _webClientFactory;
        private readonly string _slackSettingsGropName;

      
        public HttpPostNotifyerDaemon(Func<IEirWebClient> webClientFactory, NotificationCategories slackSettingsGropName)
        {
            _webClientFactory = webClientFactory;
            _slackSettingsGropName = slackSettingsGropName.ToString().ToLower();
        }

        public bool SendRawMessage()
        {
            return true;
        }

        public void EnqueueMessage(string text)
        {
            Task.Run(() => PostMessageToSlackAsync(text));
        }

        public string GetBodyTemplate()
        {
            return "";
        }

        private async Task PostMessageToSlackAsync(string text)
        {
            try
            {
                using (var webClient = _webClientFactory())
                {
                    await webClient.UploadStringAsync(Uri, HttpMethod.Post, text, CancellationToken.None);
                }
            }
            catch (Exception ex)
            {
                Log.To.Main.AddException("Error when posting to Slack.", ex);
            }
        }

        private Uri Uri => new Uri(Settings.GetSetting($"httpPost.{_slackSettingsGropName}.url"));
    }
}
