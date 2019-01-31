using System.Drawing;
using System.Windows.Forms;

namespace FreyrViewer.Ui.Splashes
{
    public sealed partial class SplashControl : UserControl
    {
        private readonly StringFormat _stringFormat;
        private readonly Rectangle _frameRectangle;
        private readonly Rectangle _textRectangle;

        public SplashControl()
        {
            InitializeComponent();

            BackColor = SystemColors.Control;

            _stringFormat = new StringFormat(StringFormat.GenericDefault)
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            _frameRectangle = new Rectangle(0, 0, Width - 1, Height - 1);
            _textRectangle = textLabel.Bounds;
            textLabel.Visible = false;
        }

        public override string Text
        {
            get => textLabel?.Text ?? string.Empty;
            set => textLabel.Text = value;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            e.Graphics.DrawRectangle(Pens.Black, _frameRectangle);
            e.Graphics.DrawString(Text, textLabel.Font, Brushes.Black, _textRectangle, _stringFormat);
        }
    }
}
