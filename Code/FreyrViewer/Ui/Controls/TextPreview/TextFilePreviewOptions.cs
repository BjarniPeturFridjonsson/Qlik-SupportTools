using System.Windows.Forms;
using System.Windows.Media;
using FreyrViewer.Services;

namespace FreyrViewer.Ui.Controls.TextPreview
{
    public class TextFilePreviewOptions
    {
        public GenericDataWrapperService DateaWrapperService { get; set; }
        //public Panel PreviewPanel { get; set; }
        public PreviewText TextPreviewer { get; set; }
        public int NrOfTextRowsForTextPreview { get; set; } = 20;
        public int PreviewImageWidth { get; set; } = 250;
        public Color WhiteSpaceColor { get; set; }
        public Color TextColor { get; set; }
        public TabControl CurrentActiveTab { get; set; }
    }
}
