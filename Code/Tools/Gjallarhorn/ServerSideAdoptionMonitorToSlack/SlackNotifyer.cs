using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
//using Eir.Common.Common;
//using Eir.Common.Logging;
//using Eir.Common.Net.Http;

namespace ServerSideAdoptionMonitorToSlack
{
    /// <summary>
    /// This corresponds with slack integration values in settings.xml 
    /// </summary>
   
    public class SlackNotifyer
    {
      

        private class SlackMessageBody
        {
            public string username { get; set; }

            public string text { get; set; }

            public string icon_emoji { get; set; }
            

            public string ToJson() => JsonConvert.SerializeObject(this);
        }

        public async Task PostMessageToSlackAsync(string text)
        {
            try
            {
                string jsonBody = new SlackMessageBody
                {
                    username = "Bjarni's bot",
                    text = text,
                    icon_emoji = ":alien:"
                }.ToJson();

                using (var webClient = new HttpClient())
                {
                    await webClient.PostAsync("https://hooks.slack.com/services/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", new StringContent(jsonBody), CancellationToken.None);
                }
            }
            catch (Exception ex)
            {
               // Log.To.Main.AddException("Error when posting to Slack.", ex);
            }
        }

    }
}
