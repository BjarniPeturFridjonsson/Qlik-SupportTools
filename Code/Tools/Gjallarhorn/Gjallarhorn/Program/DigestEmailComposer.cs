using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using Eir.Common.Common;


namespace Gjallarhorn.Program
{
    public class DigestEmailComposer
    {
        public MailMessage FormatMessage(string tblCusotmers, string tblHosts)
        {
            var msg = new MailMessage();

            string content = "";
            
            var datapointContent = string.Empty;

            if (File.Exists(@"Data\SupportDigest.htm"))
            {
                content = File.ReadAllText(@"Data\SupportDigest.htm", Encoding.UTF8);
            }

            content = content.Replace("[CUSTOMERS]", tblCusotmers);
            content = content.Replace("[HOSTS]", tblHosts);
            var avHtml = AlternateView.CreateAlternateViewFromString(content, null, MediaTypeNames.Text.Html);

            if (File.Exists(@"Data\top_circles.png"))
            {
                var img = new LinkedResource(@"Data\top_circles.png", MediaTypeNames.Image.Jpeg) { ContentId = "Pic1" };
                avHtml.LinkedResources.Add(img);
            }

            msg.AlternateViews.Add(avHtml);

            return msg;
        }
    }
}