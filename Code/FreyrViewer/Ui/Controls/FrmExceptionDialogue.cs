using System;
using System.Drawing;
using System.Windows.Forms;
using FreyrViewer.Common;
using FreyrViewer.Ui.Helpers;

namespace FreyrViewer.Ui.Controls
{
    public partial class FrmExceptionDialogue : Form
    {
        private readonly IClipboard _clipboard;
        private readonly int _lblIngressOrgHeight;
        private readonly Bitmap _expandImage;
        private readonly Bitmap _collapseImage;
        private const int MARGIN = 3;
        private const int MAX_LABEL_SIZE_RBEFORE_RESIZING = 60;

        public enum ExceptionIcons
        {
            Application,
            Asterisk,
            Error,
            Exclamation,
            Information,
            Question,
            Warning,
            WinLogo
        }

        /// <summary>
        /// A Exception dialogue with details button and autoresize 
        /// based on the sizes of the texts sent in.
        /// <para>&#160;</para>
        /// <para>Design wise copy of the windows 8 standard exception dlg.</para>
        /// Code inspired by the internal windows GridErrorDlg from 
        /// <para> http://referencesource.microsoft.com/#System.Windows.Forms/winforms/Managed/System/WinForms/PropertyGridInternal/GridErrorDlg.cs,47bef32f7ae3a917 </para>
        /// </summary>
        public FrmExceptionDialogue() : this(new WindowsClipboard())
        {
        }

        public FrmExceptionDialogue(IClipboard clipboard)
        {
            InitializeComponent();
            _clipboard = clipboard;
            _lblIngressOrgHeight = lblIngress.Height;
            _lblIngressOrgHeight = lblIngress.Height;
            _expandImage = new Bitmap(typeof(ThreadExceptionDialog), "down.bmp");
            _expandImage.MakeTransparent();
            _collapseImage = new Bitmap(typeof(ThreadExceptionDialog), "up.bmp");
            _collapseImage.MakeTransparent();
            cmdDetails.Image = _expandImage;
            pictureBox.Image = SystemIcons.Error.ToBitmap();
            Title = "Proactive Desktop";
            Ingress = "Error occured";
        }

        /// <summary>
        /// When you want to use your own image as an icon on the form. Go ahead. 
        /// <br/>
        /// <para>Max size is 64x64 pixels or it will be resized.</para>
        /// </summary>
        /// <param name="icon"></param>
        public void SetIcon(Bitmap icon)
        {
            if (icon.Height > 64 || icon.Width > 64)
            {
                pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            }
            pictureBox.Image = icon;
        }

        /// <summary>
        /// The default is Error, but you can choose the icon that will be shown in the dialogue based on the SystemIcons
        /// </summary>
        /// <param name="icon"></param>
        public void SetIcon(ExceptionIcons icon)
        {
            Bitmap image;
            pictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
            switch (icon)
            {
                case ExceptionIcons.Application:
                    image = SystemIcons.Application.ToBitmap();
                    break;
                case ExceptionIcons.Asterisk:
                    image = SystemIcons.Asterisk.ToBitmap();
                    break;
                case ExceptionIcons.Error:
                    image = SystemIcons.Error.ToBitmap();
                    break;
                case ExceptionIcons.Exclamation:
                    image = SystemIcons.Exclamation.ToBitmap();
                    break;
                case ExceptionIcons.Information:
                    image = SystemIcons.Information.ToBitmap();
                    break;
                case ExceptionIcons.Question:
                    image = SystemIcons.Question.ToBitmap();
                    break;
                case ExceptionIcons.Warning:
                    image = SystemIcons.Warning.ToBitmap();
                    break;
                case ExceptionIcons.WinLogo:
                    image = SystemIcons.WinLogo.ToBitmap();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("icon does not exist", icon, null);
            }
            pictureBox.Image = image;
        }

        /// <summary>
        /// The title of the winform
        /// </summary>
        public string Title
        {
            set { Text = value; }
        }

        /// <summary>
        /// Larger colored text above the Error message itself.
        /// </summary>
        public string Ingress
        {
            get { return lblIngress.Text; }
            set { lblIngress.Text = value; }
        }

        /// <summary>
        /// The error message itself under the Ingress.
        /// </summary>
        public string ErrorMsg
        {
            get { return lblMessage.Text; }
            set { lblMessage.Text = value; }
        }

        /// <summary>
        /// The detailed exception text that is hidden by default.
        /// </summary>
        public string ErrorDetails
        {
            get { return txtDetails.Text; }
            set { txtDetails.Text = value; }
        }

        /// <summary>
        /// You can hide the cancel button if you want.
        /// </summary>
        public bool ShowCancelButton
        {
            set { cmdCancel.Visible = value; }
        }


        private void ToggleDetails()
        {
            if (!txtDetails.Visible)
            {
                cmdDetails.Image = _collapseImage;
                var size = txtDetails.Font.GetTextSize(txtDetails.Text);
                int height = (int)size.Height + MARGIN;
                if (height < 80)
                    height = 80;

                height = Math.Min(height, Screen.PrimaryScreen.WorkingArea.Height);


                //txtDetails.Height = height;
                //txtDetails.Top = txtDetails.Top - height;
                Height = Height + height;
                if (Top + Height > Screen.PrimaryScreen.WorkingArea.Height)
                {
                    Top = Screen.PrimaryScreen.WorkingArea.Height - Height;
                }
                //panContainer.Height = panContainer.Height - height;
                txtDetails.Visible = true;
            }
            else
            {
                //var height = txtDetails.Height;
                cmdDetails.Image = _expandImage;
                txtDetails.Visible = false;

                //panContainer.Height = panContainer.Height + height;
                //txtDetails.Height = height;
                //txtDetails.Top = txtDetails.Top + height;
                Height = this.MinimumSize.Height;
            }
            DoResize();
        }

        private void DoLoad()
        {
            DoResize();
            //if (_lblIngressOrgHeight < lblIngress.Height)
            //{
            //    Height = Height + lblIngress.Height - _lblIngressOrgHeight;
            //}
            //if ((lblIngress.Height + lblMessage.Height) > MAX_LABEL_SIZE_RBEFORE_RESIZING)
            //    Height = Height + (lblIngress.Height + lblMessage.Height) - MAX_LABEL_SIZE_RBEFORE_RESIZING;
        }

        private void DoResize()
        {
            //int rightMargin = 40;
            //lblIngress.MaximumSize = new Size(Bounds.Width - (lblIngress.Left + rightMargin), 0);
            //lblMessage.MaximumSize = new Size(Bounds.Width - (lblMessage.Left + rightMargin), 0);
            //lblMessage.Top = lblIngress.Top + lblIngress.Height + MARGIN;
            //lblLine.Top = panContainer.Top + panContainer.Height;
        }

        private void FrmExceptionViewer_Load(object sender, EventArgs e)
        {
            DoLoad();
        }

        private void CmdDetails_Click(object sender, EventArgs e)
        {
            ToggleDetails();
        }

        private void cmdCopyToClipboard_Click(object sender, EventArgs e)
        {
            SendContentToClipboard();
        }

        private void cmdOk_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FrmExceptionViewer_Resize(object sender, EventArgs e)
        {
            DoResize();
        }

        private void txtDetails_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A) txtDetails.SelectAll();
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        public void SendContentToClipboard()
        {
            _clipboard.SendToClipboard(GetClipboardText());
        }

        private string GetClipboardText()
        {
            return $"--------------------\n{Text}\n--------------------\n{Ingress}\n--------------------\n{ErrorDetails}\n--------------------\n{ErrorMsg}\n--------------------";
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Check if CTRL+C was pressed.
            if (keyData == (Keys.Control | Keys.C))
            {
                SendContentToClipboard();
                return true; // indicates that we have handled the command, so don't send to focused control.
            }

            if (keyData.HasFlag(Keys.Escape))
            {
                Close();
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}