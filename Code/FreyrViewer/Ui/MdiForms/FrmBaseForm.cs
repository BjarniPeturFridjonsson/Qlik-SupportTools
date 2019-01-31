using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using FreyrViewer.Common;

namespace FreyrViewer.Ui.MdiForms
{
    public partial class FrmBaseForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private readonly string _title = "unknown";
        private CommonResources _commonResources;
        private readonly ComponentDisposalHelper _disposalHelper;

        /// <summary>
        /// This is only there for the Visual Studio Forms designer.
        /// </summary>
        private FrmBaseForm()
        {
            InitializeComponent();
        }

        protected FrmBaseForm(CommonResources commonResources, string title)
        {
            _title = title;
            _commonResources = commonResources;
            InitializeComponent();
            _disposalHelper = new ComponentDisposalHelper(this);
            //WireDragDrop(Controls); does not work. needs to run after child initialize I guess.
        }

        public virtual void ShowFilterIfSupported() { }
        public virtual void ManualActivateEvent(object param) { }

        public void ResizeAllToCurrentSize(Control.ControlCollection controls = null)
        {
            Switchboard.Instance.ControlResize.SetCorrectFontSize(controls ?? Controls);
        }

        public void WireDragNdrop()
        {
            WireDragDrop(Controls);
        }

        private void WireDragDrop(Control.ControlCollection ctls)
        {

            foreach (Control ctl in ctls)
            {
                ctl.AllowDrop = true;
                ctl.DragEnter += ctl_DragEnter;
                ctl.DragDrop += ctl_DragDrop;
                WireDragDrop(ctl.Controls);
            }
        }

        private void ctl_DragDrop(object sender, DragEventArgs e)
        {
            Switchboard.Instance.ReceiveDragNdrop(sender,e);
        }

        private void ctl_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        public virtual Task InitAsync(CancellationToken ct)
        {
            return Task.FromResult(false);
        }

        public override string Text
        {
            get => _title;
            set => base.Text = value;
        }

        protected ComponentDisposalHelper DisposalHelper
        {
            get { return _disposalHelper; }
        }

        protected void AddDisposedAction(Action action)
        {
            _disposalHelper.AddDisposedAction(action);
        }

        public void ShowSplash(string message = "Please wait...")
        {
            ctrlSplash.BringToFront();
            ctrlSplash.Text = message;
            ctrlSplash.Visible = true;
        }

        public void HideSplash()
        {
            ctrlSplash.Visible = false;
        }
        private void FrmBaseForm_SizeChanged(object sender, EventArgs e)
        {
            if (ctrlSplash.Visible == false)
                return;
            ctrlSplash.Left = (ClientSize.Width - ctrlSplash.Width) / 2;
            ctrlSplash.Top = (ClientSize.Height - ctrlSplash.Height) / 2;
        }
    }
}
