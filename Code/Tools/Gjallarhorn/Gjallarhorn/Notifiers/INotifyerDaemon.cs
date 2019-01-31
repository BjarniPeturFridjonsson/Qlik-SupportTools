namespace Gjallarhorn.Notifiers
{
    public interface INotifyerDaemon
    {
        bool SendRawMessage();
        void EnqueueMessage(string text);
        string GetBodyTemplate();
    }
}
