using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace FreyrViewer.Common.Winforms
{
    public class DateTimePickerHelper
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        private const uint WM_SYSKEYDOWN = 0x104;

        public void Open(DateTimePicker obj)
        {
            if (obj == null) return;
            SendMessage(obj.Handle, WM_SYSKEYDOWN, (int)Keys.Down, 0);
        }
    }
}
