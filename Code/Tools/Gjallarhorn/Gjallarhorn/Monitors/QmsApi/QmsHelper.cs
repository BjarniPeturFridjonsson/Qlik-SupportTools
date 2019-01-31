using System.Net;
using Newtonsoft.Json.Linq;
using SenseApiLibrary;

namespace Gjallarhorn.Monitors.QmsApi
{
    public class QmsHelper
    {
        public JArray GetJArray(SenseApiSupport senseApiSupport, string url)
        {
            JArray dynamicJson = senseApiSupport.RequestWithResponse(
                ApiMethod.Get,
                $"https://{senseApiSupport.Host}:4242/{url}", //qrs/servicestatus/full",
                null,
                null,
                HttpStatusCode.OK,
                JArray.Parse);
            return dynamicJson;
        }
    }
}
