using System;
using System.Reflection;
using System.Windows.Forms;

namespace FreyrViewer.Ui
{
    public partial class FrmAbout : Form
    {
        public FrmAbout()
        {
            InitializeComponent();
            SetLabelInformation();
        }
        private void SetLabelInformation()
        {
            lblVersionInfo.Text = $@"Qlik Case Cockpit v{Assembly.GetExecutingAssembly().GetName().Version} Copyright {DateTime.Now.Year} Qlik Technologies Inc";
            lblVersionInfo.Parent = picBackground;
            lblMarquee.Parent = picBackground;
            _marquee = new string(' ', 150) + _marquee;
        }

        
        private string _marquee = @"This program was developed by Bjarni Fridjonsson with the help of Stefan, Sebastian and Christer. I wan't to extend my thanks to the suggestions made by, Bastian, Chotana, Sonja, Chris, John, Levi, Andrew, Mario, Maria and Filippo                              ";
        private void tmrTick_Tick(object sender, EventArgs e)
        {
            
            if (lblMarquee.Text.Length == 0)
            {
                lblMarquee.Text = _marquee;
                return;
            }
            lblMarquee.Text = lblMarquee.Text.Substring(1);
        }
    }
}
