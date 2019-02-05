using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SenseApiLibrary
{
    public class SenseApiSupport
    {
        private const int ApiTimeoutSeconds = 60;
        static SenseApiSupport()
        {
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true; // TODO! OK...?
        }

        private static readonly Random _random = new Random();

        private readonly SenseApiUser _user;
        private readonly X509Certificate2 _clientCertificate;

        private class Result<T>
        {
            public Result(bool successful, T jsonResponse)
            {
                Successful = successful;
                JsonResponse = jsonResponse;
            }

            public bool Successful { get; }

            public T JsonResponse { get; }
        }

        public static SenseApiSupport CreateHttp(string host)
        {
            

            var exceptions = new List<Exception>(); 
            var hostName = host;
            
            var senseApiSupport = new SenseApiSupport(
                host,
                SenseApiUser.RepositoryServiceAccount,
                null);

            try
            {
                senseApiSupport.RequestWithResponse(
                    ApiMethod.Get,
                    $"http://{hostName}:4242/qrs/about",
                    null,
                    null,
                    HttpStatusCode.OK,
                    JToken.Parse);

                return senseApiSupport;
            }
            catch (Exception exception)
            {
                exceptions.Add(new Exception("Request failed using http " , exception));
            }

            const string message = "Qlik Sense is not accesseble to http.";
            if (exceptions.Count > 0)
            {
                throw new AggregateException(message, exceptions);
            }

            throw new Exception(message);
        }

        public static SenseApiSupport Create(string host)
        {
            var certSupport = new CertSupport();

            var exceptions = new List<Exception>();
            var hostName = host;
           
            foreach (X509Certificate2 clientCertificate in certSupport.GetAllSenseClientCertificates())
            {
                var senseApiSupport = new SenseApiSupport(
                    host,
                    SenseApiUser.RepositoryServiceAccount,
                    clientCertificate);

                try
                {
                    senseApiSupport.RequestWithResponse(
                        ApiMethod.Get,
                        $"https://{hostName}:4242/qrs/about",
                        null,
                        null,
                        HttpStatusCode.OK,
                        JToken.Parse);

                    return senseApiSupport;
                }
                catch (Exception exception)
                {
                    exceptions.Add(new Exception("Request failed using certificate " + clientCertificate.Thumbprint, exception));
                }
            }

            const string message = "No valid Qlik Sense client certificate found.";
            if (exceptions.Count > 0)
            {
                throw new AggregateException(message, exceptions);
            }

            throw new Exception(message);
        }

        private SenseApiSupport(string host, SenseApiUser user, X509Certificate2 clientCertificate)
        {
            Host = host;
            _user = user;
            _clientCertificate = clientCertificate;
        }

        public string Host { get; }
       
        public T RequestWithResponse<T>(
            ApiMethod method,
            string uri,
            ApiQueryField[] apiQueryFields,
            JToken jsonRequest,
            HttpStatusCode expectedStatusCode,
            Func<string, T> parseResponse)
        {
            Result<T> result = Request(
                method,
                uri,
                apiQueryFields,
                jsonRequest,
                expectedStatusCode,
                true,
                parseResponse);

            return result.JsonResponse;
        }

        public bool RequestWithoutResponse(
            ApiMethod method,
            string uri,
            ApiQueryField[] apiQueryFields,
            JToken jsonRequest,
            HttpStatusCode expectedStatusCode)
        {
            Result<bool> result = Request<bool>(
                method,
                uri,
                apiQueryFields,
                jsonRequest,
                expectedStatusCode,
                false,
                null);

            return result.Successful;
        }

        private Result<T> Request<T>(
            ApiMethod method,
            string uri,
            ApiQueryField[] apiQueryFields,
            JToken jsonRequest,
            HttpStatusCode expectedStatusCode,
            bool readJsonResponse,
            Func<string, T> parseResponse)
        {
            Result<T> result;

            string xrfKey = GenerateXrfKey();

            var apiQueryFieldList = new List<ApiQueryField> { new ApiQueryField("Xrfkey", xrfKey) };
            if (apiQueryFields != null)
            {
                apiQueryFieldList.AddRange(apiQueryFields);
            }

            string fullUri = ComposeUri(uri, apiQueryFieldList);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fullUri);

            request.Method = method.ToString().ToUpperInvariant();
            request.Headers.Add("X-Qlik-Xrfkey", xrfKey);
            request.Headers.Add("X-Qlik-User", $"UserDirectory={_user.UserDirectory}; UserId={_user.UserId}");
            request.Accept = "application/json";
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Timeout = (int)TimeSpan.FromSeconds(ApiTimeoutSeconds).TotalMilliseconds;
            if(_clientCertificate != null)
                request.ClientCertificates.Add(_clientCertificate);

            WriteJsonRequest(jsonRequest, request);

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode == expectedStatusCode)
                {
                    string jsonResponse = ReadJsonResponse(readJsonResponse, response);

                    result = new Result<T>(true, parseResponse(jsonResponse));
                }
                else
                {
                    result = new Result<T>(false, default(T));
                }
            }

            return result;
        }

        private static string GenerateXrfKey()
        {
            const string allowedChars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

            return string.Concat(Enumerable.Range(1, 16).Select(x => allowedChars[_random.Next(allowedChars.Length)]));
        }

        private static string ComposeUri(string uri, IEnumerable<ApiQueryField> apiQueryFields)
        {
            var uriBuilder = new StringBuilder(uri);

            bool isFirst = true;

            foreach (ApiQueryField apiQueryField in apiQueryFields)
            {
                if (isFirst)
                {
                    uriBuilder.Append("?");
                    isFirst = false;
                }
                else
                {
                    uriBuilder.Append("&");
                }

                uriBuilder.Append(apiQueryField.GetEscapedString());
            }

            return uriBuilder.ToString();
        }

        private static void WriteJsonRequest(JToken jsonRequest, WebRequest request)
        {
            if (jsonRequest != null)
            {
                byte[] requestBytes = Encoding.UTF8.GetBytes(jsonRequest.ToString(Formatting.None));

                request.ContentLength = requestBytes.Length;
                request.ContentType = "application/json";

                if (requestBytes.Length > 0)
                {
                    using (Stream requestStream = request.GetRequestStream())
                    {
                        requestStream.Write(requestBytes, 0, requestBytes.Length);
                    }
                }
            }
        }

        private static string ReadJsonResponse(bool getResponse, WebResponse response)
        {
            if (!getResponse)
            {
                return null;
            }

            using (Stream responseStream = response.GetResponseStream())
            {
                if (responseStream == null)
                {
                    throw new NullReferenceException("The responseStream is null!");
                }

                using (StreamReader streamReader = new StreamReader(responseStream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }
    }
}