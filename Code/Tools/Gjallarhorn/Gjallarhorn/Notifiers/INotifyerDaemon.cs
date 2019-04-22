namespace Gjallarhorn.Notifiers
{
    public interface INotifyerDaemon
    {
        bool SendRawMessage();
        void EnqueueMessage(string monitorName, string text);
        string GetBodyTemplate();
    }
}
