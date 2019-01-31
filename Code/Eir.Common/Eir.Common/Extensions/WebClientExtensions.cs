//using System.Net;
//using System.Threading;
//using System.Threading.Tasks;
//using Eir.Common.Net.Http;

//namespace Eir.Common.Extensions
//{
//    public static class WebClientExtensions
//    {
//        public static Task<string> DownloadStringAsync(this IEirWebClient webClient, string uri, CancellationToken ct)
//        {
//            using (ct.Register(webClient.CancelAsync))
//            {
//                return webClient.DownloadStringAsync(uri);
//            }
//        }

//        public static Task<string> UploadStringAsync(this IEirWebClient webClient, string uri, string data, CancellationToken ct)
//        {
//            using (ct.Register(webClient.CancelAsync))
//            {
//                return webClient.UploadStringAsync(uri, data);
//            }
//        }

//        public static Task<string> UploadStringAsync(this IEirWebClient webClient, string uri, string method, string data, CancellationToken ct)
//        {
//            using (ct.Register(webClient.CancelAsync))
//            {
//                return webClient.UploadStringAsync(uri, method, data);
//            }
//        }
//    }
//}