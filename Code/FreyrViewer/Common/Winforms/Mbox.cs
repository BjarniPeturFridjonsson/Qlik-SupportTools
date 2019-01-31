using System;
using System.Windows.Forms;
using FreyrViewer.Extensions;
using FreyrViewer.Ui.Controls;

namespace FreyrViewer.Common
{
    public static class Mbox
    {
        private static Form _owner;
        /*
         * The new form construction make sure that the popup is modal and topmost nomatter if called from splash or non gui thread.
         */

        public static void SetBaseOwnerForm(Form frm)
        {
            _owner = frm;
        }

        public static void Error(Exception ex, string msg, params object[] parAmour)
        {
            DoError(string.Format(msg, parAmour), ex, false, null);
        }

        public static DialogResult ErrorWithCancel(Exception ex, string ingress, string msg, params object[] parAmour)
        {
            return DoError(string.Format(msg, parAmour), ex, true, null);
        }

        public static DialogResult ErrorWithCancel(Exception ex, string ingress, string msg)
        {
            return DoError(msg, ex, true, ingress);
        }

        public static void Error(string msg, Exception ex)
        {
            DoError(msg, ex, false, null);
        }

        private static DialogResult DoError(string msg, Exception ex, bool showCancel, string ingress)
        {
            if (ex == null)
                ex = new Exception("Unknown exception");
           
            var box = new FrmExceptionDialogue(new WindowsClipboard())
            {
                Ingress = ingress ?? "Exception occured",
                ErrorDetails = ex.ToString(),
                ErrorMsg = msg,
                ShowCancelButton = showCancel
            };
            var owner = GetOwner();
            return WinformExtensions.OnUiThread(owner, ctl => box.ShowDialog(ctl));

            //MessageBox.Show(_owner, msg, SetRightTitle(null), MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void Show(string msg, string title = null)
        {
            var owner = GetOwner();
            WinformExtensions.OnUiThread(owner, ctl => MessageBox.Show(ctl, msg, title));
        }

        public static DialogResult Show(string msg, string title, MessageBoxButtons buttons)
        {
            var owner = GetOwner();
            return WinformExtensions.OnUiThread(owner, ctl => MessageBox.Show(ctl, msg, SetRightTitle(title), buttons));
        }

        public static DialogResult Show(string msg, string title, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            var owner = GetOwner();
            return WinformExtensions.OnUiThread(owner, ctl => MessageBox.Show(ctl, msg, SetRightTitle(title), buttons, icon));
        }

        /// <summary>
        /// This makes sure that mdi child popupforms and dialogue boxes can't wander behind the application.
        /// This happens here because we havent removed all non standard windows message pipe forms (the loader forms)
        /// Dont fuck with the windows message pipe!!!
        /// <para>The new form stuff if we failed to set an owner and the </para>
        /// </summary>
        /// <returns></returns>
        private static Form GetOwner()
        {
            var ret = _owner;
            if (_owner == null || _owner.IsDisposed)
                ret = new Form { TopMost = true };
            return ret;
        }

        private static string SetRightTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                title = "Qlik Proactive Desktop";

            else
            {
                if (!title.Contains("Proactive"))
                    title = "Qlik Proactive Desktop - " + title;
            }
            return title;
        }
    }
}