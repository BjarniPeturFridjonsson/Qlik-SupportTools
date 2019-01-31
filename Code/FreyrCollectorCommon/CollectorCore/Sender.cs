using System;
using System.Net;

namespace FreyrCollectorCommon.CollectorCore
{
    public interface ISender
    {
        void Transport(Uri recceverPath, string localFileName);
    }

    public class SenderBase
    {
        public string SendGroupId { get; }
        public string CaseId { get; }

        public SenderBase(string caseId, string sendGroupId)
        {
            SendGroupId = sendGroupId;
            CaseId = caseId;
        }
    }

    public class Sender : SenderBase, ISender
    {
        public Sender(string caseId, string sendGroupId) : base(caseId, sendGroupId)
        {
        }

        public void Transport(Uri recceverPath, string localFileName)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("QlikCaseId", CaseId);
                client.Headers.Add("SendId", SendGroupId);
                client.UploadFile(recceverPath, localFileName);
            }
        }

    }
}
