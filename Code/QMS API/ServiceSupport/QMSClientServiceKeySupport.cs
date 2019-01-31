using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using QMS_API.QMSBackend;

namespace QMS_API.ServiceSupport
{
    public static class QMSClientServiceKeySupport
    {
        private class ServiceKeyEndpointBehavior : IEndpointBehavior
        {
            public readonly ServiceKeyClientMessageInspector _clientMessageInspector;

            internal class ServiceKeyClientMessageInspector : IClientMessageInspector
            {
                private const string SERVICE_KEY_HTTP_HEADER = "X-Service-Key";

                public object BeforeSendRequest(ref Message request, IClientChannel channel)
                {
                    object httpRequestMessageObject;
                    if (request.Properties.TryGetValue(HttpRequestMessageProperty.Name, out httpRequestMessageObject))
                    {
                        var httpRequestMessage = httpRequestMessageObject as HttpRequestMessageProperty;
                        if (httpRequestMessage != null)
                        {
                            httpRequestMessage.Headers[SERVICE_KEY_HTTP_HEADER] = (ServiceKey ?? string.Empty);
                        }
                        else
                        {
                            httpRequestMessage = new HttpRequestMessageProperty();
                            httpRequestMessage.Headers.Add(SERVICE_KEY_HTTP_HEADER, (ServiceKey ?? string.Empty));
                            request.Properties[HttpRequestMessageProperty.Name] = httpRequestMessage;
                        }
                    }
                    else
                    {
                        var httpRequestMessage = new HttpRequestMessageProperty();
                        httpRequestMessage.Headers.Add(SERVICE_KEY_HTTP_HEADER, (ServiceKey ?? string.Empty));
                        request.Properties.Add(HttpRequestMessageProperty.Name, httpRequestMessage);
                    }
                    return null;
                }

                public string ServiceKey { get; set; }

                public void AfterReceiveReply(ref Message reply, object correlationState)
                {
                }
            }

            public ServiceKeyEndpointBehavior()
            {
                _clientMessageInspector = new ServiceKeyClientMessageInspector();
            }

            void IEndpointBehavior.Validate(ServiceEndpoint endpoint) { }

            void IEndpointBehavior.AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters) { }

            void IEndpointBehavior.ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher) { }

            void IEndpointBehavior.ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
            {
                clientRuntime.MessageInspectors.Add(_clientMessageInspector);
            }
        }

        public static void Initiate(QMSClient qmsClient)
        {
            var serviceKeyEndpointBehavior = new ServiceKeyEndpointBehavior();
            qmsClient.Endpoint.Behaviors.Add(serviceKeyEndpointBehavior);
            serviceKeyEndpointBehavior._clientMessageInspector.ServiceKey = qmsClient.GetTimeLimitedServiceKey();
        }
    }
}