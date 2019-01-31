using System;
using System.Diagnostics;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Eir.Common.Logging;

namespace Eir.Common.Net.Http
{
    public class EirWebClient : IEirWebClient
    {
        private class WorkEntity<T>
        {
            public T Result { get; }
            public long RequestLength { get; }
            public long ResponseLength { get; }

            public WorkEntity(T result, long requestLength, long responseLength)
            {
                Result = result;
                RequestLength = requestLength;
                ResponseLength = responseLength;
            }
        }

        static EirWebClient()
        {
            ServicePointManager.ServerCertificateValidationCallback = ServerCertificateValidationCallback;
        }

        /// <summary>
        /// Gets or sets the maximum number of concurrent connections (default is 2 in windows and a recommendation in RFC).
        /// </summary>
        public bool UseHttpLog { get; set; }

        public string UserAgent { get; set; }

        public Encoding Encoding { get; set; }

        public ICredentials Credentials { get; set; }

        public WebHeaderCollection Headers { get; }

        public EirWebClient()
        {
            Encoding = Encoding.UTF8;
            Headers = new WebHeaderCollection();
        }

        public byte[] UploadData(Uri uri, HttpMethod method, byte[] data)
        {
            return DoWork(webClient =>
            {
                var result = webClient.UploadData(uri, method.ToString().ToUpper(), data);
                return new WorkEntity<byte[]>(result, data.Length, result?.Length ?? 0);
            }, uri, method);
        }

        public Task<byte[]> UploadDataAsync(Uri uri, HttpMethod method, byte[] data, CancellationToken ct)
        {
            return DoWorkAsync(async webClient =>
                {
                    using (ct.Register(webClient.CancelAsync))
                    {
                        var result = await webClient.UploadDataTaskAsync(uri, method.ToString().ToUpper(), data);
                        return new WorkEntity<byte[]>(result, data.Length, result?.Length ?? 0);
                    }
                },
                uri,
                method);
        }

        public Task<string> UploadStringAsync(Uri uri, HttpMethod method, string data, CancellationToken ct)
        {
            return DoWorkAsync(async webClient =>
                {
                    using (ct.Register(webClient.CancelAsync))
                    {
                        var result = await webClient.UploadStringTaskAsync(uri, method.ToString().ToUpper(), data);
                        return new WorkEntity<string>(result, data.Length, result?.Length ?? 0);
                    }
                },
                uri,
                method);
        }

        public Task<string> DownloadStringAsync(Uri uri, CancellationToken ct)
        {
            return DoWorkAsync(async webClient =>
                {
                    using (ct.Register(webClient.CancelAsync))
                    {
                        var result = await webClient.DownloadStringTaskAsync(uri);
                        return new WorkEntity<string>(result, 0, result?.Length ?? 0);
                    }
                },
                uri,
                HttpMethod.Get);
        }



        public string DownloadString(Uri uri)
        {
            return DoWork(webClient =>
            {
                var result = webClient.DownloadString(uri);
                return new WorkEntity<string>(result, 0, result.Length);
            }, uri, HttpMethod.Get);
        }

        public string UploadString(Uri uri, HttpMethod method, string data)
        {
            return DoWork(webClient =>
            {
                var result = webClient.UploadString(uri, data);
                return new WorkEntity<string>(result, data.Length, result.Length);
            }, uri, method);
        }

        private static int GetHttpStatusCodeFromWex(WebException wex)
        {
            try
            {
                var response = wex.Response as HttpWebResponse;
                if (response != null)
                    return (int)response.StatusCode;
                return -2;
            }
            catch
            {
                return -1;
            }
        }

        private T DoWork<T>(Func<WebClient, WorkEntity<T>> work, Uri uri, HttpMethod method)
        {
            int httpStatusCode = -3;
            long responseLength = -1;
            long requestLength = -1;
            Stopwatch requestTimer = UseHttpLog ? Stopwatch.StartNew() : null;

            try
            {
                using (WebClient webClient = CreateWebClient())
                {
                    WorkEntity<T> res = work(webClient);
                    httpStatusCode = 200;
                    responseLength = res?.ResponseLength ?? 0;
                    requestLength = res?.RequestLength ?? 0;


                    return res == null ? default(T) : res.Result;
                }
            }
            catch (WebException wex)
            {
                httpStatusCode = GetHttpStatusCodeFromWex(wex);
                throw;
            }
            finally
            {
                if (requestTimer != null)
                {
                    requestTimer.Stop();
                    HttpLog(requestTimer.ElapsedMilliseconds, uri, method, requestLength, responseLength, httpStatusCode);
                }
            }
        }

        private async Task<T> DoWorkAsync<T>(Func<WebClient, Task<WorkEntity<T>>> work, Uri uri, HttpMethod method)
        {
            int httpStatusCode = -3;
            long responseLength = -1;
            long requestLength = -1;
            Stopwatch requestTimer = UseHttpLog ? Stopwatch.StartNew() : null;

            try
            {
                using (WebClient webClient = CreateWebClient())
                {
                    WorkEntity<T> res = await work(webClient).ConfigureAwait(false);
                    httpStatusCode = 200;
                    responseLength = res?.ResponseLength ?? 0;
                    requestLength = res?.RequestLength ?? 0;


                    T result = res == null ? default(T) : res.Result;
                    return result;
                }
            }
            catch (WebException wex)
            {
                httpStatusCode = GetHttpStatusCodeFromWex(wex);
                throw;
            }
            finally
            {
                if (requestTimer != null)
                {
                    requestTimer.Stop();
                    HttpLog(requestTimer.ElapsedMilliseconds, uri, method, requestLength, responseLength, httpStatusCode);
                }
            }
        }

        private WebClient CreateWebClient()
        {
            var webClient = new WebClient
            {
                Credentials = Credentials,
                Encoding = Encoding,
                Headers = Headers
            };

            webClient.Headers[HttpRequestHeader.UserAgent] = UserAgent;
            return webClient;
        }

        private void HttpLog(long responseTimeInMs, Uri uri, HttpMethod httpMethod, long requestContentLength, long responseLengthInBytes, int statusCode)
        {
            try
            {
                Log.To.Http.Add(
                    requestHttpMethod: httpMethod,
                    requestRawUrl: uri.AbsoluteUri,
                    requestUserAgent: UserAgent,
                    requestRemoteEndPointAddress: Environment.MachineName,
                    responseTimeInMs: responseTimeInMs,
                    requestContentLength: requestContentLength,
                    responseLengthInBytes: responseLengthInBytes,
                    statusCode: statusCode);
            }
            catch (Exception ex)
            {
                Log.To.Main.AddException("Error when writing http log", ex);
            }
        }

        private static bool ServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                return true;
            }

            string uri = (sender as WebRequest)?.RequestUri.ToString() ?? "<unknown uri>";

            Log.To.Main.Add(
                $"Request for uri '{uri}' resulted in certificate validation error '{sslPolicyErrors}'. Certificate: {certificate}",
                LogLevel.Error);

            return false;
        }

        public void Dispose()
        {
        }
    }
}