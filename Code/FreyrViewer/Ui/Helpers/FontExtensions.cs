using System;
using System.Drawing;

namespace FreyrViewer.Ui.Helpers
{
    public enum FontRescale
    {
        OnlyShrink,
        OnlyGrow,
        ShrinkOrGrow
    }

    public static class FontExtensions
    {
        public static Font Rescale(this Font prototype, string textToFit, Size targetSize, FontRescale rescale)
        {
            Font font = prototype;

            const float minSize = 0.1F;

            using (var b = new Bitmap(1, 1))
            {
                using (var g = Graphics.FromImage(b))
                {
                    bool usingPrototype = true;
                    while (true)
                    {
                        SizeF size = g.MeasureString(textToFit, font);
                        float scaleW = targetSize.Width / size.Width;
                        float scaleH = targetSize.Height / size.Height;

                        float scale = Math.Min(scaleW, scaleH);

                        switch (rescale)
                        {
                            case FontRescale.OnlyShrink:
                                if (scale > 1)
                                {
                                    scale = 1;
                                }
                                break;

                            case FontRescale.OnlyGrow:
                                if (scale < 1)
                                {
                                    scale = 1;
                                }
                                break;
                        }

                        float correctSize = font.Size * scale;

                        if (!usingPrototype)
                        {
                            font.Dispose();
                        }

                        bool giveUp;
                        if (correctSize < minSize)
                        {
                            correctSize = minSize;
                            giveUp = true;
                        }
                        else
                        {
                            giveUp = false;
                        }

                        font = new Font(font.FontFamily, correctSize, font.Style, font.Unit, font.GdiCharSet);
                        if (((scale >= 0.8) && (scale <= 1.2)) || giveUp)
                        {
                            return font;
                        }

                        usingPrototype = false;
                    }
                }
            }
        }

        public static SizeF GetTextSize(this Font prototype, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return SizeF.Empty;
            }

            using (var b = new Bitmap(1, 1))
            {
                using (var g = Graphics.FromImage(b))
                {
                    return g.MeasureString(text, prototype);
                }
            }
        }
    }
}