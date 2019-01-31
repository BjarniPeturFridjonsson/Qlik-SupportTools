using System;
using System.Collections.Generic;
using System.Linq;

namespace Eir.Common.CallChain
{
    public interface IInformationLogger
    {
        void Add(string header, string ingress, string message);
        IEnumerable<T> TransformInformations<T>(Func<string, string, string, T> itemTransformFunc);
    }

    public class InformationLogger : IInformationLogger
    {
        private class Information
        {
            public Information(string header, string ingress, string message)
            {
                Header = header;
                Ingress = ingress;
                Message = message;
            }

            public string Header { get; }
            public string Ingress { get; }
            public string Message { get; }
        }

        private readonly List<Information> _informations = new List<Information>();

        public void Add(string header, string ingress, string message)
        {
            _informations.Add(new Information(header, ingress, message));
        }

        /// <summary>
        /// Transforms the contains information elements. The <paramref name="itemTransformFunc"/> get
        /// three string arguments passed to it: header, ingress and message.
        /// </summary>
        public IEnumerable<T> TransformInformations<T>(Func<string, string, string, T> itemTransformFunc)
        {
            return _informations.Select(item => itemTransformFunc(item.Header, item.Ingress, item.Message));
        }
    }
}