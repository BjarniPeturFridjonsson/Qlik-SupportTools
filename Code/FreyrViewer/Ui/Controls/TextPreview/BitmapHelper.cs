using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;

namespace FreyrViewer.Ui.Controls.TextPreview
{
    public class BitmapHelper
    {
        public Bitmap GetBitmapFromSource(BitmapSource source)
        {
            //convert pixel format:
            var bgra32 = new FormatConvertedBitmap();
            bgra32.BeginInit();
            bgra32.Source = source;
            bgra32.DestinationFormat = System.Windows.Media.PixelFormats.Bgra32;
            bgra32.EndInit();

            //now convert it to Bitmap:
            Bitmap bmp = new Bitmap(bgra32.PixelWidth, bgra32.PixelHeight, PixelFormat.Format32bppArgb);
            BitmapData bits = bmp.LockBits(new Rectangle(Point.Empty, bmp.Size), ImageLockMode.WriteOnly, bmp.PixelFormat);
            bgra32.CopyPixels(System.Windows.Int32Rect.Empty, bits.Scan0, bits.Height * bits.Stride, bits.Stride);
            bmp.UnlockBits(bits);
            return bmp;
        }
    }
}
