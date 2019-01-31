using System;
using System.Collections.Generic;

namespace Eir.Common.Rest
{
    public class ResponseObject<T>
    {
        public ResponseObject(T value, IEnumerable<RestError> errors, IEnumerable<RestInformation> informations)
        {
            Value = value;
            Errors = errors ?? new RestError[] { };
            Informations = informations ?? new RestInformation[] { };
        }

        public T Value { get; private set; }
        public IEnumerable<RestError> Errors { get; private set; }
        public IEnumerable<RestInformation> Informations { get; private set; }
    }

    public class RestInformation
    {
        public string Header { get; set; }
        public string Ingress { get; set; }
        public string Message { get; set; }
    }

    public class RestError
    {
        public Guid Id { get; set; }
        public int Code { get; set; }
        public string Header { get; set; }
        public string Ingress { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
    }
}