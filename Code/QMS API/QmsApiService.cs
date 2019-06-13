using System;
using System.Diagnostics;
using System.ServiceModel;
using Eir.Common.Logging;
using QMS_API.QMSBackend;
using QMS_API.ServiceSupport;

namespace QMS_API
{
    public abstract class QmsApiService : IDisposable
    {
        private readonly object _syncObj = new object();
        private readonly string _address;
        private QMSClient _client;

        protected QmsApiService(string address)
        {
            _address = address;
        }

        public void Dispose()
        {
            _client?.Close();
        }

        protected QMSClient Client
        {
            get
            {
                lock (_syncObj)
                {
                    QMSClient client = _client ?? (_client = CreateClient());

                    if (client == null)
                    {
                        throw new System.Exception("QMS API client not connected (use Connect())!");
                    }

                    return client;
                }
            }
        }
        
        private QMSClient CreateClient()
        {
            const int maxSize = 26214400;
            TimeSpan receiveTimeout = new TimeSpan(0, 10, 0);
            TimeSpan sendTimeout = new TimeSpan(0, 10, 0);
            TimeSpan innerChannelOperationTimeout = new TimeSpan(0, 10, 0);

            try
            {
                EndpointAddress endpointAddress = new EndpointAddress(_address);
                BasicHttpBinding httpBinding = new BasicHttpBinding();
                httpBinding.MaxBufferSize = maxSize;
                httpBinding.MaxBufferPoolSize = maxSize * 2;
                httpBinding.MaxReceivedMessageSize = maxSize;
                httpBinding.ReceiveTimeout = receiveTimeout;
                httpBinding.OpenTimeout = receiveTimeout;
                httpBinding.SendTimeout = sendTimeout;
                httpBinding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
                httpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Ntlm;
                
                QMSClient client = new QMSClient(httpBinding, endpointAddress);
                QMSClientServiceKeySupport.Initiate(client);
                client.InnerChannel.OperationTimeout = innerChannelOperationTimeout;
                return client;
            }
            catch (System.Exception ex)
            {
                Log.To.Main.AddException("Failed creating Qv Client",ex);
            }

            return null;
        }

        public bool TestConnection()
        {
            try
            {
                Client.GetServices(ServiceTypes.QlikViewServer);
                return true;
            }
            catch (System.Exception ex)
            {
                Log.To.Main.AddException("Failed Testing connection of Qv Client", ex);
                return false;
            }
        }
    }
}