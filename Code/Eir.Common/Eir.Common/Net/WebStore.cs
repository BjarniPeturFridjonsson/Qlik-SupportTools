using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Eir.Common.Common;
using Eir.Common.Logging;
using Eir.Common.Net.Http;

namespace Eir.Common.Net
{
    public class WebStore
    {
        private readonly int _apiVersion;
        private readonly BaseUris _baseUris;
        private readonly string _userAgent;

        protected WebStore(
            int apiVersion,
            BaseUris baseUris,
            string userAgent)
        {
            _apiVersion = apiVersion;
            _baseUris = baseUris;
            _userAgent = userAgent;
        }

        protected async Task<T> Request<T>(
            UriFragment uriFragment,
            Func<IEirWebClient, Uri, Task<T>> webFunction,
            Trying trying,
            Func<T> getResultIfFail,
            [CallerMemberName] string memberName = null)
        {
            var exceptions = new List<Exception>();

            if (trying == null)
            {
                trying = Trying.Once;
            }

            for (int retry = 0; retry < trying.Count; retry++)
            {
                if (retry > 0)
                {
                    await Task.Delay(trying.Pause).ConfigureAwait(false);
                }


                foreach (Uri baseUri in _baseUris.Uris)
                {
                    try
                    {
                        Uri uri = new Uri(UriSupport.Combine(baseUri.AbsoluteUri, uriFragment));

                        using (IEirWebClient webClient = GetWebClient())
                        {
                            return await webFunction(webClient, uri).ConfigureAwait(false);
                        }
                    }
                    catch (WebException ex)
                    {
                        Log.To.Main.AddException($"Error in request to {uriFragment} on '{baseUri}'", ex);
                        exceptions.Add(ex);
                        if (!ShouldTryNextUri(ex.Status))
                        {
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.To.Main.AddException("WebStore general exception", ex);
                        throw;
                    }
                }
            }

            if (getResultIfFail != null)
            {
                return getResultIfFail();
            }

            var exception = new AggregateException(
                $"Error in {GetType().Name}<{typeof(T).Name}>.{memberName}(\"{uriFragment}\")",
                exceptions);

            Log.To.Main.AddException("WebStore Exception", exception);

            throw exception;
        }

        private bool ShouldTryNextUri(WebExceptionStatus webExceptionStatus)
        {
            return webExceptionStatus != WebExceptionStatus.ProtocolError;
        }

        protected virtual IEirWebClient GetWebClient()
        {
            var wc = new EirWebClient
            {
                Encoding = Encoding.UTF8,
                Credentials = CredentialCache.DefaultCredentials,
                UserAgent = _userAgent,
                UseHttpLog = Settings.GetBool("UseHttpLog", false)
            };

            wc.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            wc.Headers.Add(HttpRequestHeader.Accept, "application/json");

            if (_apiVersion > 1)
            {
                wc.Headers.Add(CustomHttpHeaders.API_VERSION, _apiVersion.ToString());
            }

            return wc;
        }
    }
}