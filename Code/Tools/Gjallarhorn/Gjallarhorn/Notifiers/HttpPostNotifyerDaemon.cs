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
        private readonly string _settingsGropName;

      
        public HttpPostNotifyerDaemon(Func<IEirWebClient> webClientFactory, NotificationCategories settingsGropName)
        {
            _webClientFactory = webClientFactory;
            _settingsGropName = settingsGropName.ToString().ToLower();
        }

        public bool SendRawMessage()
        {
            return true;
        }

        public void EnqueueMessage(string monitorName, string text)
        {
            Task.Run(() => PostMessage(text));
        }

        public string GetBodyTemplate()
        {
            return "";
        }

        private async Task PostMessage(string text)
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
                Log.To.Main.AddException("Error when posting to HttpPostDeamon.", ex);
            }
        }

        private Uri Uri => new Uri(Settings.GetSetting($"httpPost.{_settingsGropName}.url"));
    }
}
