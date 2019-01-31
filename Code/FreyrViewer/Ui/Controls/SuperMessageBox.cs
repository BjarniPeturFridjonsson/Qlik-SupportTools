using System;
using System.Drawing;
using System.Windows.Forms;

namespace FreyrViewer.Ui.Controls
{
    public partial class SuperMessageBox : Form
    {
        public SuperMessageBox(string title,string ingress, string msg)
        {
            InitializeComponent();
            Text = title;
            lblIngress.Text = ingress;
            lblMessage.Text = msg;
            
        }

        public void SetSize(Point size)
        {
            Width = size.X;
            Height = size.Y;
        }

        private void SuperMessageBox_Resize(object sender, EventArgs e)
        {
            DoResize(sender as Control);
        }

        private void DoResize(Control sender)
        {
            lblIngress.MaximumSize = new Size((sender).ClientSize.Width - lblIngress.Left, 10000);
            lblMessage.Top = lblIngress.Bottom + 15;
            lblMessage.MaximumSize = new Size((sender).ClientSize.Width - lblMessage.Left, 10000);
        }

        private void SuperMessageBox_Load(object sender, EventArgs e)
        {
            this.Width = this.Width + 1;
        }
    }
}
