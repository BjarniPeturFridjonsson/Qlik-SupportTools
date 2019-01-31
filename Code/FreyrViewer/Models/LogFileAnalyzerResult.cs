using System.Windows.Media;

namespace FreyrViewer.Models
{
    public class LogFileAnalyzerResult
    {
        public int RowNumber { get; set; }
        public Color? ColorLeft { get; set; }
        public Color? ColorRight { get; set; }
        public int PixelWith { get; set; }
        public string Description { get; set; }
    }
}
