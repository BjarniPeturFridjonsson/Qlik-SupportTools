using System.Windows.Forms;

namespace FreyrViewer.Common
{
    internal class WindowsClipboard : IClipboard
    {
        public void SendToClipboard(string text)
        {
            Clipboard.SetData(DataFormats.StringFormat, text);
        }
    }

    public interface IClipboard
    {
        void SendToClipboard(string text);
    }
}