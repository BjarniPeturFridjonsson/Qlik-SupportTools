using System.Windows.Forms;

namespace FreyrViewer.Ui.Splashes
{
    public partial class SplashLoading : Form
    {
        public SplashLoading()
        {
            InitializeComponent();
        }

        public override string Text
        {
            get => splashControl?.Text ?? string.Empty;
            set => splashControl.Text = value;
        }
    }
}