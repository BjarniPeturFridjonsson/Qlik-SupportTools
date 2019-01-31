using System.Windows.Forms;

namespace FreyrViewer.Ui.Splashes
{
    public partial class MiniSplashControl : UserControl
    {
        public MiniSplashControl()
        {
            InitializeComponent();
            Bounds = pictureBox.Bounds;
        }
    }
}
