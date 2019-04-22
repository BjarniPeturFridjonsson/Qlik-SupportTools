using System;
using System.Net.Mail;
using System.Threading.Tasks;
using Eir.Common.Common;
using Eir.Common.Logging;
using Gjallarhorn.Common;

namespace Gjallarhorn.Notifiers
{
    public class MailerDaemon : INotifyerDaemon
    {
        private readonly string _slackSettingsGropName;

        public MailerDaemon(NotificationCategories notificationCategories)
        {
            _slackSettingsGropName = notificationCategories.ToString().ToLower();
        }

        //private readonly BlockingCollection<MailMessage> _mailMessages = new BlockingCollection<MailMessage>();
        //private CancellationTokenSource _cancellationTokenSource;
        //private Thread _workerThread;

        public bool SendRawMessage()
        {
            return false;
        }

        public void EnqueueMessage(string monitorName, string text)
        {
            Task.Run(() => CreateMailAndSend(text));
        }

        public string GetBodyTemplate()
        {
            return Settings.GetSetting($"Mail.{_slackSettingsGropName}.Template");
        }

        private void CreateMailAndSend(string text)
        {
            MailMessage msg = null;
            try
            {
                var subject = Settings.GetSetting($"Mail.{_slackSettingsGropName}.Subject");
                subject = subject.Replace("{serviceName}", Constants.SERVICE_NAME)
                    .Replace("{time}", DateTimeProvider.Singleton.Time().ToString("yyyy-MM-dd HH:mm:ss"));

                var body = text;

                msg = new MailMessage
                {
                    Subject = subject,
                    Body = body,
                    From = new MailAddress(Settings.GetSetting($"Mail.{_slackSettingsGropName}.From")),
                    IsBodyHtml = false,
                };
                
                msg.To.Add(new MailAddress(Settings.GetSetting($"Mail.{_slackSettingsGropName}.Recipients")));
                if(!string.IsNullOrWhiteSpace(Settings.GetSetting($"Mail.{_slackSettingsGropName}.Recipients.CC")))
                    msg.CC.Add(new MailAddress(Settings.GetSetting($"Mail.{_slackSettingsGropName}.Recipients.CC")));
                if (!string.IsNullOrWhiteSpace(Settings.GetSetting($"Mail.{_slackSettingsGropName}.Recipients.BCC")))
                    msg.Bcc.Add(new MailAddress(Settings.GetSetting($"Mail.{_slackSettingsGropName}.Recipients.BCC")));
                if (TrySendMail(msg))
                {
                    Log.To.Main.Add("Mail sent to '" + msg.To + "'");
                }
                else
                {
                    Log.To.Main.Add("Failed to send mail to '" + msg.To + "'", LogLevel.Error);
                }
            }
            catch (Exception ex)
            {
                Log.To.Main.AddException($"Failed creating mail with subject {msg?.Subject ?? "NULL"}", ex);
            }
           
        }

       
        private bool TrySendMail(MailMessage msg)
        {
            try
            {
#if DEBUG
                //Trace.WriteLine("======SENDING MAIL======");
                //Trace.WriteLine("subject: " + msg.Subject);
                //Trace.WriteLine("body: " + msg.Body);
                //Trace.WriteLine("========================");
#else
#endif
                var client = new SmtpClient();
                client.Send(msg);


                return true;
            }
            catch (Exception ex)
            {
                
                Log.To.Main.AddException($"Failed sending mail with subject {msg.Subject}",ex);
                return false;
            }
        }
    }

  

 
}