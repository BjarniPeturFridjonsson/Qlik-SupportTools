using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FreyrViewer.Ui.Controls.TextPreview
{
    public partial class PreviewText : UserControl
    {
        //https://stackoverflow.com/questions/11642861/highlight-current-line-of-richtextbox
        //scrollinfo
        private bool _isActive;
        //scrollbar stuff
        private const int SB_HORZ = 0x0;
        private const int WM_HSCROLL = 0x114;
        private const int SB_THUMBPOSITION = 4;

        [DllImport("user32.dll")]
        private static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool PostMessageA(IntPtr hWnd, int nBar, int wParam, int lParam);
        [DllImport("user32.dll")]
        private static extern bool GetScrollRange(IntPtr hWnd, int nBar, out int lpMinPos, out int lpMaxPos);

        private const uint OBJID_HSCROLL = 0xFFFFFFFA;
        //private const uint OBJID_VSCROLL = 0xFFFFFFFB;
        //private const uint OBJID_CLIENT = 0xFFFFFFFC;

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "GetScrollBarInfo")]
        private static extern int GetScrollBarInfo(IntPtr hWnd,uint idObject,ref Scrollbarinfo psbi);
        public struct Scrollbarinfo
        {
            public int CbSize;
            public Rect RcScrollBar;
            public int DxyLineButton;
            public int XyThumbTop;
            public int XyThumbBottom;
            public int Reserved;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public int[] Rgstate;
        }

        public struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public PreviewText()
        {
            InitializeComponent();
        }
        
        public void SetText(string text, double yPositionAsPercentage)
        {
            txtPreview.Text = text;

            var info = new Scrollbarinfo();
            info.CbSize = Marshal.SizeOf(info);
            GetScrollBarInfo(txtPreview.Handle, OBJID_HSCROLL, ref info);
            //Position the scrollbar based on the presentage 
            GetScrollRange(txtPreview.Handle, SB_HORZ, out int _, out int vSmaxPos);
            var tumbSize = info.XyThumbBottom - info.XyThumbTop;
            var pos = (int)Math.Floor(vSmaxPos * yPositionAsPercentage);
            pos -= tumbSize/2;

            if (pos < 0) pos = 0;
            SetScrollPos(txtPreview.Handle, SB_HORZ, pos, true);
            PostMessageA(txtPreview.Handle, WM_HSCROLL, SB_THUMBPOSITION + 0x10000 * pos, 0);
            _isActive = true;
        }

        public bool HideIfPossible(bool force=false)
        {
        
            if (ClientRectangle.Contains(PointToClient(MousePosition)))
            {
                if (_isActive)
                {
                    Visible = true;
                    return false;
                }
                    
            }
            Visible = false;
            return true;
        }

        private void PreviewTextDisplay_VisibleChanged(object sender, EventArgs e)
        {
            //dont hide me if the mouse is over me.
          
        }
    }
}
