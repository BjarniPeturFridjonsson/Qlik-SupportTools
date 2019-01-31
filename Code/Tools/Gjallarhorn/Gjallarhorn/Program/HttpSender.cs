using System;
using System.Net;
using System.Text;
using Eir.Common.Logging;

namespace Gjallarhorn.Program
{
    public class HttpSender
    {
        private readonly string _userAgent;
        public HttpSender()
        {
            _userAgent = "";
        }

        internal bool Post(string uri, string json)
        {
            try
            {
                bool success = false;
                for (int i = 0; i < 10; i++)
                {
                    try
                    {
                        using (var wc = GetWebClient())
                        {
                            wc.UploadString(uri, json);
                            wc.Dispose();
                        }
                        success = true;
                        break;
                    }
                    catch (Exception ex)
                    {
                        Log.To.Main.Add("Retry " + i + " of 10: " + ex.Message);
                    } // ignore
                }
                if (!success) throw new Exception("Max retries reached");
            }
            catch (WebException webException)
            {
                var logger = new StringBuilder();
                logger.Append("Could not connect to Qlik backend at " + uri);
                logger.Append(" Verb: POST ");
                logger.Append(webException);
                logger.Append(" Content: " + json);
                Log.To.Main.Add(logger.ToString());
                return false;
            }
            catch (Exception ex)
            {
                var logger = new StringBuilder();

                logger.Append("URI: " + uri);
                logger.Append("Verb: POST");
                logger.Append(ex);
                logger.Append("Content: " + json);
                Log.To.Main.Add(logger.ToString());
                return false;
            }
            return true;
        }

        internal string Get(string uri)
        {
            try
            {
                using (var wc = GetWebClient())
                {
                    var result = wc.DownloadString(uri);
                    return result;
                }
            }
            catch (WebException webException)
            {
                var logger = new StringBuilder();
                logger.Append("Could not connect to Qlik backend at " + uri);
                logger.Append(" Verb: POST ");
                logger.Append(webException);
                Log.To.Main.Add(logger.ToString());
                return string.Empty;
            }
            catch (Exception ex)
            {
                var logger = new StringBuilder();

                logger.Append("URI: " + uri);
                logger.Append("Verb: POST");
                logger.Append(ex);
                Log.To.Main.Add(logger.ToString());
                return string.Empty;
            }
        }

        private WebClient GetWebClient()
        {
            var wc = new WebClient
            {
                Encoding = Encoding.UTF8,

            };
            wc.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            wc.Headers.Add(HttpRequestHeader.UserAgent, _userAgent);
            return wc;
        }
    }
}

